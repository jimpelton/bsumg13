using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using uGCapture.Controller;

namespace uGCapture
{
public class AptinaController : ReceiverController
{
    private ulong size;
    private byte[] dest;

    private bool running=false;
    private Mutex mutex;

    private int nextIdx=0;

    ManagedSimpleCapture msc;

    private static Semaphore barrierSemaphore;
    private static int barrierCounter;
    private static int numcams;

    public AptinaController(int numcams) : base()
    { 
        if (barrierSemaphore != null)
        {
            barrierSemaphore = new Semaphore(0, numcams);
        }

        if (mutex != null)
        {
            mutex = new Mutex();
        }

        barrierCounter = 0;
        
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
        mutex.WaitOne();
        running = false;
        mutex.ReleaseMutex();
    }

    public static void go(AptinaController me)
    {
        me.mutex.WaitOne();
        if (me.running )
        {
            me.mutex.ReleaseMutex();
            return;
        }
        me.mutex.ReleaseMutex();

        while (true)
        {
            me.mutex.WaitOne();
            if (!me.running)
            {
                me.mutex.ReleaseMutex();
                barrierSemaphore.Release(1);
                return;
            }
            me.mutex.ReleaseMutex();

            Interlocked.Increment(ref barrierCounter);
            if (barrierCounter == numcams)
            {
                barrierSemaphore.Release(2); 
            }
            barrierSemaphore.WaitOne();

            me.dp.BroadcastLog(
                me, 
                String.Format(" {0} at {1}", me.nextIdx, DateTime.Now.Millisecond),
                1);

            unsafe
            {
                byte* data = me.msc.managed_DoCapture();
                if (data == null)
                {
                    //Console.Error.WriteLine("DoCapture returned a null pointer.");
                    me.dp.BroadcastLog(me, "DoCapture returned a null pointer.", 100);
                    continue;
                }
                Marshal.Copy(new IntPtr(data), me.dest, 0, (int) me.size);
            }
            
            //Buffer<Byte> imagebuffer = null;
            //while(imagebuffer==null)
            //    imagebuffer=StagingBuffer.PopEmpty();

            BufferType bufferType = me.msc.managed_GetWavelength() == 405 ? 
                BufferType.IMAGE405 : BufferType.IMAGE485;

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
