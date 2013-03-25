using System;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
namespace uGCapture
{
public class AptinaController : ReceiverController
{
    private ulong size;
    private int tnum;
    private byte[] dest;
    private Mutex runningMutex;
    private bool running=false;
    private ManagedSimpleCapture msc;
    

    private static Semaphore barrierSemaphore;
    private static int barrierCounter;
    private static int nextIdx = 0;
    private static int numcams = 2;

    private int m_errno;
    public int Errno
    {
        get { return m_errno ; }
        private set { m_errno = value; }
    }

    private string m_iniFilePath;
    public string IniFilePath
    {
        get { return m_iniFilePath;  }
        set { m_iniFilePath = value; }
    }
    public AptinaController(BufferPool<byte> bp) 
        : base(bp)
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

    public override void init()
    {
        int r = ManagedSimpleCapture.managed_InitMidLib(numcams);
        if (r == 0) { r = msc.managed_OpenTransport(tnum); }
        size = msc.managed_SensorBufferSize();
        dest = new byte[size];

        Receiving = true;
        dp.Register(this, "AptianController"+tnum);
        dp.BroadcastLog(this, "AptinaController class initialized...", 1);
    }

    public void stop()
    {
        runningMutex.WaitOne();
        running = false;
        runningMutex.ReleaseMutex();
    }

    public static void go(AptinaController me)
    {
        int curval;
        me.runningMutex.WaitOne();
        if (me.running)
        {
            me.runningMutex.ReleaseMutex();
            return;
        }
        me.running = true;
        me.runningMutex.ReleaseMutex();

        while (true)
        {

            Console.WriteLine(String.Format("{0} top. {1}", me.tnum, DateTime.Now.Millisecond));
            curval = Interlocked.Increment(ref barrierCounter);
            if (curval == numcams)
            {
                barrierCounter = 0;
                barrierSemaphore.Release(numcams - 1);
            }
            else
            {
                barrierSemaphore.WaitOne();
            }

            me.runningMutex.WaitOne();
            if (!me.running)
            {
                me.runningMutex.ReleaseMutex();
                string mes = String.Format("{0} exiting.", me.tnum);
                Console.WriteLine(mes, me.tnum);
                me.dp.BroadcastLog(me, mes, 3);
                barrierSemaphore.Release(1);
                return;
            }
            me.runningMutex.ReleaseMutex();

            unsafe
            {
                byte *data = me.msc.managed_DoCapture();
                if (data == null)
                {
                    me.dp.BroadcastLog(me, "DoCapture returned a null pointer.", 100);
                    continue;
                }
                Marshal.Copy(new IntPtr(data), me.dest, 0, (int) me.size);
            }

            Buffer<Byte> imagebuffer = null;
            while (imagebuffer == null)
            {
                imagebuffer = me.BufferPool.PopEmpty();
                Thread.Sleep(10);
            }

            BufferType bufferType = me.msc.managed_GetWavelength() == 405 ? 
                BufferType.IMAGE405 : BufferType.IMAGE485;

            imagebuffer.setData(me.dest, bufferType);
            //File.WriteAllBytes(String.Format("data_{0}_{1}.raw",me.msc.managed_GetWavelength(), me.nextIdx++), me.dest);
            imagebuffer.Text = String.Format("{0}", DateTime.Now.Millisecond);
            me.BufferPool.PostFull(imagebuffer);
            
        }   
    }

    public override void DoFrame(object source, System.Timers.ElapsedEventArgs e)
    {
        throw new Exception("The method or operation is not implemented.");
    }
}
}
