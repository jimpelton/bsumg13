// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-04-13                                                                      
// ******************************************************************************

using System;
using System.IO;
using System.Timers;
using Enc = System.Text.Encoding;

namespace uGCapture
{
    public class Writer : ReceiverController
    {
        private uint index485 = 0;
        private uint index405 = 0;

        public string DirectoryName
        {
            get
            {
                return m_directoryName;
            }
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
            //wellIntensities405 = new long[192];
            //wellIntensities485 = new long[192];
            //phidgetsDigitalInputs = new bool[8];
            //phidgetsDigitalOutputs = new bool[8];
            //phidgetsAnalogInputs = new int[8];
            //accel1rawacceleration = new double[3];
            //accel1acceleration = new double[3];
            //accel1vibration = new double[3];
            //accel2rawacceleration = new double[3];
            //accel2acceleration = new double[3];
            //accel2vibration = new double[3];
            //NIanaloginputs = new double[6];
        }

        protected override bool init()
        {
            return true;
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
                                          Math.Max(w.index405, w.index485), ".txt");
                            //w.WriteWeatherboardOutput(fulbuf, Math.Min(w.index405, w.index485));
                            break;
                        case (BufferType.UTF8_PHIDGETS):
                            //w.WritePhidgetsOutput(fulbuf, ", Math.Min(w.index405, w.index485));
                            w.WriteOutput(fulbuf, "Phidgets_", Math.Max(w.index405, w.index485),
                                          ".txt");
                            break;
                        case (BufferType.UTF8_ACCEL):
                            w.WriteOutput(fulbuf, "Accel_",
                                          Math.Max(w.index405, w.index485), ".txt");
                            //w.WriteAccelerometerOutput(fulbuf, Math.Min(w.index405, w.index485));
                            break;
                        case (BufferType.UTF8_SPATIAL):
                            w.WriteOutput(fulbuf, "Spatial_",
                                          Math.Max(w.index405, w.index485), ".txt");
                            //w.WriteAccelerometerOutput(fulbuf, Math.Min(w.index405, w.index485));
                            break;
                        case (BufferType.UTF8_NI6008):
                            w.WriteOutput(fulbuf, "NI6008_",
                                          Math.Max(w.index405, w.index485), ".txt");
                            //w.WriteNI6008Output(fulbuf, Math.Min(w.index405, w.index485));
                            break;
                        case (BufferType.USHORT_IMAGE405):
                            w.WriteOutput(fulbuf, "Camera405_",
                                          w.index405++, ".raw");
                            //w.WriteImageOutput(fulbuf, w.index405++);
                            break;
                        case (BufferType.USHORT_IMAGE485):
                            w.WriteOutput(fulbuf, "Camera485_",
                                          w.index485++, ".raw");
                            //w.WriteImageOutput(fulbuf, w.index485++);
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
            FileStream fs = File.Create(filename, (int)buf.CapacityUtilization,
                                        FileOptions.None);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(buf.Data, 0, (int)buf.CapacityUtilization);
            bw.Close();
            fs.Close();
        }

        public override void DoFrame(object source, ElapsedEventArgs e)
        {
            //WriteData();
        }
        //private void WriteImageOutput(Buffer<Byte> buf, uint index)
        //{
        //    String filename = String.Format("data_{0}.raw", index);
        //    FileStream fs = File.Create(DirectoryName + filename, (int)buf.CapacityUtilization, FileOptions.None);
        //    BinaryWriter bw = new BinaryWriter(fs);
        //    bw.Write(buf.Data, 0, (int)buf.CapacityUtilization);
        //    bw.Close();
        //    fs.Close();
        //    StoreImageData(buf,index);

        //}

        //private void WritePhidgetsOutput(Buffer<Byte> buf, uint index)
        //{
        //    String filename = String.Format("Phidgets{0}.txt", index);
        //    FileStream fs = File.Create(DirectoryName + filename, (int)buf.CapacityUtilization, FileOptions.None);
        //    BinaryWriter bw = new BinaryWriter(fs);
        //    bw.Write(buf.Data, 0, (int)buf.CapacityUtilization);
        //    bw.Close();
        //    fs.Close();
        //    StorePhidgetsData(buf,index);
        //}

