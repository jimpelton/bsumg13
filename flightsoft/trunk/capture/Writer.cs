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

        private const string niDir = "NI6008\\";
        private const string phidDir = "Phidgets\\";
        private const string accelDir = "Accel\\";
        private const string spatDir = "Spatial\\";
        private const string baroDir = "Barometer\\";
        private const string cam405Dir = "Camera405\\";
        private const string cam485Dir = "Camera485\\";
        private const string upsDir = "UPS\\";
        private const string logDir = "Log\\";

        private const string niPrfx = "NI6008";
        private const string phidPrfx = "Phidgets";
        private const string accelPrfx = "Accel";
        private const string spatialPrfx = "Spatial";
        private const string baroPrfx = "Barometer";
        private const string cam405Prfx = "Camera405";
        private const string cam485Prfx = "Camera485";
        private const string upsPrfx = "UPS";
        private const string logPrfx = "Log";

        public string DirectoryName
        {
            get { return m_directoryName; }
            set
            {
                m_directoryName = value.Trim();
                if (!m_directoryName.EndsWith(@"\"))
                {
                    m_directoryName += @"\";
                }
            }
        }
        private string m_directoryName;


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
            try
            {
                if (!Directory.Exists(DirectoryName))
                {
                    Directory.CreateDirectory(DirectoryName);
                }
            }
            catch (System.Exception e)
            {
                Console.Error.WriteLine(e.StackTrace);
                dp.BroadcastLog(this, "Top level data directory " + DirectoryName + " could not be created\r\n" +
                    e.StackTrace, 100);
                rval = false;
            }

            foreach (string s in Str.Dirs.Values)
            {
                try
                {
                    if (!Directory.Exists(DirectoryName + s))
                    {
                        Directory.CreateDirectory(DirectoryName + s);
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.StackTrace);
                    dp.BroadcastLog(this, "Data subdirectory " + DirectoryName + " could not be created\r\n" +
                                    e.StackTrace, 100);
                    rval = false;
                }
            }
            return rval;
        }

        /// <summary>
        /// Sets is running to false and cycles the writer loop 
        /// one last time if it is stuck waiting for new full buffers.
        /// </summary>
        public void stop()
        {
            IsRunning = false;
            //Buffer<byte> b = BufferPool.PopEmpty();
            Buffer<byte> b = new Buffer<byte>();
            b.Type = BufferType.EMPTY_CYCLE;
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

            Buffer<Byte> fulbuf;
            while (true)
            {
                if (!w.IsRunning)
                {
                    return;
                }

                try
                {
                    fulbuf = w.BufferPool.PopFull();
                    switch (fulbuf.Type)
                    {
                        case (BufferType.UTF8_UPS):
                            w.WriteOutput(fulbuf,
                                          Str.Dirs[DirStr.DIR_UPS] + upsPrfx,
                                          w.indexUPS,
                                          ".txt");
                            w.indexUPS += 1;
                            break;

                        case (BufferType.UTF8_VCOMM):
                            w.WriteOutput(fulbuf,
                                          Str.Dirs[DirStr.DIR_VCOMM] + baroPrfx,
                                          w.indexBarometer,
                                          ".txt");
                            w.indexBarometer += 1;
                            break;

                        case (BufferType.UTF8_PHIDGETS):
                            w.WriteOutput(fulbuf,
                                          Str.Dirs[DirStr.DIR_PHIDGETS] + phidPrfx,
                                          w.indexPhidgets,
                                          ".txt");
                            w.indexPhidgets += 1;
                            break;

                        case (BufferType.UTF8_ACCEL):
                            w.WriteOutput(fulbuf,
                                          Str.Dirs[DirStr.DIR_ACCEL] + accelPrfx,
                                          w.indexAccel,
                                          ".txt");
                            w.indexAccel += 1;
                            break;

                        case (BufferType.UTF8_SPATIAL):
                            w.WriteOutput(fulbuf,
                                          Str.Dirs[DirStr.DIR_SPATIAL] + spatialPrfx,
                                          w.indexSpatial,
                                          ".txt");
                            w.indexSpatial += 1;
                            break;

                        case (BufferType.UTF8_NI6008):
                            w.WriteOutput(fulbuf,
                                          Str.Dirs[DirStr.DIR_NI_DAQ] + niPrfx,
                                          w.indexNI6008,
                                          ".txt");
                            w.indexNI6008 += 1;
                            break;

                        case (BufferType.USHORT_IMAGE405):
                            w.WriteImageOutput(fulbuf,
                                               Str.Dirs[DirStr.DIR_CAMERA405] + cam405Prfx,
                                               w.index405,
                                               ".raw");
                            w.index405 += 1;
                            break;

                        case (BufferType.USHORT_IMAGE485):
                            w.WriteImageOutput(fulbuf,
                                               Str.Dirs[DirStr.DIR_CAMERA485] + cam485Prfx,
                                               w.index485,
                                               ".raw");
                            w.index485 += 1;
                            break;

                        case (BufferType.UTF8_LOG):
                            w.WriteOutput(fulbuf,
                            Str.Dirs[DirStr.DIR_LOGGER] + logPrfx, w.indexLog, ".txt");
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
            } // while...
        }
        
        private void WriteOutput(Buffer<byte> buf, string fnamePfx,
            uint index, string fnameExt)
        {
            String filename = String.Format(
                "{0}{1}_{2,8:D8}{3}", DirectoryName, fnamePfx, index, fnameExt);

            FileStream fs = File.Create(filename, (int)buf.CapacityUtilization,
                                        FileOptions.None);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(buf.Data, 0, (int)buf.CapacityUtilization);
            bw.Close();
            fs.Close();
        }

        // similar to WriteOutput, but inserts the buffer's filltime into the filename.
        private void WriteImageOutput(Buffer<byte> buf, string fnamePfx,
            uint index, string fnameExt)
        {
            String filename = String.Format(
                "{0}{1}_{2,8:D8}_{3,18:D18}{4}", DirectoryName, fnamePfx, index, buf.FillTime, fnameExt);

            FileStream fs = File.Create(filename, (int)buf.CapacityUtilization,
                                        FileOptions.None);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(buf.Data, 0, (int)buf.CapacityUtilization);
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