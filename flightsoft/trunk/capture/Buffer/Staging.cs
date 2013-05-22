// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-05-05                                                                      
// ******************************************************************************
using System;
using System.Collections.Generic;

namespace uGCapture
{

    /// <summary>
    /// Works in conjunction with DataSet to deliver the newest set of
    /// data available.
    /// 
    /// The newest data set is intended to be retrieved by a client that
    /// calls CaptureClass.GetLastData().
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Staging<T>
    {

        private object data_mutex = new object();
        private readonly DataSet<T> lastDataSet;

        private readonly Dictionary<BufferType, int> sizes = new Dictionary
            <BufferType, int>()
            {
                {BufferType.USHORT_IMAGE405, 0},
                {BufferType.USHORT_IMAGE485, 0},
                {BufferType.UTF8_ACCEL, 0},
                {BufferType.UTF8_NI6008, 0},
                {BufferType.UTF8_PHIDGETS, 0},
                {BufferType.UTF8_SPATIAL, 0},
                {BufferType.UTF8_UPS, 0},
                {BufferType.UTF8_VCOMM, 0}
            };

        public Staging(int imgSize, int utfSize)
        {
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
                    if (! lastDataSet.lastData.ContainsKey(buf.Type))
                    {
                        return;
                    }

                    T[] dest = lastDataSet.lastData[buf.Type];
                    int oldsz = dest.Length;
                    if (buf.CapacityUtilization <= oldsz)  //only fill the size of the cache.
                    {
                        oldsz = buf.CapacityUtilization;
                    }
                    Array.Copy(buf.Data, dest, oldsz);
                    sizes[buf.Type] = oldsz;
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
        /// <returns>A dataset truncated </returns>
        public DataSet<T> GetLastData()
        {
            lock (data_mutex)
            {
                return new DataSet<T>(lastDataSet, sizes);	 //deep copy       
            }
        }
    }
}
