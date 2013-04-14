﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace uGCapture
{
public class Writer : ReceiverController
{
    private uint index485 = 0;
    private uint index405 = 0;
    private string m_directoryName;

    //This is the last written data stored temp so that we can send it to anyone who sends a data request.
    private long[, ,] WellIntensities;
    private bool[] phidgetsdigitalInputs;
    private bool[] phidgetsdigitalOutputs;
    private int[] phidgetsanalogInputs;
    private double phidgetTemperature_ProbeTemp;
    private double phidgetTemperature_AmbientTemp;
    private double[] accel1rawacceleration;
    private double[] accel1acceleration;
    private double[] accel1vibration;
    private double[] accel2rawacceleration;
    private double[] accel2acceleration;
    private double[] accel2vibration;
    private double[] NIanaloginputs;
    private double humidity = 0;
    private double temp1 = 0;
    private double temp2 = 0;
    private double temp3 = 0;
    private double pressure = 0;
    private double illumunation = 0;
    public int accel1state = 0;
    public int accel2state = 0;
    public int phidgets888state = 0;
    public int phidgetstempstate = 0;
    public int UPSstate = 0;
    public int VCommstate = 0;
    private Buffer<byte> image405 = null;
    private Buffer<byte> image485 = null;


    public string DirectoryName
    {
        get { return m_directoryName; }
        set { m_directoryName = value;}
    }

    public Writer(BufferPool<byte> bp) : base(bp)
    {
        WellIntensities = new long[2,16,12];
        phidgetsdigitalInputs = new bool[8];
        phidgetsdigitalOutputs = new bool[8];
        phidgetsanalogInputs = new int[8];
        accel1rawacceleration = new double[3];
        accel1acceleration = new double[3];
        accel1vibration = new double[3];
        accel2rawacceleration = new double[3];
        accel2acceleration = new double[3];
        accel2vibration = new double[3];
        NIanaloginputs = new double[6];
    }

    public override void init()
    {
        this.Receiving = true;
        dp.Register(this,"FileWriter");
        this.FrameTime = 50;
    }

    /*
     * WriteData
     * Writes data to the disk.
     * returns false if an error occurs.
     */
    public static void WriteData(Writer w)
    {
        Buffer<Byte> fulbuf = null;
        while (true)
        {
            try
            {
                fulbuf = w.BufferPool.PopFull();

                //if (fulbuf == null){ continue; }
                
                switch (fulbuf.Type)
                {
                    case (BufferType.UTF8_PHIDGETS):
                        w.WritePhidgetsOutput(fulbuf, Math.Min(w.index405, w.index485));
                        break;
                    case (BufferType.UTF8_VCOM):
                        w.WriteWeatherboardOutput(fulbuf, Math.Min(w.index405, w.index485));
                        break;
                    case (BufferType.UTF8_ACCEL):
                        w.WriteAccelerometerOutput(fulbuf, Math.Min(w.index405, w.index485));
                        break;
                    case (BufferType.UTF8_NI6008):
                        w.WriteNI6008Output(fulbuf, Math.Min(w.index405, w.index485));
                        break;
                    case (BufferType.USHORT_IMAGE405):
                        //index405 = uint.Parse(fulbuf.Text);
                        w.WriteImageOutput(fulbuf, 405, w.index405++);
                        break;
                    case (BufferType.USHORT_IMAGE485):
                        //index485 = uint.Parse(fulbuf.Text);
                        w.WriteImageOutput(fulbuf, 485, w.index485++);
                        break;
                    default:
                        break;
                }
                w.BufferPool.PostEmpty(fulbuf);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace); 
                return ;
            }
            w.ExecuteMessageQueue();
        } // while (fulbuf!=null);

       
        //return;
    }

    private void WriteImageOutput(Buffer<Byte> buf,int wavelength,uint index)
    {
        String filename = String.Format("data_{0}_{1}.raw", wavelength, index);
        FileStream fs = File.Create("C:\\Data\\" + m_directoryName + "\\" + filename, (int)buf.CapacityUtilization, FileOptions.None);
        BinaryWriter bw = new BinaryWriter(fs);
        bw.Write(buf.Data, 0, (int)buf.CapacityUtilization);
        bw.Close();
        fs.Close();
        StoreImageData(buf,wavelength,index);
        
    }

    private void WritePhidgetsOutput(Buffer<Byte> buf, uint index)
    {
        String filename = String.Format("Phidgets{0}.txt", index);
        FileStream fs = File.Create("C:\\Data\\"+m_directoryName+"\\"+filename, (int)buf.CapacityUtilization, FileOptions.None);
        BinaryWriter bw = new BinaryWriter(fs);
        bw.Write(buf.Data, 0, (int)buf.CapacityUtilization);
        bw.Close();
        fs.Close();
        StorePhidgetsData(buf,index);
    }

    private void WriteNI6008Output(Buffer<Byte> buf, uint index)
    {
        String filename = String.Format("NI6008_{0}.txt", index);
        FileStream fs = File.Create("C:\\Data\\" + m_directoryName + "\\" + filename, (int)buf.CapacityUtilization, FileOptions.None);
        BinaryWriter bw = new BinaryWriter(fs);
        bw.Write(buf.Data, 0, (int)buf.CapacityUtilization);
        bw.Close();
        fs.Close();
        StoreNI6008Data(buf,index);
    }

    private void WriteAccelerometerOutput(Buffer<Byte> buf, uint index)
    {
        String filename = String.Format("Accel{0}_{1}.txt", buf.Text, index);//we can use the text to differentiate between different accelerometers.
        FileStream fs = File.Create("C:\\Data\\" + m_directoryName + "\\" + filename, (int)buf.CapacityUtilization, FileOptions.None);
        BinaryWriter bw = new BinaryWriter(fs);
        bw.Write(buf.Data, 0, (int)buf.CapacityUtilization);
        bw.Close();
        fs.Close();
        StoreAccelerometerData(buf,index);
    }

    private void WriteWeatherboardOutput(Buffer<Byte> buf, uint index)
    {
        String filename = String.Format("Barometer{0}.txt", index);
        FileStream fs = File.Create("C:\\Data\\" + m_directoryName + "\\" + filename, (int)(uint)buf.CapacityUtilization, FileOptions.None);
        BinaryWriter bw = new BinaryWriter(fs);
        bw.Write(buf.Data, 0, (int)buf.CapacityUtilization);
        bw.Close();
        fs.Close();
        StoreWeatherboardData(buf,index);
    }

    private void StoreImageData(Buffer<Byte> buf, int wavelength, uint index)
    {
        Buffer<byte> buf2 = new Buffer<byte>(buf);
        if (wavelength == 405)
            image405 = buf2;
        else if (wavelength == 485)
            image485 = buf2;
        else
            dp.BroadcastLog(this, "Writer passed an image with an invalid wavelength.", 5);

    }

    private void StorePhidgetsData(Buffer<Byte> buf, uint index)
    {
        string datain = System.Text.Encoding.UTF8.GetString(buf.Data);
        string[] data = datain.Split();
        phidgetTemperature_ProbeTemp = double.Parse(data[2]);
        phidgetTemperature_AmbientTemp = double.Parse(data[3]);
        for (int i = 0; i < 8; i++)
        {
                phidgetsanalogInputs[i] = int.Parse(data[(i * 3) + 4]);
                phidgetsdigitalInputs[i] = bool.Parse(data[(i * 3) + 5]); ;
                phidgetsdigitalOutputs[i] = bool.Parse(data[(i * 3) + 6]); ;               
        }
    }

    private void StoreNI6008Data(Buffer<Byte> buf, uint index)
    {
        string datain = System.Text.Encoding.UTF8.GetString(buf.Data);
        string[] data = datain.Split();
        for (int i = 0; i < 6; i++)
            NIanaloginputs[i] = double.Parse(data[i + 1]);
    }

    private void StoreAccelerometerData(Buffer<Byte> buf, uint index)
    {
        string datain = System.Text.Encoding.UTF8.GetString(buf.Data);
        string[] data = datain.Split();
        //decide if it is the spacial or the accelerometer.
        if (buf.Text == "TODO:whatever the serial number of one of them is...")
        {
            for (int i = 2; i < 5; i++)
                accel1rawacceleration[i - 2] = double.Parse(data[i]);
            for (int i = 5; i < 8; i++)
                accel1acceleration[i - 5] = double.Parse(data[i]);
            for (int i = 8; i < 11; i++)
                accel1vibration[i - 8] = double.Parse(data[i]);
        }
        else
        {
            for (int i = 2; i < 5; i++)
                accel2rawacceleration[i - 2] = double.Parse(data[i]);
            for (int i = 5; i < 8; i++)
                accel2acceleration[i - 5] = double.Parse(data[i]);
            for (int i = 8; i < 11; i++)
                accel2vibration[i - 8] = double.Parse(data[i]);
        }

    }

    private void StoreWeatherboardData(Buffer<Byte> buf, uint index)
    {
        string datain = System.Text.Encoding.UTF8.GetString(buf.Data);
        string[] data = datain.Split();

        humidity = double.Parse(data[2]);
        temp1 = double.Parse(data[3]);
        temp2 = double.Parse(data[4]);
        temp3 = double.Parse(data[5]);
        pressure = double.Parse(data[6]);
        illumunation = double.Parse(data[7]);
    }




    public override void DoFrame(object source, ElapsedEventArgs e)
    {
        //WriteData();
    }

    public override void exDataRequest(Receiver r, Message m)
    {
        DataMessage dat = new DataMessage(r);
        dat.NIanaloginputs = NIanaloginputs;
        dat.UPSstate = UPSstate;
        dat.VCommstate = VCommstate;
        dat.WellIntensities = WellIntensities;
        dat.accel1acceleration = accel1acceleration;
        dat.accel1rawacceleration = accel1rawacceleration;
        dat.accel1state = accel1state;
        dat.accel1vibration = accel1vibration;
        dat.accel2acceleration = accel2acceleration;
        dat.accel2rawacceleration = accel2rawacceleration;
        dat.accel2state = accel2state;
        dat.accel2vibration = accel2vibration;
        dat.phidgets888state = phidgets888state;
        dat.phidgetsanalogInputs = phidgetsanalogInputs;
        dat.phidgetsdigitalInputs = phidgetsdigitalInputs;
        dat.phidgetsdigitalOutputs = phidgetsdigitalOutputs;
        dat.phidgetstempstate = phidgetstempstate;
        dat.timestamp = DateTime.Now.Ticks;   
        dp.Broadcast(dat);
    }

}
}
