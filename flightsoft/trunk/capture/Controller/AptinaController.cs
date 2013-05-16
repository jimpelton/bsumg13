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

    //true if another thread is using this class
    private bool running=false;


    private Mutex runningMutex;
    private static Semaphore barrierSemaphore;
    private static int barrierCounter;
    private static int nextIdx = 0;

    private ManagedSimpleCapture msc;
    private static int numcams = 2;


    //TODO: move up to ReceiverController?
    public int Errno
    {
        get { return m_errno ; }
        private set { m_errno = value; }
    }
    private int m_errno;

    public string IniFilePath
    {
        get { return m_iniFilePath;  }
        set { m_iniFilePath = value; }
    }

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

    private string m_iniFilePath;

    public AptinaController(BufferPool<byte> bp, string id, bool receiving = true,
        int frame_time = 500) : base(bp, id, receiving, frame_time)
    {
        if (barrierSemaphore == null)
        {
            barrierSemaphore = new Semaphore(0, numcams);
        }

        runningMutex = new Mutex();
        barrierCounter = 0;
        tnum = nextIdx;
        nextIdx += 1;
        msc = new ManagedSimpleCapture();
    }

    protected override bool init()
    {
        bool rval = true;
        int r = ManagedSimpleCapture.managed_InitMidLib(numcams);
        if (r == 0)
        {
            r = msc.managed_OpenTransport(tnum);
        }
        else
        {
            rval = false;
        }

        if (r == 0)
        {
            size = msc.managed_SensorBufferSize();
        }
        else
        {
            rval = false;
        }

        if (size != 0)
        {
            dest = new byte[size];
        }
        else
        {
            rval = false;
        }
        
        
        ////IsReceiving = r == 0 && size != 0;
        //IsReceiving = true;
        //dp.Register(this, "AptianController"+tnum);
        //dp.BroadcastLog(this, String.Format("AptinaController receiving {0}", IsReceiving),1);
        dp.BroadcastLog(this, "AptinaController class done initializing...", 1);
        dp.Broadcast(new AptinaStatusMessage(this, msc.managed_GetWavelength() == 405 ? uGCapture.StatusStr.STAT_GOOD_405 : uGCapture.StatusStr.STAT_GOOD_485));

        return rval;
    }

    public void stop()
    {
        runningMutex.WaitOne();
        IsRunning = false;
        runningMutex.ReleaseMutex();
    }

    public static void go(AptinaController me)
    {
        if (me.IsRunning || !me.IsInit)
        {
            me.dp.Broadcast(new AptinaStatusMessage(me, me.msc.managed_GetWavelength() == 405 ? uGCapture.StatusStr.STAT_ERR_405 : uGCapture.StatusStr.STAT_ERR_485));
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
                byte *data = me.msc.managed_DoCapture();
                if (data == null)
                {
                    me.dp.BroadcastLog(me, "DoCapture returned a null pointer.", 100);
                    me.dp.Broadcast(new AptinaStatusMessage(me, me.msc.managed_GetWavelength() == 405 ? uGCapture.StatusStr.STAT_FAIL_405 : uGCapture.StatusStr.STAT_FAIL_485));// Almost salmon.

                    continue;
                }
                Marshal.Copy(new IntPtr(data), me.dest, 0, (int) me.size);
            }
            
            Buffer<Byte> imagebuffer = me.BufferPool.PopEmpty();

            BufferType bufferType = me.msc.managed_GetWavelength() == 405 ? 
                BufferType.USHORT_IMAGE405 : BufferType.USHORT_IMAGE485;

            imagebuffer.setData(me.dest, bufferType);
            //imagebuffer.FillTime = (ulong)DateTime.Now.Millisecond;//No.
            imagebuffer.FillTime = (ulong)DateTime.Now.Ticks;
            me.BufferPool.PostFull(imagebuffer);
            me.dp.Broadcast(new AptinaStatusMessage(me, me.msc.managed_GetWavelength() == 405 ? uGCapture.StatusStr.STAT_GOOD_405 : uGCapture.StatusStr.STAT_GOOD_485)); // Salmon? No.
        }   
    }

    public override void DoFrame(object source, System.Timers.ElapsedEventArgs e)
    {
        throw new NotImplementedException("The method or operation is not implemented.");
    }
}
}
