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
        public string param_directoryName
        {
            set {directoryName = value;}
            get {return directoryName;}
        }
        private string directoryName;
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
        private const double FRAME_TIME = 500;

        public CaptureClass(string id) : base(id)
        {

            //messages = new Queue<Message>();

        }

        public void init()
        {
            m_timer = new Timer(FRAME_TIME);
            m_timer.Elapsed += DoFrame;

            m_bufferPool = new BufferPool<byte>(10,(int)Math.Pow(2,24));

            writer = new Writer(m_bufferPool, Str.GetIdStr(IdStr.ID_WRITER)) { DirectoryName = directoryName };
            writer.Initialize();
            wrtThread = new Thread(() => Writer.WriteData(writer));

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
            ac1 = new AptinaController(m_bufferPool, Str.GetIdStr(IdStr.ID_APTINA_ONE));
            if (ac1.Initialize())
            {
                acThread1 = new Thread(() => AptinaController.go(ac1));
                dp.Register(ac1);
            }
            else
            {
                dp.BroadcastLog(this,
                                Str.GetErrStr(ErrStr.INIT_FAIL_APTINA) + ": Camera 1.",
                                100);
            }

            ac2 = new AptinaController(m_bufferPool, Str.GetIdStr(IdStr.ID_APTINA_TWO));
            if (ac2.Initialize())
            {
                acThread2 = new Thread(() => AptinaController.go(ac2));
                dp.Register(ac2);
            }
            else
            {
                dp.BroadcastLog(this,
                                Str.GetErrStr(ErrStr.INIT_FAIL_APTINA) + ": Camera 2.",
                                100);
            }
        }

        private void initPhidgets()
        {
            phidgetsController = new PhidgetsController(m_bufferPool, Str.GetIdStr(IdStr.ID_PHIDGETS_DAQ));
            if (phidgetsController.Initialize())
            {
                string s = Str.GetMsgStr(MsgStr.INIT_OK_PHID_1018);
                dp.BroadcastLog(this,s,100);
                Console.Error.WriteLine(s);
            }
            else
            {
                string s = Str.GetErrStr(ErrStr.INIT_FAIL_PHID_1018);
                dp.BroadcastLog(this, s, 100);
                Console.Error.WriteLine(s);
            }

        }


        private void initAccelController()
        {
            accelControler = new AccelerometerPhidgetsController(m_bufferPool, 
                Str.GetIdStr(IdStr.ID_PHIDGETS_ACCEL), 159352);
            if (accelControler.Initialize())
            {
                string s = Str.GetMsgStr(MsgStr.INIT_OK_PHID_ACCEL);
                dp.BroadcastLog(this,s,100);
                Console.Error.WriteLine(s);
            }
            else
            {
                string s = Str.GetErrStr(ErrStr.INIT_FAIL_PHID_ACCEL);
                dp.BroadcastLog(this, s, 100);
                Console.Error.WriteLine(s);
            }
        }

        private void initSpatialController()
        {
            spatialController = new SpatialAccelController(m_bufferPool, 
                Str.GetIdStr(IdStr.ID_PHIDGETS_SPATIAL), 169140);

            if (spatialController.Initialize())
            {
                string s = Str.GetMsgStr(MsgStr.INIT_OK_PHID_SPTL);
                dp.BroadcastLog(this, s, 100);
                Console.Error.WriteLine(s);
            }
            else
            {
                string s = Str.GetErrStr(ErrStr.INIT_FAIL_PHID_SPTL);
                dp.BroadcastLog(this, s, 100);
                Console.Error.WriteLine(s);
            }
        }

        private void initWeatherBoard()
        {
            weatherboard = new VCommController(m_bufferPool, 
                Str.GetIdStr(IdStr.ID_VCOMM));

            if (weatherboard.Initialize())
            {
                string s = Str.GetMsgStr(MsgStr.INIT_OK_VCOMM);
                dp.BroadcastLog(this, s, 100);
                Console.Error.WriteLine(s);
            }
            else
            {
                string s = Str.GetErrStr(ErrStr.INIT_FAIL_VCOMM);
                dp.BroadcastLog(this, s, 100);
                Console.Error.WriteLine(s);
            }
        }

        private void initNI6008Controller()
        {
            ni6008 = new NIController(m_bufferPool, 
                Str.GetIdStr(IdStr.ID_NI_DAQ));

            if (ni6008.Initialize())
            {
                string s = Str.GetMsgStr(MsgStr.INIT_OK_NI_6008);
                dp.BroadcastLog(this, s, 100);
                Console.Error.WriteLine(s);
            }
            else
            {
                string s = Str.GetErrStr(ErrStr.INIT_FAIL_NI_6008);
                dp.BroadcastLog(this, s, 100);
                Console.Error.WriteLine(s);
            }
        }

        public override void exSetCaptureStateMessage(Receiver r, Message m)
        {
            SetCaptureStateMessage lm = m as SetCaptureStateMessage;
            if (lm == null) return;
            boolCapturing = lm.running;

            if (boolCapturing)
            {
                //acThread1.Start();
                //acThread2.Start();
                //wrtThread.Start();

                //phidgetsController.TickerEnabled = true;
                //accelControler.TickerEnabled = true;
                //spatialController.TickerEnabled = true;
                //writer.TickerEnabled = true;
                //weatherboard.TickerEnabled = true;
                // ni6008.TickerEnabled = true;
            }
            else
            {
                //ac1.stop();
                //ac2.stop();

                //phidgetsController.TickerEnabled = false;
                //accelControler.TickerEnabled = false;
                //spatialController.TickerEnabled = false;
                //writer.TickerEnabled = false;
                //weatherboard.TickerEnabled = false;
                //ni6008.TickerEnabled = false;
            }
        }
    }
}
