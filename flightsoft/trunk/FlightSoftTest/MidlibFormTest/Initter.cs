using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uGCapture;

namespace MidlibFormTest
{
    class Initter : Receiver
    {
        
        private AptinaController ac1;
        private AptinaController ac2;
        private Writer writer;
        private Thread acThread1;
        private Thread acThread2;
        private Thread writerThread;
        private BufferPool<byte> bp;

        private CaptureClass cc;

        public IntPtr Hwnd
        {
            get { return hwnd; }
            set { hwnd = value; }
        }
        private IntPtr hwnd;

        public delegate void cb_log_message(string message);
        cb_log_message log_callback;

        public Initter() : base("Initter")
        {
            cc = new CaptureClass(hwnd, "CaptureClass");
            cc.StorageDir = @"C:\TestDir\";
        }

        public void set_callback(cb_log_message callback)
        {
            log_callback = callback;
        }

        public void init()
        {
            Dispatch.Instance().Register(this);

            //const int pool_size_bytes = 16777216;
            //bp = new BufferPool<byte>(10, pool_size_bytes);
            
            cc.init();
            
            //initWriter();
            //initAptina();
        }

        public void startThread()
        {
            acThread1.Start();
            acThread2.Start();

            if (acThread1.IsAlive)
            {
                dp.BroadcastLog(this, "Aptina Thread 1 started.", 5);
            }
            else
            {
                dp.BroadcastLog(this, "Aptina Thread 1 failed to start.", 5);
            }

            if (acThread2.IsAlive)
            {
                dp.BroadcastLog(this, "Aptina Thread 2 started.", 5);
            }
            else
            {
                dp.BroadcastLog(this, "Aptina Thread 2 failed to start.", 5);
            }
        }

        private void initWriter()
        {
            writer = new Writer(bp, Str.GetIdStr(IdStr.ID_WRITER)) { BasePath = @"C:\TestData\" };
            if (writer.Initialize())
            {
                writerThread = new Thread(() => Writer.WriteData(writer));
                writerThread.Start();
                if (writerThread.IsAlive)
                {
                    Dispatch.Instance().BroadcastLog(this, "Writer thread started.", 5);
                }
                dp.Register(writer);
            }
            else
            {
                string s = Str.GetErrStr(ErrStr.INIT_FAIL_WRITER);
                dp.BroadcastLog(this, s, 100);
                Console.WriteLine(s);
            }
        }

        private void initAptina()
        {
            ac1 = new AptinaController(bp, Str.GetIdStr(IdStr.ID_APTINA_ONE))
            {
                Hwnd = hwnd
            };
            if (ac1.Initialize())
            {
                acThread1 = new Thread(() => AptinaController.go(ac1));
            }
            else
            {
                dp.BroadcastLog(this,
                    Str.GetErrStr(ErrStr.INIT_FAIL_APTINA) + ": Camera 1.", 100);
            }
            dp.Register(ac1);


            ac2 = new AptinaController(bp, Str.GetIdStr(IdStr.ID_APTINA_TWO))
            {
                Hwnd = hwnd
            };
            if (ac2.Initialize())
            {
                acThread2 = new Thread(() => AptinaController.go(ac2));
            }
            else
            {
                dp.BroadcastLog(this,
                     Str.GetErrStr(ErrStr.INIT_FAIL_APTINA) + ": Camera 2.", 100);
            }
            dp.Register(ac2);


        }

        public override void exLogMessage(Receiver r, Message m)
        {
            LogMessage mes = m as LogMessage;
            if (mes == null) return;
            
            log_callback(mes.ToString());
        }

    }
}