        //private void WriteNI6008Output(Buffer<Byte> buf, uint index)
        //{
        //    String filename = String.Format("NI6008_{0}.txt", index);
        //    FileStream fs = File.Create(DirectoryName + filename, (int)buf.CapacityUtilization, FileOptions.None);
        //    BinaryWriter bw = new BinaryWriter(fs);
        //    bw.Write(buf.Data, 0, (int)buf.CapacityUtilization);
        //    bw.Close();
        //    fs.Close();
        //    StoreNI6008Data(buf,index);
        //}

        //private void WriteAccelerometerOutput(Buffer<Byte> buf, uint index)
        //{
        //    String filename = String.Format("Accel{0}_{1}.txt", buf.Text, index);//we can use the text to differentiate between different accelerometers.
        //    FileStream fs = File.Create(DirectoryName+filename, (int)buf.CapacityUtilization, FileOptions.None);
        //    BinaryWriter bw = new BinaryWriter(fs);
        //    bw.Write(buf.Data, 0, (int)buf.CapacityUtilization);
        //    bw.Close();
        //    fs.Close();
        //    StoreAccelerometerData(buf,index);
        //}

        //private void WriteWeatherboardOutput(Buffer<Byte> buf, uint index)
        //{
        //    String filename = String.Format("Barometer{0}.txt", index);
        //    FileStream fs = File.Create(DirectoryName+filename, (int)buf.CapacityUtilization, FileOptions.None);
        //    BinaryWriter bw = new BinaryWriter(fs);
        //    bw.Write(buf.Data, 0, (int)buf.CapacityUtilization);
        //    bw.Close();
        //    fs.Close();
        //    StoreWeatherboardData(buf,index);
        //}

        //private void StoreImageData(Buffer<Byte> buf, uint index)
        //{

        //    //this is a nightmare for the GC       
        //    Buffer<byte> buf2 = new Buffer<byte>(buf);
        //if (buf.Type == BufferType.USHORT_IMAGE405)
        //        image405 = buf2;
        //    else if (buf.Type == BufferType.USHORT_IMAGE485)
        //        image485 = buf2;
        //    else
        //        dp.BroadcastLog(this, "Writer passed an image with an invalid wavelength.", 5);

        //}

        //private void StorePhidgetsData(Buffer<Byte> buf, uint index)
        //{
        //    string datain = Enc.UTF8.GetString(buf.Data,0,(int) buf.CapacityUtilization);
        //    string[] data = datain.Split();
        //    phidgetTemperature_ProbeTemp = double.Parse(data[2]);
        //    phidgetTemperature_AmbientTemp = double.Parse(data[3]);
        //    for (int i = 0; i < 8; i++)
        //    {
        //        phidgetsAnalogInputs[i] = int.Parse(data[(i * 3) + 4]);
        //        phidgetsDigitalInputs[i] = bool.Parse(data[(i * 3) + 5]); ;
        //        phidgetsDigitalOutputs[i] = bool.Parse(data[(i * 3) + 6]); ;
        //    }
        //}

        //private void StoreNI6008Data(Buffer<Byte> buf, uint index)
        //{
        //    string datain = Enc.UTF8.GetString(buf.Data,0,(int) buf.CapacityUtilization);
        //    string[] data = datain.Split();
        //    for (int i = 0; i < 6; i++)
        //        NIanaloginputs[i] = double.Parse(data[i + 1]);
        //}

        //private void StoreAccelerometerData(Buffer<Byte> buf, uint index)
        //{
        //    string datain = Enc.UTF8.GetString(buf.Data,0,(int)buf.CapacityUtilization);
        //    string[] data = datain.Split();
        //    //decide if it is the spacial or the accelerometer.
        //    if (buf.Text == "TODO:whatever the serial number of one of them is...")
        //    {
        //        for (int i = 2; i < 5; i++)
        //            accel1rawacceleration[i - 2] = double.Parse(data[i]);
        //        for (int i = 5; i < 8; i++)
        //            accel1acceleration[i - 5] = double.Parse(data[i]);
        //        for (int i = 8; i < 11; i++)
        //            accel1vibration[i - 8] = double.Parse(data[i]);
        //    }
        //    else
        //    {
        //        for (int i = 2; i < 5; i++)
        //            accel2rawacceleration[i - 2] = double.Parse(data[i]);
        //        for (int i = 5; i < 8; i++)
        //            accel2acceleration[i - 5] = double.Parse(data[i]);
        //        for (int i = 8; i < 11; i++)
        //            accel2vibration[i - 8] = double.Parse(data[i]);
        //    }

