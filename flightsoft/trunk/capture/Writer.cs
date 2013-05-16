// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-04-13                                                                      
// ******************************************************************************

using System;
using System.IO;
using System.Timers;

namespace uGCapture
{
    public class Writer : ReceiverController
    {
        private uint index485 = 0;
        private uint index405 = 0;
        private uint indexNI6008 = 0;
        private uint indexPhidgets = 0;
        private uint indexAccel = 0;
        private uint indexSpatial = 0;
        private uint indexBarometer = 0;
        private uint indexUPS = 0;
        private uint indexLog = 0;

        private string m_niPath;
        private string m_accelPath;
        private string m_spatPath;
        private string m_baroPath;
        private string m_cam405Path;
        private string m_cam485Path;
        private string m_upsPath;
        private string m_loggerPath;
        private string m_phidPath;

        public string BasePath
        {
            get { return m_basePath; }
            set
            {
                m_basePath = value.Trim();
                if (!m_basePath.EndsWith(@"\"))
                {
                    m_basePath += @"\";
                }
            }
        }

        private string m_basePath;


        public bool IsRunning
        {
            get
            {
                lock (m_isRunningMutex)
                {
                    return m_isRunning;
                }
            }

            set
            {
                lock (m_isRunningMutex)
                {
                    m_isRunning = value;
                }
            }
        }

        private bool m_isRunning = false;
        private object m_isRunningMutex = new object();


        public Writer(BufferPool<byte> bp, string id, bool receiving = true,
                      int frame_time = 500)
            : base(bp, id, receiving, frame_time)
        {
        }

        protected override bool init()
        {
            bool rval = true;
            makePaths(BasePath);
            try
            {
                if (!Directory.Exists(BasePath))
                {
                    Directory.CreateDirectory(BasePath);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.StackTrace);
                dp.BroadcastLog(this,
                                "Top level data directory " + BasePath +
                                " could not be created\r\n" +
                    e.StackTrace, 100);
                rval = false;
            }

            foreach (string s in Str.Dirs.Values)
            {
                try
                {
                    if (!Directory.Exists(BasePath + s))
                    {
                        Directory.CreateDirectory(BasePath + s);
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.StackTrace);
                    dp.BroadcastLog(this,
                                    "Data subdirectory " + s +
                                    " could not be created\r\n" +
                                    e.StackTrace, 100);
                    rval = false;
                }
            }
            return rval;
        }


        private void makePaths(string basePath)
        {
            m_niPath =
                basePath + Str.Dirs[DirStr.DIR_NI_DAQ] + Str.Pfx[DirStr.DIR_NI_DAQ];

            m_phidPath =
                basePath + Str.Dirs[DirStr.DIR_PHIDGETS] + Str.Pfx[DirStr.DIR_PHIDGETS];

            m_accelPath =
                basePath + Str.Dirs[DirStr.DIR_ACCEL] + Str.Pfx[DirStr.DIR_ACCEL];

            m_spatPath =
                basePath + Str.Dirs[DirStr.DIR_SPATIAL] + Str.Pfx[DirStr.DIR_SPATIAL];

            m_baroPath =
                basePath + Str.Dirs[DirStr.DIR_VCOMM] + Str.Pfx[DirStr.DIR_VCOMM];

            m_cam405Path =
                basePath + Str.Dirs[DirStr.DIR_CAMERA405] + Str.Pfx[DirStr.DIR_CAMERA405];

            m_cam485Path =
                basePath + Str.Dirs[DirStr.DIR_CAMERA485] + Str.Pfx[DirStr.DIR_CAMERA485];

            m_upsPath =
                basePath + Str.Dirs[DirStr.DIR_UPS] + Str.Pfx[DirStr.DIR_UPS];

            m_loggerPath =
                basePath + Str.Dirs[DirStr.DIR_LOGGER] + Str.Pfx[DirStr.DIR_LOGGER];
        }

        /// <summary>
        /// Sets is running to false and cycles the writer loop 
        /// one last time if it is stuck waiting for new full buffers.
        /// </summary>
        public void stop()
        {
            IsRunning = false;
            //Buffer<byte> b = BufferPool.PopEmpty();
            Buffer<byte> b = new Buffer<byte> {Type = BufferType.EMPTY_CYCLE};
            BufferPool.PostFull(b);
        }

        /// <summary>
        /// Writes data to the disk.
        /// returns false if an error occurs.
        /// </summary>
        /// <param name="w"></param>
        public static void WriteData(Writer w)
        {
            if (w.IsRunning)
            {
                return;
            }
            w.IsRunning = true;

            while (true)
            {
                if (!w.IsRunning)
                {
                    return;
                }

                DoWrite(w);
            } // while...
        }

        /// <summary>
        /// Pops a single full buffer from the pool and writes it 
        /// to disk in the appropriate location.
        /// </summary>
        /// <param name="w">The writer which should do the writing.</param>
        public static void DoWrite(Writer w)
        {
                try
                {
                    Buffer<byte> fulbuf = w.BufferPool.PopFull();
                    switch (fulbuf.Type)
                    {
                        case (BufferType.UTF8_UPS):
                            w.WriteOutput(fulbuf, w.m_upsPath, w.indexUPS, ".txt");
                            w.indexUPS += 1;
                            break;

                        case (BufferType.UTF8_VCOMM):
                            w.WriteOutput(fulbuf, w.m_baroPath, w.indexBarometer, ".txt");
                            w.indexBarometer += 1;
                            break;

                        case (BufferType.UTF8_PHIDGETS):
                        w.WriteOutput(fulbuf, w.m_phidPath, w.indexPhidgets, ".txt");
                            w.indexPhidgets += 1;
                            break;

                        case (BufferType.UTF8_ACCEL):
                        w.WriteOutput(fulbuf, w.m_accelPath, w.indexAccel, ".txt");
                            w.indexAccel += 1;
                            break;

                        case (BufferType.UTF8_SPATIAL):
                        w.WriteOutput(fulbuf, w.m_spatPath, w.indexSpatial, ".txt");
                            w.indexSpatial += 1;
                            break;

                        case (BufferType.UTF8_NI6008):
                        w.WriteOutput(fulbuf, w.m_niPath, w.indexNI6008, ".txt");
                            w.indexNI6008 += 1;
                            break;

                        case (BufferType.USHORT_IMAGE405):
                        w.WriteImageOutput(fulbuf, w.m_cam405Path, w.index405, ".raw");
                            w.index405 += 1;
                            break;

                        case (BufferType.USHORT_IMAGE485):
                        w.WriteImageOutput(fulbuf, w.m_cam485Path, w.index485, ".raw");
                            w.index485 += 1;
                            break;

                        case (BufferType.UTF8_LOG):
                        w.WriteOutput(fulbuf, w.m_loggerPath, w.indexLog, ".txt");
                            w.indexLog += 1;
                            break;

                        case (BufferType.EMPTY_CYCLE):
                            break;

                        default:
                            break;
                    }
                    w.BufferPool.PostEmpty(fulbuf);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                    //return ;
                }
        }

        private void WriteOutput(Buffer<byte> buf, string fnamePfx,
            uint index, string fnameExt)
        {
            String filename = String.Format(
                "{0}_{1}{2}", fnamePfx, index, fnameExt);

            FileStream fs = File.Create(filename, buf.CapacityUtilization, FileOptions.None);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(buf.Data, 0, buf.CapacityUtilization);
            bw.Close();
            fs.Close();
        }

        // similar to WriteOutput, but inserts the buffer's filltime into the filename.
        private void WriteImageOutput(Buffer<byte> buf, string fnamePfx,
            uint index, string fnameExt)
        {
            String filename = String.Format(
                "{0}_{1,8:D8}_{2,18:D18}{3}", fnamePfx, index, buf.FillTime, fnameExt);
            FileStream fs = File.Create(filename, buf.CapacityUtilization, FileOptions.None);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(buf.Data, 0, buf.CapacityUtilization);
            bw.Close();
            fs.Close();
        }

        public override void exReceiverCleanUpMessage(Receiver r, Message m)
        {
            base.exReceiverCleanUpMessage(r, m);
            IsRunning = false;
        }


        public override void DoFrame(object source, ElapsedEventArgs e)
        {
        }
    }
}

//This is the last written data stored temp so that we can send it to anyone who sends a data request.
//private bool[] phidgetsDigitalInputs;
//private bool[] phidgetsDigitalOutputs;
//private int[] phidgetsAnalogInputs;
//private double phidgetTemperature_ProbeTemp;
//private double phidgetTemperature_AmbientTemp;
//private double[] accel1rawacceleration;
//private double[] accel1acceleration;
//private double[] accel1vibration;
//private double[] accel2rawacceleration;
//private double[] accel2acceleration;
//private double[] accel2vibration;
//private double[] NIanaloginputs;
//private double humidity = 0;
//private double temp1 = 0;
//private double temp2 = 0;
//private double temp3 = 0;
//private double pressure = 0;
//private double illumunation = 0;
//public int accel1state = 0;
//public int accel2state = 0;
//public int phidgets888state = 0;
//public int phidgetstempstate = 0;
//public int UPSstate = 0;
//public int VCommstate = 0;
//private Buffer<byte> image405 = null;
//private Buffer<byte> image485 = null;
//private long[] wellIntensities405;
//private long[] wellIntensities485;