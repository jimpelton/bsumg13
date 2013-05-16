
using System;
using System.Collections;
using System.Collections.Generic;

namespace uGCapture
{
   
 public class DataSet<T>
    {
	/// <summary>
	/// Arrays for each data type.
	/// Index into this array with the BufferType enumerations.
	/// </summary>
        public List<T[]> lastData;

	
        public DataSet(int imgSize)
        {
	        lastData = new List<T[]>(9);   //one array for each enumeration in BufferType.
            lastData.Add(new T[imgSize]);  //image buffer
            lastData.Add(new T[imgSize]);  //image buffer

            for (int i = 2; i < lastData.Capacity; i++) //remaining buffers are for text data.
            {
		        lastData.Add(new T[4096]);
            }
        }

	/// <summary>
	/// Create a deep copy of the given data set.
	/// </summary>
	/// <param name="lastDataSet">The DataSet to make a copy of.</param>
        public DataSet(DataSet<T> lastDataSet)
        {
	        lastData = new List<T[]>(lastDataSet.lastData.Count);

            for (int i = 0; i < lastDataSet.lastData.Capacity; i++)
            {
                int sz = lastDataSet.lastData[i].Length;
		        T[] tarr = new T[sz];
                lastData.Add(tarr);
                Array.Copy(lastDataSet.lastData[i], tarr, tarr.Length);
            }
        }
    }
}