        //}

        //private void StoreWeatherboardData(Buffer<Byte> buf, uint index)
        //{
        //    string datain = Enc.UTF8.GetString(buf.Data,0,(int) buf.CapacityUtilization);
        //    string[] data = datain.Split();

        //    humidity = double.Parse(data[2]);
        //    temp1 = double.Parse(data[3]);
        //    temp2 = double.Parse(data[4]);
        //    temp3 = double.Parse(data[5]);
        //    pressure = double.Parse(data[6]);
        //    illumunation = double.Parse(data[7]);
        //}






        //public override void exDataRequestMessage(Receiver r, Message m)
        //{
        //    DataMessage dat = new DataMessage(r);
        //    for (int i = 0; i < 6;i++ )
        //        dat.NIanaloginputs[i] = NIanaloginputs[i];
        //    dat.UPSstate = UPSstate;
        //    dat.VCommstate = VCommstate;
        //    Array.Copy(wellIntensities405, dat.WellIntensities405, dat.WellIntensities405.Length);
        //    Array.Copy(wellIntensities485, dat.WellIntensities485, dat.WellIntensities485.Length);
        //    for (int i = 0; i < 3; i++)
        //        dat.accel1acceleration[i] = accel1acceleration[i];
        //    for (int i = 0; i < 3; i++)
        //        dat.accel1rawacceleration[i] = accel1rawacceleration[i];
        //    for (int i = 0; i < 3; i++)
        //        dat.accel1vibration[i] = accel1vibration[i];
        //    for (int i = 0; i < 3; i++)
        //        dat.accel2acceleration[i] = accel2acceleration[i];
        //    for (int i = 0; i < 3; i++)
        //        dat.accel2rawacceleration[i] = accel2rawacceleration[i];
        //    for (int i = 0; i < 3; i++)
        //        dat.accel2vibration[i] = accel2vibration[i];

        //    dat.accel1state = accel1state;
        //    dat.accel2state = accel2state;

        //    dat.phidgets888state = phidgets888state;
        //    for (int i = 0; i < 8; i++)
        //        dat.phidgetsanalogInputs[i] = phidgetsAnalogInputs[i];
        //    for (int i = 0; i < 8; i++)
        //        dat.phidgetsdigitalInputs[i] = phidgetsDigitalInputs[i];
        //    for (int i = 0; i < 8; i++)
        //        dat.phidgetsdigitalOutputs[i] = phidgetsDigitalOutputs[i];
        //    dat.phidgetstempstate = phidgetstempstate;

        //    dat.vcommHumidity = humidity;
        //    dat.vcommIllumination = illumunation;
        //    dat.vcommPressure = pressure;
        //    dat.vcommTemperature1 = temp1;
        //    dat.vcommTemperature2 = temp2;
        //    dat.vcommTemperature3 = temp3;

        //    //System.Diagnostics.Process proc = System.Diagnostics.Process.GetCurrentProcess();
        //    //long available =  proc.VirtualMemorySize64;
        //    //long peak = proc.PeakVirtualMemorySize64;

        //    //if (available < peak / 2)//this seems to work well.
        //    //{
        //    //    if (image405 != null)
        //    //        lock (image405)
        //    //            dat.image405 = image405;
        //    //    if (image485 != null)
        //    //        lock (image485)
        //    //            dat.image485 = image485;
        //    //}

        //    dat.image405 = image405;
        //    dat.image485 = image485;
        //    dat.phidgetTemperature_ProbeTemp = phidgetTemperature_ProbeTemp;
        //    dat.phidgetTemperature_AmbientTemp = phidgetTemperature_AmbientTemp;
        //    dat.timestamp = DateTime.Now.Ticks;   

        //    dp.Broadcast(dat);
        //}

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