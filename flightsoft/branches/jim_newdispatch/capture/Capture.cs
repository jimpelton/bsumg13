// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-04-13                                                                      
// ******************************************************************************

using System;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace uGCapture 
{
    public class CaptureClass : Receiver
    {
        private const string ID_WRITER = "Writer";
        private const string ID_APTINA_ONE = "Aptina_One";
        private const string ID_APTINA_TWO = "Aptina_Two";
        private const string ID_PHIDGETS_DAQ = "Phidgets";
        private const string ID_PHIDGETS_ACCEL = "Phidgets_Accel";
        private const string ID_PHIDGETS_SPATIAL = "Phidgets_Spatial";
        private const string ID_WEATHERBOARD = "Weatherboard";
        private const string ID_NI_DAQ = "NI6008";

        private String directoryName;
        private bool boolCapturing = false;
        
        private BufferPool<byte> m_bufferPool;

        private Timer m_timer;
        private PhidgetsController phidgetsController;
        private AccelerometerPhidgetsController accelControler; 
        private SpatialAccelController spatialController;
        private Writer writer;
        private AptinaController ac1;
        private AptinaController ac2;
        private VCommController weatherboard;
        private NIController ni6008;

        private Thread acThread1;
        private Thread acThread2;
        private Thread wrtThread;
        private const double frame_time = 500;

        public CaptureClass(string id) : base(id)
        {
            //TODO: move datetime and directory creation into GUI.
            directoryName = DateTime.Now.ToString("yyyy_MM_dd_HHmm");
            System.IO.Directory.CreateDirectory("C:\\Data\\"+directoryName);
        }

        public void init()
        {
            m_timer = new Timer(frame_time);
            m_timer.Elapsed += DoFrame;

            m_bufferPool = new BufferPool<byte>(10,(int)Math.Pow(2,24));

            writer = new Writer(m_bufferPool, ID_WRITER) {DirectoryName = directoryName};
            writer.init();
            
            initAptina();
            initPhidgets();
            initAccelController();
            initSpatialController();
            initWeatherBoard();
            initNI6008Controller();

            m_timer.Enabled = false;
            dp.Register(this);
        }

        public void DoFrame(object source, ElapsedEventArgs e)
        {
        }

        private void initAptina()
        {
            try
            {
                ac1 = new AptinaController(m_bufferPool, ID_APTINA_ONE);
                ac1.init();
                acThread1 = new Thread(() => AptinaController.go(ac1));
                dp.Register(ac1);
            }
            catch (AptinaControllerNotInitializedException eek)
            {
                dp.BroadcastLog(this, eek.Message, 100);
                Console.WriteLine(eek.StackTrace);
            }

            try
            {
                ac2 = new AptinaController(m_bufferPool, ID_APTINA_TWO);
                ac2.init();
                acThread2 = new Thread(() => AptinaController.go(ac2));
                dp.Register(ac2);
            }
            catch (AptinaControllerNotInitializedException eek)
            {
                dp.BroadcastLog(this, eek.Message, 100);
                Console.WriteLine(eek.StackTrace);
            }
        }

        private void initPhidgets()
        {
            try
            {
                phidgetsController = new PhidgetsController(m_bufferPool, ID_PHIDGETS_DAQ);
                phidgetsController.init();
            }
            catch (PhidgetsControllerNotInitializedException eek)
            {
                dp.BroadcastLog(this, eek.Message, 100);
                Console.WriteLine(eek.StackTrace);
            }
        }


        private void initAccelController()
        {
            try
            {
                accelControler = new AccelerometerPhidgetsController(m_bufferPool, ID_PHIDGETS_ACCEL, 159352);
                accelControler.init();
            }
            catch (AccelerometerControllerNotInitializedException eek)
            {
                dp.BroadcastLog(this, eek.Message, 100);
                Console.WriteLine(eek.StackTrace);
            }           
        }

        private void initSpatialController()
        {
            try
            {
                spatialController = new SpatialAccelController(m_bufferPool, ID_PHIDGETS_SPATIAL, 169140);
                spatialController.init();
            }
            catch (SpatialControllerNotInitializedException eek)
            {
                dp.BroadcastLog(this, eek.Message, 100);
                Console.WriteLine(eek.StackTrace);
            }
        }

        private void initWeatherBoard()
        {
            try
            {
                weatherboard = new VCommController(m_bufferPool, ID_WEATHERBOARD);
                weatherboard.init();
            }
            catch (VCommControllerNotInitializedException eek)
            {
                dp.BroadcastLog(this, eek.Message, 100);
                Console.WriteLine(eek.StackTrace);               
            }           
        }

        private void initNI6008Controller()
        {
            try
            {
                ni6008 = new NIController(m_bufferPool, ID_NI_DAQ);
                ni6008.init();
            }
            catch (NIControllerNotInitializedException eek)
            {
                dp.BroadcastLog(this, eek.Message, 100);
                Console.WriteLine(eek.StackTrace);
            }
        }

        public override void exSetCaptureStateMessage(Receiver r, Message m)
        {
            SetCaptureStateMessage lm = m as SetCaptureStateMessage;
            if (lm == null) return;
            boolCapturing = lm.running;

            if (boolCapturing)
            {
                acThread1 = new Thread(() => AptinaController.go(ac1));
                acThread2 = new Thread(() => AptinaController.go(ac2));
                wrtThread = new Thread(() => Writer.WriteData(writer));
                acThread1.Start();
                acThread2.Start();
                wrtThread.Start();

                phidgetsController.TickerEnabled = true;
                accelControler.TickerEnabled = true;
                spatialController.TickerEnabled = true;
                writer.TickerEnabled = true;
                weatherboard.TickerEnabled = true;
                // ni6008.TickerEnabled = true;
            }
            else
            {
                ac1.stop();
                ac2.stop();
                //wrtThread.Start();

                phidgetsController.TickerEnabled = false;
                accelControler.TickerEnabled = false;
                spatialController.TickerEnabled = false;
                writer.TickerEnabled = false;
                weatherboard.TickerEnabled = false;
                ni6008.TickerEnabled = false;
            }
        }
    }
}
