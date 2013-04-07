using System;
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

    public string DirectoryName
    {
        get { return m_directoryName; }
        set { m_directoryName = value;}
    }

    public Writer(BufferPool<byte> bp) : base(bp)
    {
    }

    public override void init()
    {
        //this.Receiving = true;
        //dp.Register(this,"FileWriter");

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
    }

    private void WritePhidgetsOutput(Buffer<Byte> buf, uint index)
    {
        String filename = String.Format("Phidgets{0}.txt", index);
        FileStream fs = File.Create("C:\\Data\\"+m_directoryName+"\\"+filename, (int)buf.CapacityUtilization, FileOptions.None);
        BinaryWriter bw = new BinaryWriter(fs);
        bw.Write(buf.Data, 0, (int)buf.CapacityUtilization);
        bw.Close();
        fs.Close();
    }

    private void WriteNI6008Output(Buffer<Byte> buf, uint index)
    {
        String filename = String.Format("NI6008_{0}.txt", index);
        FileStream fs = File.Create("C:\\Data\\" + m_directoryName + "\\" + filename, (int)buf.CapacityUtilization, FileOptions.None);
        BinaryWriter bw = new BinaryWriter(fs);
        bw.Write(buf.Data, 0, (int)buf.CapacityUtilization);
        bw.Close();
        fs.Close();
    }

    private void WriteAccelerometerOutput(Buffer<Byte> buf, uint index)
    {
        String filename = String.Format("Accel{0}_{1}.txt", buf.Text, index);//we can use the text to differentiate between different accelerometers.
        FileStream fs = File.Create("C:\\Data\\" + m_directoryName + "\\" + filename, (int)buf.CapacityUtilization, FileOptions.None);
        BinaryWriter bw = new BinaryWriter(fs);
        bw.Write(buf.Data, 0, (int)buf.CapacityUtilization);
        bw.Close();
        fs.Close();
    }

    private void WriteWeatherboardOutput(Buffer<Byte> buf, uint index)
    {
        String filename = String.Format("Barometer{0}.txt", index);
        FileStream fs = File.Create("C:\\Data\\" + m_directoryName + "\\" + filename, (int)(uint)buf.CapacityUtilization, FileOptions.None);
        BinaryWriter bw = new BinaryWriter(fs);
        bw.Write(buf.Data, 0, (int)buf.CapacityUtilization);
        bw.Close();
        fs.Close();
    }

    public override void DoFrame(object source, ElapsedEventArgs e)
    {
        //WriteData();
    }


}
}
