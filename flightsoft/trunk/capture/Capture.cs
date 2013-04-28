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
        private string _storageDir;
        public string StorageDir
        {
            get { return _storageDir; }
            set { 
                _storageDir = value;
                _storageDir = _storageDir.Trim();
		if (! _storageDir.EndsWith(@"\"))
		{
		    _storageDir += @"\";
		}
            }
        }

        private bool boolCapturing = false;
        
        private BufferPool<byte> bufferPool;

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
        }

        public void init()
        {

            bufferPool = new BufferPool<byte>(10,(int)Math.Pow(2,24));

            initWriter();
            initAptina();
            initPhidgets();
            initAccelController();
            initSpatialController();
            initWeatherBoard();
            initNI6008Controller();

            //m_timer.Enabled = false;
            dp.Register(this);
        }

        public void DoFrame(object source, ElapsedEventArgs e) { }

	private void initWriter()
	{
        writer = new Writer(bufferPool, Str.GetIdStr(IdStr.ID_WRITER)) { DirectoryName = _storageDir };
	    if (writer.Initialize())
	    {
	        wrtThread = new Thread(() => Writer.WriteData(writer));
	        //	    writer.IsRunning = true;
	        wrtThread.Start();
	        dp.Register(writer);
	    }
	    else
	    {
	        string s = Str.GetErrStr(ErrStr.INIT_FAIL_WRITER);
	        dp.BroadcastLog(this, s , 100);
            Console.WriteLine(s);
	    }
	}
	
	//Aptina cameras.
        private void initAptina()
        {
            ac1 = new AptinaController(bufferPool, Str.GetIdStr(IdStr.ID_APTINA_ONE));
            if (ac1.Initialize())
            {
                acThread1 = new Thread(() => AptinaController.go(ac1));
                dp.Register(ac1);
                acThread1.Start();
            }
            else
            {
                dp.BroadcastLog(this, 
                    Str.GetErrStr(ErrStr.INIT_FAIL_APTINA) + ": Camera 1.", 100);
            }

            ac2 = new AptinaController(bufferPool, Str.GetIdStr(IdStr.ID_APTINA_TWO));
            if (ac2.Initialize())
            {
                acThread2 = new Thread(() => AptinaController.go(ac2));
                dp.Register(ac2);
                acThread2.Start();
            }
            else
            {
                dp.BroadcastLog(this,
                     Str.GetErrStr(ErrStr.INIT_FAIL_APTINA) + ": Camera 2.", 100);
            }

        }

	// 1018 DAQ and Temperature
        private void initPhidgets()
        {
            phidgetsController = new PhidgetsController(bufferPool, Str.GetIdStr(IdStr.ID_PHIDGETS_1018));
            if (phidgetsController.Initialize())
            {
                dp.Register(phidgetsController);
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

	// Phidgits Accellerometer.
        private void initAccelController()
        {
            accelControler = new AccelerometerPhidgetsController(bufferPool, 
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

	// Phidgits Spatial Accellerometer.
        private void initSpatialController()
        {
            spatialController = new SpatialAccelController(bufferPool, 
                Str.GetIdStr(IdStr.ID_PHIDGETS_SPATIAL), 169140);

            if (spatialController.Initialize())
            {
                string s = Str.GetMsgStr(MsgStr.INIT_OK_PHID_SPTL);
                dp.Register(spatialController);
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

	// Sparkfun weatherboard through virtual com port.
        private void initWeatherBoard()
        {
            weatherboard = new VCommController(bufferPool, 
                Str.GetIdStr(IdStr.ID_VCOMM));

            if (weatherboard.Initialize())
            {
                string s = Str.GetMsgStr(MsgStr.INIT_OK_VCOMM);
                dp.Register(weatherboard);
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

	// NI-6008 DAQ.
        private void initNI6008Controller()
        {
            ni6008 = new NIController(bufferPool, 
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
        }
    }
}
