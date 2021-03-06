﻿// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-04-13                                                                      
// ******************************************************************************

using System;
using System.Threading;
using System.Timers;
using System.IO;
using System.Collections.Generic;

namespace uGCapture
{
    public class CaptureClass : Receiver
    {
        /// <summary>
        /// The top level storage directory which captured data will be written to.
        /// </summary>
        public string StorageDir
        {
            get { return m_storageDir; }
            set
            {
                m_storageDir = value;
                m_storageDir = m_storageDir.Trim();
                if (!m_storageDir.EndsWith(@"\"))
                {
                    m_storageDir += @"\";
                }
            }
        }
        private string m_storageDir;

        /// <summary>
        /// Time in UTC since the capture system was initialized.
        /// </summary>
        public static DateTime StartTimeUTC
        {
            get { return m_startTimeUTC; }
        }
        private static DateTime m_startTimeUTC;
        
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
        private IntPtr hwnd;

        public CaptureClass(string id) : base(id, true) {}

        public CaptureClass(IntPtr hwnd, string id)
            : base(id)
        {
            this.hwnd = hwnd;
        }

        public void init()
        {
            m_startTimeUTC = DateTime.UtcNow;
            dp.Register(this);
                                                                               // 10M, 4K
            Staging<byte> sBuf = new Staging<byte>(2 * 2592 * 1944, 4096);     // image buffer size, utf8 buffer size
            bufferPool = new BufferPool<byte>(20, (int)Math.Pow(2,24), sBuf);
            
            initLogger();   
            initWriter();
            initAptina();
            initPhidgets();
            initAccelController();
            initSpatialController();
            initWeatherBoard();
            initNI6008Controller();
            initUPSController();

            Dispatch.Scheduler.Start();
        }

        private void initWriter()
        {
            writer = new Writer(bufferPool, Str.GetIdStr(IdStr.ID_WRITER)) { BasePath = m_storageDir };

            if (writer.Initialize())
            {
                wrtThread = new Thread(() => Writer.WriteData(writer));
                wrtThread.Start();
            }
            else
            {
                string s = Str.GetErrStr(ErrStr.INIT_FAIL_WRITER);
                dp.BroadcastLog(this, s, 100);
                Console.WriteLine(s);
            }
            dp.Register(writer);
        }

        //Aptina cameras.
        private void initAptina()
        {
            ac1 = new AptinaController(bufferPool, Str.GetIdStr(IdStr.ID_APTINA_ONE))
                {
                    Hwnd = hwnd
                };
            if (ac1.Initialize())
            {
                acThread1 = new Thread(() => AptinaController.go(ac1));
                dp.Broadcast(new AptinaStatusMessage(this, ac1.Status_Good));
            }
            else
            {
                dp.Broadcast(new AptinaStatusMessage(this, ac1.Status_Err, ac1.Errno));
                dp.BroadcastLog(this,
                    Str.GetErrStr(ErrStr.INIT_FAIL_APTINA) + ": Camera 1.", 100);
            }
            dp.Register(ac1);

            ac2 = new AptinaController(bufferPool, Str.GetIdStr(IdStr.ID_APTINA_TWO))
                {
                    Hwnd = hwnd
                };
            if (ac2.Initialize())
            {
                acThread2 = new Thread(() => AptinaController.go(ac2));
                dp.Broadcast(new AptinaStatusMessage(this, ac2.Status_Good, ac2.Errno));
            }
            else
            {
                dp.BroadcastLog(this,
                     Str.GetErrStr(ErrStr.INIT_FAIL_APTINA) + ": Camera 2.", 100);
            }
            dp.Register(ac2);

            acThread1.Start();
            acThread2.Start();


        }

        // 1018 DAQ and Temperature
        private void initPhidgets()
        {
            phidgetsController = new PhidgetsController(bufferPool, Str.GetIdStr(IdStr.ID_PHIDGETS_1018));

            if (phidgetsController.Initialize())
            {
                dp.Broadcast(new PhidgetsStatusMessage(this, Status.STAT_GOOD, ErrStr.INIT_OK_PHID_1018));
                Console.Error.WriteLine(Str.GetErrStr(ErrStr.INIT_OK_PHID_1018));
            }
            else
            {
                dp.Broadcast(new PhidgetsStatusMessage(this, Status.STAT_ERR, ErrStr.INIT_FAIL_PHID_1018));
                Console.Error.WriteLine(Str.GetErrStr(ErrStr.INIT_FAIL_PHID_1018));
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
                dp.Broadcast(new AccelStatusMessage(this, Status.STAT_GOOD, ErrStr.INIT_OK_PHID_ACCEL));
                Console.Error.WriteLine(Str.GetErrStr(ErrStr.INIT_OK_PHID_ACCEL));
            }
            else
            {
                dp.Broadcast(new AccelStatusMessage(this, Status.STAT_FAIL, ErrStr.INIT_FAIL_PHID_ACCEL));
                Console.Error.WriteLine(Str.GetErrStr(ErrStr.INIT_FAIL_PHID_ACCEL));
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
                dp.Broadcast(new SpatialStatusMessage(this, Status.STAT_GOOD, ErrStr.INIT_OK_PHID_SPTL));
                Console.Error.WriteLine(Str.GetErrStr(ErrStr.INIT_OK_PHID_SPTL));
            }
            else
            {
                dp.Broadcast(new SpatialStatusMessage(this, Status.STAT_FAIL, ErrStr.INIT_FAIL_PHID_SPTL));
                Console.Error.WriteLine(Str.GetErrStr(ErrStr.INIT_FAIL_PHID_SPTL));
            }
            dp.Register(spatialController);

        }

        // Sparkfun weatherboard through virtual com port.
        private void initWeatherBoard()
        {
            weatherboard = new VCommController(bufferPool, Str.GetIdStr(IdStr.ID_VCOMM));

            if (weatherboard.Initialize())
            {
                string s = Str.GetErrStr(ErrStr.INIT_OK_VCOMM);
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
            ni6008 = new NIController(bufferPool, Str.GetIdStr(IdStr.ID_NI_DAQ));

            if (ni6008.Initialize())
            {
                string s = Str.GetErrStr(ErrStr.INIT_OK_NI_6008);
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
            UPS = new UPSController(bufferPool, Str.GetIdStr(IdStr.ID_UPS));

            if (UPS.Initialize())
            {
                string s = Str.GetErrStr(ErrStr.INIT_OK_UPS);
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
            dp.Register(logger);
	    }
	
        public DataSet<byte> GetLastData()
        {
            return bufferPool.Staging.GetLastData();   
        }

        public void switchToBackupDrive(string newPath)
        {
            wrtThread.Suspend();//depricated. Find alternative.
            writer.BasePath = newPath;
            wrtThread.Resume();//depricated.
        }

        public override void exSetCaptureStateMessage(Receiver r, Message m)
        {
            SetCaptureStateMessage lm = m as SetCaptureStateMessage;
            if (lm == null) return;
            boolCapturing = lm.running;
        }

        public override void exReceiverCleanUpMessage(Receiver r, Message m)
        {
            base.exReceiverCleanUpMessage(r, m);
            
        }

        public void Shutdown()
        {
            ac1.stop();
            ac2.stop();
            writer.stop();
            Dispatch.Instance().CleanUpMessageThreads();
        }
    }
}
