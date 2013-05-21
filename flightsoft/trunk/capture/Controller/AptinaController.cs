// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-04-13                                                                      
// ******************************************************************************

using System;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
namespace uGCapture
{
public class AptinaController : ReceiverController
{
    /// <summary>
    /// Initialize midlib2 system.
    /// </summary>
    /// <param name="nCamsReq">Number of cameras to expect.</param>
    /// <param name="hwnd">Window handle (type HWND) to main window.</param>
    /// <param name="attachCallback">Device detach event callback.</param>
    /// <returns>0 on success, 1 or a value from enum mi_error_code on failure.</returns>
    [DllImport("Midlib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int initMidLib2(int nCamsReq, IntPtr hwnd, AttachCallback attachCallback);

    /// <summary>
    /// The wavelength for the given camera index.
    /// Value is based off the fuse id value provided from midlib in when
    /// querying readRegister() with 0x00FA register address.
    /// </summary>
    /// <param name="camIdx">Wavelength to get</param>
    /// <returns>an int that is the wavelength of the camera filter.</returns>
    [DllImport("Midlib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int getWavelengthIdx(int camIdx);

    /// <summary>
    /// Stop the transport for this camera index.
    /// </summary>
    [DllImport("Midlib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void stopTransportIdx(int camIdx);

    [DllImport("Midlib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern ulong sensorBufferSizeIdx(int camIdx);

    [DllImport("Midlib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern UIntPtr doCaptureIdx(int camIdx);

    [DllImport("Midlib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int setDeviceCallback(
        IntPtr hwnd, 
        [MarshalAs(UnmanagedType.FunctionPtr)] AttachCallback cb_ptr
    );

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void AttachCallback(int camIdx);

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
    public StatusStr Status_Fail 
    {
        get { return STATUSSTR_FAIL; }
    }
    private StatusStr STATUSSTR_FAIL;

    public StatusStr Status_Good
    {
        get { return STATUSSTR_GOOD; }
    }
    private StatusStr STATUSSTR_GOOD;

    public StatusStr Status_Err
    {
        get { return STATUSSTR_ERR; }
    }
    private StatusStr STATUSSTR_ERR;

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
    private const int NUMCAMS = 2;

    public AptinaController(BufferPool<byte> bp, string id, bool receiving = true)
     : base(bp, id, receiving)
    {
        if (barrierSemaphore == null)
        {
            barrierSemaphore = new Semaphore(0, NUMCAMS);
        }

        tnum = nextIdx;   //the next controller will have tnum=tnum+1.
        nextIdx += 1;  
    }

    protected override bool init()
    {
        bool rval = true; int r = 0;

        r = initMidLib2(NUMCAMS, Hwnd, 
            (camIdx) =>
                {
                    dp.Broadcast(
                        new AptinaStatusMessage(this, STATUSSTR_FAIL,
                        Str.GetErrStr(ErrStr.APTINA_FAIL_DISCONNECT) + " " + waveLength));
                });

        if (r != 0)
        {
            rval = false;
            Errno = ErrStr.INIT_FAIL_APTINA_INITMIDLIB;
        }
        else
        {
            waveLength = getWavelengthIdx(tnum);
            size = sensorBufferSizeIdx(tnum);

            if (waveLength == 405)
            {
                bufferType = BufferType.USHORT_IMAGE405;
                STATUSSTR_FAIL = StatusStr.STAT_FAIL_405;
                STATUSSTR_GOOD = StatusStr.STAT_GOOD_405;
            }
            else
            {
                bufferType = BufferType.USHORT_IMAGE485;
                STATUSSTR_FAIL = StatusStr.STAT_FAIL_485;
                STATUSSTR_GOOD = StatusStr.STAT_GOOD_485;
            }
        }
    
        {
            rval = false;
            Errno = ErrStr.INIT_FAIL_APTINA_OPENTRANSPORT;
        }

        if (size != 0)
        {
            dest = new byte[size];
        }
        else
        {
            rval = false;
            Errno = ErrStr.INIT_FAIL_APTINA;
        }

        dp.BroadcastLog(this, "AptinaController class done initializing...", 1);
        if (rval)
            dp.Broadcast(new AptinaStatusMessage(this, STATUSSTR_GOOD));
        else
            dp.Broadcast(new AptinaStatusMessage(this, STATUSSTR_FAIL));

        return rval;
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
        if (me.IsRunning || !me.IsInit)
        {
            return;
        }
        me.IsRunning = true;

        while (true)
        {
            int curval = Interlocked.Increment(ref barrierCounter);
            if (curval == NUMCAMS)
            {
                barrierCounter = 0;
                barrierSemaphore.Release(NUMCAMS - 1);
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
                    me.dp.Broadcast(new AptinaStatusMessage(me, me.STATUSSTR_FAIL));  // Almost salmon.
                    continue;
                }
                Marshal.Copy(new IntPtr(data), me.dest, 0, (int) me.size);
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
//    StatusStr.STAT_ERR_405 : StatusStr.STAT_ERR_485));