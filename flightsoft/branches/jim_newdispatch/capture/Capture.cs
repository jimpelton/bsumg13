using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using Phidgets;
using Phidgets.Events;
using Timer = System.Timers.Timer;

namespace uGCapture 
{
    public class CaptureClass : Receiver
    {
        private String directoryName;
        private bool boolCapturing = false;
        
        private BufferPool<byte> m_bufferPool;
        //private Queue<Message> messages;

        private Timer m_timer;
        private PhidgetsController phidgetsController;
        private AccelerometerController accelControler; 
        private SpatialController spatialController;
        private Writer writer;
        private AptinaController ac1;
        private AptinaController ac2;
        private VCommController weatherboard;
        private NIController ni6008;

        private Thread acThread1;
        private Thread acThread2;
        private Thread wrtThread;
        private const double frame_time = 500;

        public CaptureClass() 
        {
            //TODO: move datetime and directory creation into GUI.
            directoryName = DateTime.Now.ToString("yyyy_MM_dd_HHmm");
            System.IO.Directory.CreateDirectory("C:\\Data\\"+directoryName);

            //messages = new Queue<Message>();
        }

        public void init()
        {
            dp.Register(this,"CaptureControl");

            m_bufferPool = new BufferPool<byte>(10,(int)Math.Pow(2,24));

            m_timer = new Timer(frame_time);
            m_timer.Elapsed += DoFrame;

            writer = new Writer(m_bufferPool);       
            writer.DirectoryName = directoryName;
            writer.init();
            
            ac1 = new AptinaController(m_bufferPool);
            ac2 = new AptinaController(m_bufferPool);
            

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

            try
            {
                phidgetsController = new PhidgetsController(m_bufferPool);
                phidgetsController.init();
            }
            catch (PhidgetsControllerNotInitializedException eek)
            {
                dp.BroadcastLog(this, eek.Message, 100);
                Console.WriteLine(eek.StackTrace);
            }

            try
            {
                weatherboard = new VCommController(m_bufferPool);
                weatherboard.init();
            }
            catch (VCommControllerNotInitializedException eek)
            {
                dp.BroadcastLog(this, eek.Message, 100);
                Console.WriteLine(eek.StackTrace);               
            }
            
            try
            {
                ni6008 = new NIController(m_bufferPool);
                ni6008.init();
            }
            catch (NIControllerNotInitializedException eek)
            {
                dp.BroadcastLog(this, eek.Message, 100);
                Console.WriteLine(eek.StackTrace);
            }
            
            try
            {
                accelControler = new AccelerometerController(m_bufferPool, 159352);
                accelControler.init();
            }
            catch (AccelerometerControllerNotInitializedException eek)
            {
                dp.BroadcastLog(this, eek.Message, 100);
                Console.WriteLine(eek.StackTrace);
            }

            try
            {
                spatialController = new SpatialController(m_bufferPool, 169140);
                spatialController.init();
            }
            catch (SpatialControllerNotInitializedException eek)
            {
                dp.BroadcastLog(this, eek.Message, 100);
                Console.WriteLine(eek.StackTrace);
            }
 
            this.Receiving = true;
            m_timer.Enabled = true;
        }

        public void DoFrame(object source, ElapsedEventArgs e)
        {
            ExecuteMessageQueue();

           //check to see if we are capturing and if we are then enable the timers on our shtuff.
            if (!boolCapturing)
            {
                phidgetsController.TickerEnabled = false;
                accelControler.TickerEnabled = false;
                spatialController.TickerEnabled = false;
                writer.TickerEnabled = false;
                weatherboard.TickerEnabled = false;
                ni6008.TickerEnabled = false;
            }
            else
            {
                phidgetsController.TickerEnabled = true;
                accelControler.TickerEnabled = true;
                spatialController.TickerEnabled = true;
                writer.TickerEnabled = true;
                weatherboard.TickerEnabled = true;
               // ni6008.TickerEnabled = true;
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

        public override void exSetCaptureStateMessage(Receiver r, Message m)
        {
            SetCaptureStateMessage lm = m as SetCaptureStateMessage;
            if (lm != null)
            {
                boolCapturing = lm.running;

                if (boolCapturing)
                {                   
                    acThread1 = new Thread(() => AptinaController.go(ac1));
                    acThread2 = new Thread(() => AptinaController.go(ac2));
                    wrtThread = new Thread(() => Writer.WriteData(writer));
                    wrtThread.Start();
                    acThread1.Start();
                    acThread2.Start();
                }
                else
                {
                    ac1.stop();
                    ac2.stop();
                    //wrtThread.Start();
                }
            }
        }

    }
}
