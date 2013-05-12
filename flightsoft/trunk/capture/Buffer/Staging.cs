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

        public Staging(int imgSize)
        {
	    size405 = 0;
            size485 = 0;
	    lastDataSet = new DataSet<T>(imgSize);
        }

        public void Inspect(Buffer<T> buf) 
        {
            try
            {
                lock (data_mutex)
                {
                    T[] dest = lastDataSet.lastData[(int)buf.Type];
                    Array.Copy(buf.Data, dest, dest.Length);
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
