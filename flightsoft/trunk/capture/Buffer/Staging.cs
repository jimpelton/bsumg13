// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-05-05                                                                      
// ******************************************************************************
using System;

namespace uGCapture
{
    internal struct ImagePair<ImgT>
    {
        public ImgT[] image405;
        public ImgT[] image485;
        public int size405;
        public int size485;

        public ImagePair(int sz)
        {
            size405 = 0;
            size485 = 0;
            image405 = new ImgT[sz];
            image485 = new ImgT[sz];
        }
    }

    public class Staging<T>
    {
        private ImagePair<T> imagePair;
        private object lock485 = new object();
        private object lock405 = new object();

        public Staging(int imgSize)
        {
            imagePair = new ImagePair<T>(imgSize);
        }

        public void Inspect(Buffer<T> buf) 
        {
            switch (buf.Type)
            {
                case BufferType.USHORT_IMAGE405:
                    lock (lock405)
                    {
                        Array.Copy(buf.Data, imagePair.image405, (int)buf.CapacityUtilization);
                        imagePair.size405 = (int) buf.CapacityUtilization;
                    }
                    break;
                case BufferType.USHORT_IMAGE485:
                    lock (lock485)
                    {
                        Array.Copy(buf.Data, imagePair.image485, (int)buf.CapacityUtilization);
                        imagePair.size485 = (int) buf.CapacityUtilization;
                    }
                    break;
                case BufferType.UTF8_ACCEL:
                    break;
                case BufferType.UTF8_SPATIAL:
                    break;
                case BufferType.UTF8_PHIDGETS:
                    break;
                case BufferType.UTF8_NI6008:
                    break;
                case BufferType.UPS:
                    break;
                case BufferType.UTF8_VCOMM:
                    break;
            }
        }

        /// <summary>
        /// Get a reference to the latest image data.
        /// </summary>
        /// <param name="nm"></param>
        /// <returns></returns>
        public T[] ImageData(int nm)
        {
            switch (nm)
            {
                case 0:
                    lock (lock405)
                    {
                        T[] t = new T[imagePair.size405];
                        Array.Copy(imagePair.image405, t, t.Length);
                        return t;
                    }
                case 1:
                    lock (lock485)
                    {
                        T[] t = new T[imagePair.size485];
                        Array.Copy(imagePair.image485, t, t.Length);
                        return t;
                    }
                default: return new T[0];
            }
        }
    }
}
