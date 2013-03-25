using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using Phidgets;
using Phidgets.Events;
using Timer = System.Timers.Timer;

namespace uGCapture 
{
    public class CaptureClass : ReceiverController
    {
        private static int FRAME_TIME = 500;
        private String directoryName;
        private bool boolCapturing = false;

        private Queue<Message> messages;
        private Dispatch dispatch;

        private PhidgetsController phidgetsController;
        private AccelerometerController a1; 
        private AccelerometerController a2;
        private Writer writer;
        private AptinaController ac1;
        private AptinaController ac2;
        private VCommController weatherboard;
        //private Timer ticker;

        private Thread acThread1;
        private Thread acThread2;

        public CaptureClass() : base()
        {
            DateTime time = DateTime.Now;              // Use current time
            directoryName = "yyyyMMMdddHHmm";          // Use this format  
            directoryName = time.ToString(directoryName);

            System.IO.Directory.CreateDirectory("C:\\Data\\"+directoryName);

            messages = new Queue<Message>();
            dispatch = Dispatch.Instance();

            //ticker = new Timer(FRAME_TIME);
            //ticker.Elapsed += new ElapsedEventHandler(DoFrame);
        }

        public override void init()
        {
            BufferPool = new BufferPool<byte>(10,(int)Math.Pow(2,24));
            writer = new Writer(BufferPool);       
            writer.DirectoryName = directoryName;
            writer.init();
            
            a1 = new AccelerometerController(BufferPool);
            a2 = new AccelerometerController(BufferPool);
            ac1 = new AptinaController(BufferPool);
            ac2 = new AptinaController(BufferPool);
            try
            {
                ac1.init();
                acThread1 = new Thread(() => AptinaController.go(ac1));
            }
            catch (AptinaControllerNotInitializedException eek)
            {
                dp.BroadcastLog(this, eek.Message, 100);
                Console.WriteLine(eek.StackTrace);
            }
            try
            {
                ac2.init();
                acThread2 = new Thread(() => AptinaController.go(ac2));
            }
            catch (AptinaControllerNotInitializedException eek)
            {
                dp.BroadcastLog(this, eek.Message, 100);
                Console.WriteLine(eek.StackTrace);
            }

            phidgetsController = new PhidgetsController(BufferPool);
            phidgetsController.init();

            try
            {
                weatherboard = new VCommController(BufferPool);
                weatherboard.init();
            }
            catch (VCommControllerNotInitializedException eek)
            {
                dp.BroadcastLog(this, eek.Message, 100);
                Console.WriteLine(eek.StackTrace);               
            }


            
            this.Receiving = true;
            dp.Register(this,"CaptureControl");

            this.TickerEnabled = true;
           
        }

        public override void DoFrame(object source, ElapsedEventArgs e)
        {
            //while(this.msgs.Count > 0)
            //    msgs.Dequeue().execute(this);
            ExecuteMessageQueue();

           //check to see if we are capturing and if we are then enable the timers on our shtuff.
            if (!boolCapturing)
            {
                phidgetsController.TickerEnabled = false;
                a1.TickerEnabled = false;
                a2.TickerEnabled = false;
                writer.TickerEnabled = false;
                weatherboard.TickerEnabled = false;
            }
            else
            {
                phidgetsController.TickerEnabled = true;
                a1.TickerEnabled = true;
                a2.TickerEnabled = true;
                writer.TickerEnabled = true;
                weatherboard.TickerEnabled = true;
            }
        }

        //public void LogDebugMessage(String s)
        //{
        //    LogDebugMessage(s, 0);
        //}

        //public void LogDebugMessage(String s, int severtity)
        //{
        //    dp.Broadcast
        //    (
        //        new LogMessage(this, s, severtity)
        //    );
        //}

        public override void exSetCaptureState(Receiver r, Message m)
        {
            SetCaptureStateMessage lm = m as SetCaptureStateMessage;
            if (lm != null)
            {
                boolCapturing = lm.running;

                if (boolCapturing)
                {                   
                    acThread1 = new Thread(() => AptinaController.go(ac1));
                    acThread2 = new Thread(() => AptinaController.go(ac2));
                    acThread1.Start();
                    acThread2.Start();
                }
                else
                {
                    ac1.stop();
                    ac2.stop();
                }
            }
        }

    }
}
