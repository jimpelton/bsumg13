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
    private uint index = 0;
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
        throw new NotImplementedException();
    }

    /*
     * WriteData
     * Writes data to the disk.
     * returns false if an error occurs.
     */
    public Boolean WriteData()
    {
        Buffer<Byte> fulbuf = null;
        do
        {
            try
            {
                fulbuf = BufferPool.PopFull();

                switch (fulbuf.Type)
                {
                    case (BufferType.PHIDGETS):
                        WritePhidgetsOutput(fulbuf);
                        break;
                    case (BufferType.IMAGE405):
                        index = uint.Parse(fulbuf.Text);
                        WriteImageOutput(fulbuf,405);
                        break;
                    case (BufferType.IMAGE485):
                        index = uint.Parse(fulbuf.Text);
                        WriteImageOutput(fulbuf, 485);
                        break;

                    default:
                        break;
                }
                BufferPool.PostEmpty(fulbuf);
            }
            catch (Exception e)
            {
                return false;
            }
        } while (fulbuf!=null);
        

        return true;
    }

    private void WriteImageOutput(Buffer<Byte> buf,int wavelength)
    {
        String filename = String.Format("data_{0}_{1}.raw", wavelength, index);
        FileStream fs = File.Create("C:\\Data\\" + m_directoryName + "\\" + filename, (int)(uint)buf.CapacityUtilization, FileOptions.None);
        BinaryWriter bw = new BinaryWriter(fs);
        bw.Write(buf.Data);
        bw.Close();
        fs.Close();
    }

    private void WritePhidgetsOutput(Buffer<Byte> buf)
    {
        String filename = String.Format("Phidgets{0}.txt", index);
        FileStream fs = File.Create("C:\\Data\\"+m_directoryName+"\\"+filename, (int)(uint)buf.CapacityUtilization, FileOptions.None);
        BinaryWriter bw = new BinaryWriter(fs);
        bw.Write(buf.Data);
        bw.Close();
        fs.Close();
    }

    public override void DoFrame(object source, ElapsedEventArgs e)
    {
        WriteData();
    }


}
}
