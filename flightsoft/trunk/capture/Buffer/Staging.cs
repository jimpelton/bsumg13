// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-05-05                                                                      
// ******************************************************************************
using System;
using System.Text;

namespace uGCapture
{

    public class Staging<T>
    {

        public int size405;
        public int size485;

        private object data_mutex = new object();

        private DataSet<T> lastDataSet; 

        public Staging(int imgSize, int utfSize)
        {
            size405 = 0;
            size485 = 0;
            lastDataSet = new DataSet<T>(imgSize, utfSize);
        }

        /// <summary>
        /// Copy this buffer's contents into the appropriate staging
        /// cache in lastDataSet.
        /// </summary>
        /// <param name="buf"></param>
        public void Inspect(Buffer<T> buf) 
        {
            try
            {
                lock (data_mutex)
                {
                    T[] dest = lastDataSet.lastData[buf.Type];
                    int sz = dest.Length;
                    if (buf.CapacityUtilization >= sz)  //only fill the size of the cache.
                    {
                        sz = buf.CapacityUtilization;
                    }
                    Array.Copy(buf.Data, dest, sz);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.StackTrace);                
            }
        }

        /// <summary>
        /// Get a copy of the latest data.
        /// </summary>
        /// <param name="nm"></param>
        /// <returns></returns>

        public DataSet<T> GetLastData()
        {
            lock (data_mutex)
            {
                return new DataSet<T>(lastDataSet);	 //deep copy       
            }
        }
    }
}
