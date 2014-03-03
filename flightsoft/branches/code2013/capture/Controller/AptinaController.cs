// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-04-13                                                                      
// ******************************************************************************

using System;
using System.Collections.Generic;
using System.Management;
using System.Threading;
using System.Runtime.InteropServices;

namespace uGCapture
{
public class AptinaController : ReceiverController
{
    // Initialize midlib2 system.
    // Return values: 0 on success, 1 or a value from enum mi_error_code on failure.
    [DllImport("Midlib.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int initMidLib2(int nCamsReq, IntPtr hwnd, AttachCallback attachCallback);

    // The wavelength for the given camera index.
    // Value is based off the fuse id value provided from midlib in when
    // querying readRegister() with 0x00FA register address.
    // 
    // Returns: An int that is the wavelength of the camera filter.
    [DllImport("Midlib.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int getWavelengthIdx(int camIdx);

    // Stop the transport for this camera index.
    [DllImport("Midlib.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int stopTransportIdx(int camIdx);

    [DllImport("Midlib.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int openTransportIdx(int camIdx);

    // Returns the buffer size for the sensor for the given camera index.
    [DllImport("Midlib.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern ulong sensorBufferSizeIdx(int camIdx);

    // Perform a capture for this camera.
    [DllImport("Midlib.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern UIntPtr doCaptureIdx(int camIdx, ref int errval);

    // Set the detach event callback for this camera.
    [DllImport("Midlib.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int setDeviceCallback(
        IntPtr hwnd,
        [MarshalAs(UnmanagedType.FunctionPtr)] AttachCallback cb_ptr
        );

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void AttachCallback(int camIdx);


    /// <summary>
    /// Window handle from GUI used for the
    /// Midlib2 callback.
    /// </summary>
    public IntPtr Hwnd
    {
        get { return m_hwnd; }
        set { m_hwnd = value; }
    }
    private unsafe IntPtr m_hwnd = IntPtr.Zero;

    //protected Status Status_405 = Status.STAT_ERR;
    //protected Status Status_485 = Status.STAT_ERR;
    private Status last_status = Status.STAT_ERR;

    //the buffer size for this controller
    private ulong size;
    
    //the thread number (and camera index) for this controller.
    private int tnum;
    
    //destination buffer for data from midlib
    private byte[] dest;

    //true if another thread is running for this instance.
    private bool running = false;
    private object runningMutex = new object();

    /// <summary>
    /// The buffertype for this aptina controller, 
    /// either a USHORT_IMAGE405 or USHORT_IMAGE485.
    /// </summary>
    public BufferType Type
    {
        get { return bufferType; }
    }
    private BufferType bufferType;

    /// <summary>
    /// The last MiError that occured.
    /// </summary>
    public MiErrorCode MiError { get; private set; }

    /// <summary>
    /// filter wavelength for this controller.
    /// </summary>
    public int WaveLength
    { 
        get { return waveLength; } 
    }
    private int waveLength;

    // aptina configuration file.
    public string IniFilePath
    {
        get { return m_iniFilePath;  }
        set { m_iniFilePath = value; }
    }
    private string m_iniFilePath;

    /// <summary>
    /// true if this instance is associated with 
    /// a running thread.
    /// </summary>
    public bool IsRunning
    {
        get
        {
            lock (runningMutex)
            {
                return running;
            }
        }
        private set
        {
            lock (runningMutex)
            {
                running = value;
            }
        }
    }

    // set by init() and the midlib detached callback function and
    // checked by the worker function go() to determine if we should
    // wait for a reattach event.
    private bool IsCurrentlyDetached
    {
        get
        {
            lock (detachedMutex)
            {
                return isCurrentlyDetached;
            }
        }
        set
        {
            lock (detachedMutex)
            {
                isCurrentlyDetached = value;
            }
        }
    }
    private bool isCurrentlyDetached = true;
    private object detachedMutex = new object();

    private static Semaphore barrierSemaphore;
    private static int barrierCounter = 0;

    private static int nextIdx = 0;
    private static int numCams = 2;
    private AttachCallback attach_cb;

    private static ManagementEventWatcher w;
    private static string[] pnpIDs = new string[2];
    private const int MAX_CAMS=2;


    /****************************************************************************
     * Constructor
     ****************************************************************************/

    public AptinaController(BufferPool<byte> bp, string id, bool receiving=true, bool executing=false)
        : base(bp, id, receiving, executing)
    {
        attach_cb = AttachCb;

        if (barrierSemaphore == null)
        {
            barrierSemaphore = new Semaphore(0, numCams);
        }

        tnum = nextIdx;   //the next controller will have tnum=tnum+1.
        nextIdx += 1;

        MiError = MiErrorCode.MI_CAMERA_ERROR;
    }


    /****************************************************************************
     * Attach/Detach Handling
     * ***************************************************************************/

    // AttachCb() is a callback from native space informing us that a camera has disconnected.
    // Sets isCurrentlyDetached=true, which checks wmi on the next heartbeat until connected again.
    private void AttachCb(int camIdx)
    {
        IsCurrentlyDetached = true;
        IsRunning = false;
        IsExecuting = false;

        CheckedStatusBroadcast(ref last_status, 
            new AptinaStatusMessage(WaveLength, this, Status.STAT_FAIL, ErrStr.APTINA_DISCONNECT));
        dp.BroadcastLog(this, Status.STAT_FAIL, Str.GetErrStr(ErrStr.APTINA_DISCONNECT), WaveLength.ToString());
    }


    /*************************************************
     * init()
     ************************************************/
    protected override bool init()
    {
        bool rval = true; 
        int r = 0;

        determinePnpIdOfAptinaCamerasWithWindowsWMI();

        int mi_success = (int)MiErrorCode.MI_CAMERA_SUCCESS;
        
        r = initMidLib2(numCams, Hwnd, attach_cb);

        if (r != mi_success)
        {
            Errno = ErrStr.INIT_FAIL_APTINA_INITMIDLIB;
            MiError = (MiErrorCode)r;
            return false;
        }

        r = openTransportIdx(tnum);
        if (r != mi_success)
        {
            Errno = ErrStr.INIT_FAIL_APTINA_OPENTRANSPORT;
            MiError = (MiErrorCode)r;
            return false;
        }

        IsCurrentlyDetached = false;

        waveLength = getWavelengthIdx(tnum);
        if (waveLength == 0)
        {
            Errno = ErrStr.INIT_FAIL_APTINA_FUSE_ERROR;
        }

        bufferType = waveLength == 405 ? 
            BufferType.USHORT_IMAGE405 : BufferType.USHORT_IMAGE485;
        
        size = sensorBufferSizeIdx(tnum);
        if (size <= 0)
        {
            Errno = ErrStr.INIT_FAIL_APTINA_ZERO_SIZE;
            return false;
        }
        dest = new byte[size];

        MiError = MiErrorCode.MI_CAMERA_SUCCESS;
        last_status = Status.STAT_GOOD;
        return rval;
    }

    
    private void determinePnpIdOfAptinaCamerasWithWindowsWMI()
    {
         ManagementObjectSearcher usbDevices = 
            new ManagementObjectSearcher("SELECT * FROM Win32_USBHub");

        int pnpIdx = 0;
         foreach (ManagementObject mo in usbDevices.Get())
         {
            string id = mo["PNPDeviceID"].ToString();
            if (id.Contains("VID_20FB&PID_100D")) { pnpIDs[pnpIdx++] = id; }
         }
    }

    /****************************************************************************
     * Thread func
     ****************************************************************************/
    public void stop()
    {
        IsRunning = false;
        dp.BroadcastLog(this, Status.STAT_ERR, "Aptina controller stopping.", WaveLength.ToString());
    }

    public static void go(AptinaController me)
    {
        if (me.IsRunning || !me.IsInit) { return; }
        me.IsRunning = true;

        while (true)
        {
            if (me.IsCurrentlyDetached)
            {
                me.waitForReattachEvent();
            }

            int curval = Interlocked.Increment(ref barrierCounter);
            if (curval == numCams)
            {
                barrierCounter = 0;
                barrierSemaphore.Release(numCams - 1);
            }
            else
            {
                barrierSemaphore.WaitOne();
            }

            if (!me.IsRunning)
            {
                barrierSemaphore.Release(1);
                break;
            }

            doCapture(me);

        }  /*  while  */

        // stop transport for this cam.
        int err = stopTransportIdx(me.tnum);
        if ((MiErrorCode) err != MiErrorCode.MI_CAMERA_SUCCESS)
        {
            //me.dp.BroadcastLog(me, Status.STAT_ERR, 
            //    "Aptina controller couldn't stop transport.");

            me.CheckedStatusBroadcast(ref me.last_status, new AptinaStatusMessage(me.WaveLength, me, Status.STAT_ERR, 
                "Aptina controller couldn't stop transport."));
        }
        else
        {
            //me.dp.BroadcastLog(me, Status.STAT_FAIL, 
            //    "Aptina controller stopped transport and worker thread is exiting.");

            me.CheckedStatusBroadcast(ref me.last_status, new AptinaStatusMessage(me.WaveLength, me, Status.STAT_FAIL, 
                "Aptina thread exiting."));
        }
    }

    private static void doCapture(AptinaController me)
    {
        int miErr = 0;
        long time = me.GetUTCMillis();
        unsafe
        {
            byte* data = (byte*)doCaptureIdx(me.tnum, ref miErr);
            if (miErr != 0)
            {
                me.dp.BroadcastLog(me, Status.STAT_ERR, Str.GetMiErrStr((MiErrorCode)miErr));
            }
            if (data == null)
            {
                me.CheckedStatusBroadcast(ref me.last_status, new AptinaStatusMessage(me.WaveLength, 
                    me, Status.STAT_ERR, ErrStr.APTINA_FAIL_CAPTURE_NULLBUFFER));  // Almost salmon.
                return;
            }
            Marshal.Copy(new IntPtr(data), me.dest, 0, me.dest.Length);
        }

        Buffer<Byte> imagebuffer = me.BufferPool.PopEmpty();
        imagebuffer.setData(me.dest, me.bufferType);
        imagebuffer.FillTime = time;
        me.BufferPool.PostFull(imagebuffer);
        //me.dp.Broadcast(new AptinaStatusMessage(me.WaveLength, me, Status.STAT_GOOD, 0));  
    }

    /* Register with the WMI for a usb attach event.
       Polls at 1 second intervals. */
    private void waitForReattachEvent()
    {
        WqlEventQuery query =
            new WqlEventQuery("__InstanceCreationEvent",
            new TimeSpan(0, 0, 1),
            "TargetInstance isa \"Win32_USBControllerdevice\"");

        // Initialize an event watcher and subscribe to events 
        // that match this query
        ManagementEventWatcher watcher = new ManagementEventWatcher();
        watcher.Query = query;

        // times out watcher.WaitForNextEvent in 5 seconds
        //watcher.Options.Timeout = new TimeSpan(0, 0, 5);
        int i = 0;
        while (i < 5)
        {
            // Block until the next event occurs 
            watcher.WaitForNextEvent();

            ManagementObjectSearcher usbDevices = 
                new ManagementObjectSearcher("SELECT * FROM Win32_USBHub");
            
            int camIdx = -1;
            foreach (ManagementObject mo in usbDevices.Get())
            {
                string id = mo["PNPDeviceID"].ToString();
                camIdx = Array.FindIndex(pnpIDs, id.Equals);
                
                if (camIdx >= 0) break;
            }

            if (camIdx < 0) continue;

            MiError = (MiErrorCode)openTransportIdx(camIdx);
            if (MiError == MiErrorCode.MI_CAMERA_SUCCESS)
            {
                CheckedStatusBroadcast(ref last_status, new AptinaStatusMessage(WaveLength, this, Status.STAT_ATCH, 
                    "Camera transport reopened."));
                break;
            }
            
            i += 1;
        }

        //cancel subscription to the WMI.
        watcher.Stop();
    }

    //public override void exHeartBeatMessage(Receiver r, Message m)
    //{
    //    bool detached = IsCurrentlyDetached;
    //    if (!detached)
    //    {
    //        int err = openTransportIdx(tnum);
    //        if (err == 0)
    //        {
    //            dp.Broadcast(new AptinaStatusMessage(WaveLength, this, Status.STAT_GOOD,
    //                Str.GetErrStr(ErrStr.APTINA_RECONNECT)));

    //            Console.Error.WriteLine("Reconnected to Aptina camera " + WaveLength);
                   
    //            Errno = ErrStr.ERR_NONE;
    //        }
    //        else
    //        {
    //            Errno = ErrStr.APTINA_DISCONNECT;
    //        }
    //    }

    //    lock (detachedMutex)
    //    {
    //        isCurrentlyDetached = detached;
    //    }
    //}

}

}

//public void WaitForUSBChangeEvent(object sender, EventArrivedEventArgs e)
//{
//    (sender as ManagementEventWatcher).Stop();
//    // watcher.Stop();
//    Invoke((MethodInvoker)delegate
//    {
//        ManagementObjectSearcher usbDevs = new ManagementObjectSearcher("SELECT * FROM Win32_USBHub");
//        // Loop through each object (disk) retrieved by WMI
//        //this.comboBox1.Items.Clear();
//        //
//        foreach (ManagementObject manObj in usbDevs.Get())
//        {
//            if (manObj.)

//            if (!listBox1.Items.Contains(moDiskete["DeviceID"]))
//            {
//                // Add the HDD to the list (use the Model field as the item's caption)
//                listBox1.Items.Add(moDiskete["DeviceID"].ToString() + " " + moDiskete["Description"]);
//                listBox1.Items.Add("\t" + moDiskete["PNPDeviceID"]);

//            }
//        }
//        this.comboBox1.SelectedIndex = 0;
//    });
//    (sender as ManagementEventWatcher).Start();
//}

//public static void Add_AttachUSBHandler()
//{
//    WqlEventQuery q;
//    ManagementScope scope = new ManagementScope("root\\CIMV2");
//    scope.Options.EnablePrivileges = true;

//    try
//    {
//        q = new WqlEventQuery();
//        q.EventClassName = "__InstanceCreationEvent";
//        q.WithinInterval = new TimeSpan(0, 0, 3);
//        q.Condition = @"TargetInstance ISA Win32_USBControllerdevice";
//        w = new ManagementEventWatcher(scope, q);
//        w.EventArrived += USBAdded;
//        w.WaitForNextEvent();
//    }
//    catch (Exception e)
//    {
//        Console.WriteLine(e.Message);
//        if (w != null)
//        {
//            w.Stop();
//        }
//    }
//}