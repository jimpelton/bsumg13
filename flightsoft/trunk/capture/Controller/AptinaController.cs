using System;
using System.Threading;
using System.Runtime.InteropServices;

namespace uGCapture
{
public class AptinaController : ReceiverController
{
    private ulong size;

    private byte[] dest;

    private Mutex runningMutex;

    private bool running=false;

    ManagedSimpleCapture msc;

    private static Semaphore barrierSemaphore;
    private static int barrierCounter;
    private readonly int numcams;
    private int nextIdx;

    public AptinaController(BufferPool<byte> bp, int numcams) 
        : base(bp)
    { 
        if (barrierSemaphore != null)
        {
            barrierSemaphore = new Semaphore(0, numcams);
        }

        if (runningMutex != null)
        {
            runningMutex = new Mutex();
        }

        barrierCounter = 0;
        nextIdx = 0;
        this.numcams = numcams;
    }

    public override void init()
    {
        ManagedSimpleCapture.managed_InitMidLib(numcams);
        size = msc.managed_SensorBufferSize();
        dest = new byte[size];
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
        me.runningMutex.WaitOne();
        if (me.running)
        {
            me.runningMutex.ReleaseMutex();
            return;
        }
        me.runningMutex.ReleaseMutex();

        while (true)
        {
            me.runningMutex.WaitOne();
            if (!me.running)
            {
                me.runningMutex.ReleaseMutex();
                barrierSemaphore.Release(1);
                return;
            }
            me.runningMutex.ReleaseMutex();

            Interlocked.Increment(ref barrierCounter);
            if (barrierCounter == me.numcams)
            {
                barrierSemaphore.Release(2); 
            }
            barrierSemaphore.WaitOne();

            unsafe
            {
                byte *data = me.msc.managed_DoCapture();
                if (data == null)
                {
                    //me.dp.BroadcastLog(me, "DoCapture returned a null pointer.", 100);
                    
                    continue;
                }
                Marshal.Copy(new IntPtr(data), me.dest, 0, (int) me.size);
            }
            
            //Buffer<Byte> imagebuffer = null;
            //while(imagebuffer==null)
            //    imagebuffer=StagingBuffer.PopEmpty();

            //BufferType bufferType = me.msc.managed_GetWavelength() == 405 ? 
            //    BufferType.IMAGE405 : BufferType.IMAGE485;

            //imagebuffer.setData(me.dest, bufferType);
            //File.WriteAllBytes(String.Format("data_{0}_{1}.raw",me.msc.managed_GetWavelength(), me.nextIdx++), me.dest);
            //imagebuffer.Text = String.Format("{0}", me.nextIdx++);
            //imagebuffer.CapacityUtilization = me.size;
            //StagingBuffer.PostFull(imagebuffer);
            
        }   
    }

    public override void DoFrame(object source, System.Timers.ElapsedEventArgs e)
    {
        throw new Exception("The method or operation is not implemented.");
    }
}
}
