﻿// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-04-13                                                                      
// ******************************************************************************

using System;
using System.Threading;
using System.Timers;

namespace uGCapture
{
    public class CaptureClass : Receiver
    {
        private string _storageDir;
        public string StorageDir
        {
            get { return _storageDir; }
            set
            {
                _storageDir = value;
                _storageDir = _storageDir.Trim();
                if (!_storageDir.EndsWith(@"\"))
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
        private UPSController UPS;
        private Logger logger;

        private Thread acThread1;
        private Thread acThread2;
        private Thread wrtThread;
        private const double FRAME_TIME = 500;

        public CaptureClass(string id)
            : base(id)
        {
        }

        public void init()
        {

            Staging<byte> sBuf = new Staging<byte>(2 * 2592 * 1944); // Three magic numbers !
            bufferPool = new BufferPool<byte>(10, (int)Math.Pow(2, 24), sBuf);

            initWriter();
            initAptina();
            initPhidgets();
            initAccelController();
            initSpatialController();
            initWeatherBoard();
            initNI6008Controller();
            initUPSController();
	    // initLogger();    // --JP 5/13/13

            dp.Register(this);
        }

        public void DoFrame(object source, ElapsedEventArgs e) { }

        private void initWriter()
        {
            writer = new Writer(bufferPool, Str.GetIdStr(IdStr.ID_WRITER)) { BasePath = _storageDir };
            if (writer.Initialize())
            {
                wrtThread = new Thread(() => Writer.WriteData(writer));
                wrtThread.Start();
                dp.Register(writer);
            }
            else
            {
                string s = Str.GetErrStr(ErrStr.INIT_FAIL_WRITER);
                dp.BroadcastLog(this, s, 100);
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
                string s = Str.GetMsgStr(MsgStr.INIT_OK_PHID_1018);
                dp.BroadcastLog(this, s, 100);
                Console.Error.WriteLine(s);

            }
            else
            {
                string s = Str.GetErrStr(ErrStr.INIT_FAIL_PHID_1018);
                dp.BroadcastLog(this, s, 100);
                Console.Error.WriteLine(s);
            }
            dp.Register(phidgetsController);
        }

        // Phidgits Accellerometer.
        private void initAccelController()
        {
            const int accelerometer_serial_number = 159352;
            accelControler = new AccelerometerPhidgetsController(bufferPool,
                Str.GetIdStr(IdStr.ID_PHIDGETS_ACCEL), accelerometer_serial_number);
            if (accelControler.Initialize())
            {
                string s = Str.GetMsgStr(MsgStr.INIT_OK_PHID_ACCEL);
                dp.BroadcastLog(this, s, 100);
                Console.Error.WriteLine(s);
            }
            else
            {
                string s = Str.GetErrStr(ErrStr.INIT_FAIL_PHID_ACCEL);
                dp.BroadcastLog(this, s, 100);
                Console.Error.WriteLine(s);
            }
            dp.Register(accelControler);
        }

        // Phidgits Spatial Accellerometer.
        private void initSpatialController()
        {
            const int spatial_serial_number = 169140;
            spatialController = new SpatialAccelController(bufferPool,
                Str.GetIdStr(IdStr.ID_PHIDGETS_SPATIAL), spatial_serial_number);

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
            dp.Register(spatialController);
        }

        // Sparkfun weatherboard through virtual com port.
        private void initWeatherBoard()
        {
            weatherboard = new VCommController(bufferPool,
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
            dp.Register(weatherboard);
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
            dp.Register(ni6008);
        }

        // APC UTF8_UPS Data Getter
        private void initUPSController()
        {
            UPS = new UPSController(bufferPool,
                Str.GetIdStr(IdStr.ID_UPS));

            if (UPS.Initialize())
            {
                string s = Str.GetMsgStr(MsgStr.INIT_OK_UPS);
                dp.BroadcastLog(this, s, 100);
                Console.Error.WriteLine(s);
            }
            else
            {
                string s = Str.GetErrStr(ErrStr.INIT_FAIL_UPS);
                dp.BroadcastLog(this, s, 100);
                Console.Error.WriteLine(s);
            }
            dp.Register(UPS);
        }
	
	    public void initLogger()
	    {
	        logger = new Logger(Str.GetIdStr(IdStr.ID_LOGGER));
	    }
	
        public DataSet<byte> GetLastData()
        {
            if(bufferPool.Staging!=null)
                return bufferPool.Staging.GetLastData();
            return null;
        } 

        public override void exSetCaptureStateMessage(Receiver r, Message m)
        {
            SetCaptureStateMessage lm = m as SetCaptureStateMessage;
            if (lm == null) return;
            boolCapturing = lm.running;
        }

        public override void exReceiverCleanUpMessage(Receiver r, Message m)
        {
            ac1.stop();
            ac2.stop();
            writer.stop();
            base.exReceiverCleanUpMessage(r, m);
        }
    }
}
