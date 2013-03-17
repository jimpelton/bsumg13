using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;

namespace uGCapture
{
    class AptinaController : Receiver, IController
    {
        private ulong size;
        private byte[] dest;
        private static bool running=false;
        private int nextIdx=0;
        Mutex mutex;
        ManagedSimpleCapture msc;
        public static Semaphore barrierSemaphore;
        public static int barrierCounter;
        public AptinaController() 
        {
            barrierSemaphore = new Semaphore(0,2);
            barrierCounter = 0;
            Int32 initResult = ManagedSimpleCapture.managed_InitMidLib(2);
            
            dp.BroadcastLog(this, "initResult (success=0): " + initResult,1);

            ManagedSimpleCapture mansc1 = new ManagedSimpleCapture();
            ManagedSimpleCapture mansc2 = new ManagedSimpleCapture();
            
            mansc1.managed_OpenTransport(1);
            mansc2.managed_OpenTransport(0);

            AptinaController mysc1 = new AptinaController(mansc1);
            AptinaController mysc2 = new AptinaController(mansc2);

            Thread t1 = new Thread(() => AptinaController.go(mysc1));
            Thread t2 = new Thread(() => AptinaController.go(mysc2));
            running = true;
            t1.Start();
            t2.Start();


            //wait for user to press a key, then stop.
            
            //mysc.stop();
            //t.Join();

            dp.BroadcastLog(this, "Done!",1);
                  
        }


        public AptinaController(ManagedSimpleCapture msc)
        {
            dp.BroadcastLog(this, "AptinaController starting up...", 1);
            size = msc.managed_SensorBufferSize();
            dest = new byte[size];
            mutex = new Mutex();

            this.msc = msc;
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
            if (!running)
            {
                running = true;
                me.mutex.ReleaseMutex();
                return;
            }
           
            while (true)
            {
                Interlocked.Increment(ref barrierCounter);

                if (barrierCounter > 1)
                {
                    Interlocked.Decrement(ref barrierCounter);
                    Interlocked.Decrement(ref barrierCounter);
                    barrierSemaphore.Release(2);//if we have two ready to go then release them both.
                }
                barrierSemaphore.WaitOne();

                if (!running)
                {
                    //me.mutex.ReleaseMutex();
                    barrierCounter = 65536;
                    barrierSemaphore.Release(1);
                    return;
                }

                me.dp.BroadcastLog(me, " " + me.nextIdx + " at " + DateTime.Now.Millisecond, 1);

                unsafe 
                {
                    byte* data = me.msc.managed_DoCapture();
                    if (data == null) 
                    {
                        //Console.Error.WriteLine("DoCapture returned a null pointer.");
                        me.dp.BroadcastLog(me, "DoCapture returned a null pointer.", 100);
                        continue;
                    }
                    Marshal.Copy(new IntPtr(data), me.dest, 0, (int)me.size);
                    
                    //me.dp.BroadcastLog(me, "Wrote some datums at " + DateTime.Now.Millisecond,1);
                
               
                };
                File.WriteAllBytes(String.Format("data_{0}_{1}.raw",me.msc.managed_GetWavelength(), me.nextIdx++), me.dest);
            }
        }
    }
}
