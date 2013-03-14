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

        public AptinaController() { }


        public AptinaController(ManagedSimpleCapture msc)
        {
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
                if (running == true)
                {
                    me.mutex.ReleaseMutex();
                    return;
                }
                running = true;
            me.mutex.ReleaseMutex();

            while (true)
            {
                me.mutex.WaitOne();
                    if (!running)
                    {
                        me.mutex.ReleaseMutex();
                        return;
                    }
                me.mutex.ReleaseMutex();

                unsafe 
                {
                    byte* data = me.msc.managed_DoCapture();
                    if (data != null) { continue; }
                    Marshal.Copy(new IntPtr(data), me.dest, 0, (int)me.size);
                    Console.WriteLine("Wrote some datums.");
                };
                File.WriteAllBytes(
                    String.Format("data%d.raw", me.nextIdx++), me.dest);
            }
        }
    }
}
