
using System;
using System.Collections;
using System.Collections.Generic;

namespace uGCapture
{
   
 public class DataSet<T>
    {
        public List<T[]> lastData;

        public DataSet(int imgSize)
        {
	    lastData = new List<T[]>(9); //one array for each BufferType.
            lastData.Add(new T[imgSize]);
            lastData.Add(new T[imgSize]);
            for (int i = 2; i < lastData.Capacity; i++)
            {
		lastData.Add(new T[4096]);
            }
        }

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
