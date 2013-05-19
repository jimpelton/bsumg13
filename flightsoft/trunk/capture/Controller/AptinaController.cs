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
    //the buffer size for this controller
    private ulong size;
    
    //the thread number for this controller.
    private int tnum;
    
    //destination buffer for data from ManagedSimpleCapture
    private byte[] dest;

    //this camera's wrapper around simple capture.
    //private ManagedSimpleCapture msc;

    //true if another thread is running for this instance.
    private bool running = false;
    private Mutex runningMutex;

    private BufferType bufferType;
    private StatusStr STATUSSTR_FAIL;
    private StatusStr STATUSSTR_GOOD;

    //wavelength for this controller
    public int WaveLength
    { 
        get { return waveLength; } 
    }
    private int waveLength;


    private static Semaphore barrierSemaphore;
    private static int barrierCounter;
    private static int nextIdx = 0;
    private const int numcams = 2;
    
   
    public string IniFilePath
    {
        get { return m_iniFilePath;  }
        set { m_iniFilePath = value; }
    }
    private string m_iniFilePath;

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

    public IntPtr Hwnd
    {
        get { return m_hwnd; }
        set { m_hwnd = value; }
    }
    private unsafe IntPtr m_hwnd = IntPtr.Zero;
    
    [DllImport("Midlib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int initMidLib2(int nCamsReq);

    [DllImport("Midlib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int getWavelength(int camIdx);

    [DllImport("Midlib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int openTransport(int camidx);

    [DllImport("Midlib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void stopTransport();

    [DllImport("Midlib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern ulong sensorBufferSize(int camIdx);

    [DllImport("Midlib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern UIntPtr doCapture(int camIdx);


    public AptinaController(BufferPool<byte> bp, string id, bool receiving = true)
     : base(bp, id, receiving)
    {
        if (barrierSemaphore == null)
        {
            barrierSemaphore = new Semaphore(0, numcams);
        }

        runningMutex = new Mutex();
        barrierCounter = 0;
        
        //the next AptinaController initialized will have tnum=tnum+1.
        tnum = nextIdx;
        nextIdx += 1;  
    }

    

    protected override bool init()
    {
        bool rval = true;
        int r = 0;

        r = initMidLib2(numcams);
        
        if (r != 0)
        {
            rval = false;
            Errno = ErrStr.INIT_FAIL_APTINA_INITMIDLIB;
        }

        if (r == 0)
        {
            waveLength = getWavelength(tnum);
            size = sensorBufferSize(tnum);

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
        else
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
            dp.Broadcast(new AptinaStatusMessage(this, STATUSSTR_GOOD, 
                Str.GetErrStr(ErrStr.INIT_FAIL_APTINA)));
        else
            dp.Broadcast(new AptinaStatusMessage(this, STATUSSTR_FAIL, 
                Str.GetErrStr(ErrStr.INIT_OK_APTINA)));

        return rval;
    }

    public void stop()
    {
        runningMutex.WaitOne();
        IsRunning = false;
        runningMutex.ReleaseMutex();
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
            if (curval == numcams)
            {
                barrierCounter = 0;
                barrierSemaphore.Release(numcams - 1);
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
                byte* data = (byte*)doCapture(me.tnum); 
                if (data == null)
                {
                    me.dp.Broadcast(new AptinaStatusMessage(me, me.STATUSSTR_FAIL, 
                        Str.GetErrStr(ErrStr.CAPTURE_FAIL_APTINA_NULLBUFFER)));  // Almost salmon.
                    continue;
                }
                Marshal.Copy(new IntPtr(data), me.dest, 0, (int) me.size);
            }

            Buffer<Byte> imagebuffer = me.BufferPool.PopEmpty();            
            imagebuffer.setData(me.dest, me.bufferType);
            imagebuffer.FillTime = me.GetUTCMillis();
            me.BufferPool.PostFull(imagebuffer);
            //me.dp.Broadcast(new AptinaStatusMessage(me, me.STATUSSTR_GOOD));        // Salmon? No.
        }   
    }
}
}

//me.dp.Broadcast(new AptinaStatusMessage(me, 
//    me.waveLength == 405 ? 
//    StatusStr.STAT_ERR_405 : StatusStr.STAT_ERR_485));