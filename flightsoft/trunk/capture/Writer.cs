using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using uGCapture.Controller;

namespace uGCapture
{
    class Writer : ReceiverController
    {
        private uint index = 0;
        public Writer()
        {
            index = 0;
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
                    fulbuf = StagingBuffer.PopFull();

                    switch (fulbuf.Type)
                    {
                        case (BufferType.PHIDGETS):
                            WritePhidgetsOutput(fulbuf);
                            break;
                        case (BufferType.IMAGE405):
                            index = uint.Parse(fulbuf.text);
                            WriteImageOutput(fulbuf,405);
                            break;
                        case (BufferType.IMAGE485):
                            index = uint.Parse(fulbuf.text);
                            WriteImageOutput(fulbuf, 485);
                            break;

                        default:
                            break;
                    }
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
            FileStream fs = File.Create("C:\\Data\\" + CaptureClass.directoryName + "\\" + filename, (int)(uint)buf.capacityUtilization, FileOptions.None);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(buf.Data);
            bw.Close();
            fs.Close();
        }

        private void WritePhidgetsOutput(Buffer<Byte> buf)
        {
            String filename = String.Format("Phidgets{0}.txt", index);
            FileStream fs = File.Create("C:\\Data\\"+CaptureClass.directoryName+"\\"+filename, (int)(uint)buf.capacityUtilization, FileOptions.None);
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
