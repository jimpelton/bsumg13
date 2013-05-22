// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-04-13                                                                      
// ******************************************************************************

using System;
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
    private static extern void stopTransportIdx(int camIdx);

    // Returns the buffer size for the sensor for the given camera index.
    [DllImport("Midlib.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern ulong sensorBufferSizeIdx(int camIdx);

    // Perform a capture for this camera.
    [DllImport("Midlib.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern UIntPtr doCaptureIdx(int camIdx);

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

    public Status Status_Good
    {
        get { return STATUSSTR_GOOD; }
    }
    private Status STATUSSTR_GOOD;

    public Status Status_Err
    {
        get { return STATUSSTR_ERR; }
    }
    private Status STATUSSTR_ERR;

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

    private static Semaphore barrierSemaphore;
    private static int barrierCounter = 0;

    private static int nextIdx = 0;
    private static int numCams = 2;

    public AptinaController(BufferPool<byte> bp, string id, bool receiving=true, bool executing=false)
     : base(bp, id, receiving, executing)
    {
        if (barrierSemaphore == null)
        {
            barrierSemaphore = new Semaphore(0, numCams);
        }
        tnum = nextIdx;   //the next controller will have tnum=tnum+1.
        nextIdx += 1;
    }

    protected override bool init()
    {
        bool rval = true; int r = 0;

        r = initMidLib2(numCams, Hwnd, 
            (camIdx) =>
                {
                    dp.Broadcast(new AptinaStatusMessage(this, STATUSSTR_FAIL, ErrStr.APTINA_DISCONNECT));
                    dp.BroadcastLog(this, Str.GetErrStr(ErrStr.APTINA_DISCONNECT) + " " + waveLength, 100);
                });

        if (r == 0)
        {
            waveLength = getWavelengthIdx(tnum);
            size = sensorBufferSizeIdx(tnum);
            setErrors(waveLength);
        }
        else
        {
            rval = false;
            Errno = ErrStr.INIT_FAIL_APTINA_INITMIDLIB;
        }

        if (size > 0)
        {
            dest = new byte[size];
        }
        else
        {
            rval = false;
            Errno = ErrStr.INIT_FAIL_APTINA_ZERO_SIZE;
        }
        return rval;
    }

    private void setErrors(int wl)
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

    public void stop()
    {
        lock (runningMutex)
        {
            IsRunning = false;
        }
    }

    private void updateStatus()
    {
        
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
                return;
            }

            unsafe
            {
                byte* data = (byte*)doCaptureIdx(me.tnum); 
                if (data == null)
                {
                    me.dp.Broadcast(new AptinaStatusMessage(me, me.STATUSSTR_FAIL, 
                        ErrStr.APTINA_FAIL_CAPTURE_NULLBUFFER));  // Almost salmon.
                    continue;
                }
                Marshal.Copy(new IntPtr(data), me.dest, 0, me.dest.Length);
            }

            Buffer<Byte> imagebuffer = me.BufferPool.PopEmpty();            
            imagebuffer.setData(me.dest, me.bufferType);
            imagebuffer.FillTime = me.GetUTCMillis();
            me.BufferPool.PostFull(imagebuffer);
            me.dp.Broadcast(new AptinaStatusMessage(me, me.STATUSSTR_GOOD));        // Salmon? No.
        }   
    }
}

}

//me.dp.Broadcast(new AptinaStatusMessage(me, 
//    me.waveLength == 405 ? 
//    Status.STAT_ERR_405 : Status.STAT_ERR_485));