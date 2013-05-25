// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-04-13                                                                      
// ******************************************************************************

using System;
using System.Management;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;

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


    //the buffer size for this controller
    private ulong size;
    
    //the thread number (and camera index) for this controller.
    private int tnum;
    
    //destination buffer for data from ManagedSimpleCapture
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
    /// The status fail code for this controller,
    /// either STAT_FAIL405 or STAT_FAIL485
    /// </summary>
    public Status Status_Fail 
    {
        get { return STATUSSTR_FAIL; }
    }
    private Status STATUSSTR_FAIL;

    /// <summary>
    /// Status good code for this controller.
    /// </summary>
    public Status Status_Good
    {
        get { return STATUSSTR_GOOD; }
    }
    private Status STATUSSTR_GOOD;

    /// <summary>
    /// Status err code for this controller.
    /// </summary>
    public Status Status_Err
    {
        get { return STATUSSTR_ERR; }
    }
    private Status STATUSSTR_ERR;

    /// <summary>
    /// The last MiError that occured.
    /// </summary>
    public Str.MiErrorCode MiError { get; private set; }


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

    private bool IsCurrentlyDetached
    {
        get
        {
            lock (detachedMutex)
            {
                return isCurrentlyIsDetached;
            }
        }
        set
        {
            lock (detachedMutex)
            {
                isCurrentlyIsDetached = value;
            }
        }
    }
    private bool isCurrentlyIsDetached;
    private object detachedMutex = new object();

    private static Semaphore barrierSemaphore;
    private static int barrierCounter = 0;

    private static int nextIdx = 0;
    private static int numCams = 2;
    private AttachCallback attach_cb;

    private static ManagementEventWatcher w;


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

        MiError = Str.MiErrorCode.MI_CAMERA_ERROR;
    }


    /****************************************************************************
     * Attach/Detach Handling
     * ***************************************************************************/


     

    // Callback from native space informing us that a camera has disconnected.
    // Sets isDeatched=true, which checks wmi on the next heartbeat until connected again.
    // We can't add the usb handler event here (even though it's the best place to do it)
    // because this callback method is executed by a thread in native space (midlib thread?).
    private void AttachCb(int camIdx)
    {
        IsCurrentlyDetached = true;
        IsRunning = false;

        dp.Broadcast(new AptinaStatusMessage(WaveLength, this, Status.STAT_FAIL, ErrStr.APTINA_DISCONNECT));
        dp.BroadcastLog(this, Status.STAT_FAIL, Str.GetErrStr(ErrStr.APTINA_DISCONNECT), WaveLength.ToString());
        //Add_AttachUSBHandler();
    }

    public static void Add_AttachUSBHandler()
    {
        WqlEventQuery q;
        ManagementScope scope = new ManagementScope("root\\CIMV2");
        scope.Options.EnablePrivileges = true;

        try
        {
            q = new WqlEventQuery();
            q.EventClassName = "__InstanceCreationEvent";
            q.WithinInterval = new TimeSpan(0, 0, 3);
            q.Condition = @"TargetInstance ISA Win32_USBControllerdevice";
            w = new ManagementEventWatcher(scope, q);
            w.EventArrived += USBAdded;
            w.Start();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            if (w != null)
            {
                w.Stop();
            }
        }
    }

    public static void USBAdded(object sender, EventArgs e)
    {
        Console.WriteLine("something added");
    }



    /*************************************************
     * init()
     ************************************************/
    
    protected override bool init()
    {
        bool rval = true; 
        int r = 0;
        int mi_success = (int)Str.MiErrorCode.MI_CAMERA_SUCCESS;
        
        r = initMidLib2(numCams, Hwnd, attach_cb);

        if (r != mi_success)
        {
            Errno = ErrStr.INIT_FAIL_APTINA_INITMIDLIB;
            MiError = (Str.MiErrorCode) r;
            return false;
        }

        r = openTransportIdx(tnum);
        if (r != mi_success)
        {
            Errno = ErrStr.INIT_FAIL_APTINA_OPENTRANSPORT;
            MiError = (Str.MiErrorCode) r;
            return false;
        }

        waveLength = getWavelengthIdx(tnum);
        if (waveLength == 0)
        {
            Errno = ErrStr.INIT_FAIL_APTINA_FUSE_ERROR;
        }
        bufferType = waveLength == 405 ? 
            BufferType.USHORT_IMAGE405 : BufferType.USHORT_IMAGE485;
        setErrorTypes(waveLength); 

        size = sensorBufferSizeIdx(tnum);
        if (size <= 0)
        {
            Errno = ErrStr.INIT_FAIL_APTINA_ZERO_SIZE;
            return false;
        }
        dest = new byte[size];

        MiError = Str.MiErrorCode.MI_CAMERA_SUCCESS;
        return rval;
    }

    // helper to init
    private void setErrorTypes(int wl)
    {
        if (wl == 405)
        {
            bufferType = BufferType.USHORT_IMAGE405;
            STATUSSTR_FAIL = Status.STAT_FAIL_405;
            STATUSSTR_GOOD = Status.STAT_GOOD_405;
            STATUSSTR_ERR = Status.STAT_ERR_405;
        }
        else
        {
            bufferType = BufferType.USHORT_IMAGE485;
            STATUSSTR_FAIL = Status.STAT_FAIL_485;
            STATUSSTR_GOOD = Status.STAT_GOOD_485;
            STATUSSTR_ERR = Status.STAT_ERR_485;
        }
    }




    /****************************************************************************
     * Thread func
     ****************************************************************************/
    public void stop()
    {
        lock (runningMutex)
        {
            IsRunning = false;
        }
    }

    public static void go(AptinaController me)
    {
        if (me.IsRunning || !me.IsInit) { return; }
        me.IsRunning = true;

        while (true)
        {
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
            //Console.WriteLine("Aptina Controller Capture {0} begin. Time: {1:g}", me.tnum, DateTime.Now.TimeOfDay);

            if (!me.IsRunning)
            {
                string mes = "Aptina controller " + me.tnum + " exiting"; 

                Console.WriteLine(mes);
                me.dp.BroadcastLog(me, mes, 3);
                barrierSemaphore.Release(1);
                break;
            }
            
            int miErr = 0;
            long time = me.GetUTCMillis();
            unsafe
            {
                byte* data = (byte*)doCaptureIdx(me.tnum, ref miErr); 
                if (miErr != 0)
                {
                    Console.WriteLine(Str.MiErrStr[(Str.MiErrorCode)miErr]);
                    me.dp.BroadcastLog(me, Status.STAT_ERR, Str.MiErrStr[(Str.MiErrorCode)miErr]);
                }
                if (data == null)
                {
                    me.dp.Broadcast(new AptinaStatusMessage(me.WaveLength, me, Status.STAT_FAIL, 
                        ErrStr.APTINA_FAIL_CAPTURE_NULLBUFFER));  // Almost salmon.
                    continue;
                }
                Marshal.Copy(new IntPtr(data), me.dest, 0, me.dest.Length);
            }

            Buffer<Byte> imagebuffer = me.BufferPool.PopEmpty();            
            imagebuffer.setData(me.dest, me.bufferType);
            imagebuffer.FillTime = time;
            me.BufferPool.PostFull(imagebuffer);
            //me.dp.Broadcast(new AptinaStatusMessage(me.WaveLength, me, Status.STAT_GOOD, 0));        // Salmon? No.
        }

        //TODO: stop transport.   
    }

        public override void exHeartBeatMessage(Receiver r, Message m)
        {
            bool detached;
            lock (detachedMutex)
            {
                detached = isCurrentlyIsDetached;
            }

            if (detached)
            {
                int err = stopTransportIdx(tnum);
                if (err != (int) Str.MiErrorCode.MI_CAMERA_SUCCESS)
                {
                    dp.BroadcastLog(this, Status.STAT_FAIL, 
                        "Camera was detached, and stopTransportIdx returned an error.");
                }

               err = openTransportIdx(tnum);
               if (err == 0)
               {
                   detached = false;
                   dp.Broadcast(new AptinaStatusMessage(WaveLength, this, Status.STAT_GOOD,
                       Str.GetErrStr(ErrStr.APTINA_RECONNECT)));

                   Console.Error.WriteLine("Reconnected to Aptina camera " + WaveLength);
                   
                   Errno = ErrStr.ERR_NONE;
               }
               else
               {
                   Errno = ErrStr.APTINA_DISCONNECT;
               }
            }

            lock (detachedMutex)
            {
                isCurrentlyIsDetached = detached;
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


}

}

//me.dp.Broadcast(new AptinaStatusMessage(me, 
//    me.waveLength == 405 ? 
//    Stat.STAT_ERR_405 : Stat.STAT_ERR_485));