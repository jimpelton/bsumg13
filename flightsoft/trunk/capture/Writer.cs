﻿// ******************************************************************************
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
        private uint indexbarometer = 0;
        private uint index1018 = 0;

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

        private object isRunningMutex = new object();

        public bool IsRunning
        {
            get
            {
                lock (isRunningMutex)
                {
                    return m_isRunning;
                }
            }

            set
            {
                lock (isRunningMutex)
                {
                    m_isRunning = value;
                }
            }
        }

        private bool m_isRunning = false;

        public Writer(BufferPool<byte> bp, string id, bool receiving = true,
                      int frame_time = 500)
            : base(bp, id, receiving, frame_time)
        {
        }

        protected override bool init()
        {
            return true;
        }

	/// <summary>
	/// Sets is running to false and cycles the writer loop 
	/// one last time if it is stuck waiting for new full buffers.
	/// </summary>
        public void stop()
        {
            IsRunning = false;
            Buffer<byte> b = BufferPool.PopEmpty();
            b.Type = BufferType.EMPTY_CYCLE;
            BufferPool.PostFull(b);
        }


        /**
     * Writes data to the disk.
     * returns false if an error occurs.
     */
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
                        case (BufferType.UTF8_VCOMM):
                            w.WriteOutput(fulbuf, "Barometer_",
                                          w.indexbarometer++, ".txt");
                            break;
                        case (BufferType.UTF8_PHIDGETS):
                            w.WriteOutput(fulbuf, "Phidgets_",
                                          Math.Max(w.index405, w.index485),
                                          ".txt");
                            break;
                        case (BufferType.UTF8_ACCEL):
                            w.WriteOutput(fulbuf, "Accel_",
                                          Math.Max(w.index405, w.index485), ".txt");
                            break;
                        case (BufferType.UTF8_SPATIAL):
                            w.WriteOutput(fulbuf, "Spatial_",
                                          Math.Max(w.index405, w.index485), ".txt");
                            break;
                        case (BufferType.UTF8_NI6008):
                            w.WriteOutput(fulbuf, "NI6008_",
                                          Math.Max(w.index405, w.index485), ".txt");
                            break;
                        case (BufferType.USHORT_IMAGE405):
                            w.WriteOutput(fulbuf, "Camera405_",
                                          w.index405++, ".raw");
                            break;
                        case (BufferType.USHORT_IMAGE485):
                            w.WriteOutput(fulbuf, "Camera485_",
                                          w.index485++, ".raw");
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

        private void WriteOutput(Buffer<byte> buf, String fnamePfx, uint index,
                                 string fnameExt)
        {
            String filename = String.Format("{0}{1}{2}{3}", DirectoryName, fnamePfx, index,
                                            fnameExt);
            FileStream fs = File.Create(filename, (int) buf.CapacityUtilization,
                                        FileOptions.None);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(buf.Data, 0, (int) buf.CapacityUtilization);
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