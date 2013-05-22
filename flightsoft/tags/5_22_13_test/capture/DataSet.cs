
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
    public Dictionary<BufferType, T[]> lastData;

     public readonly int ImgSize;
     public readonly int UtfSize;



    public DataSet(int imgSize, int utfSize)
    {
        ImgSize = imgSize;
        UtfSize = utfSize;

        lastData = new Dictionary<BufferType, T[]>()
            {
                {BufferType.USHORT_IMAGE405, new T[imgSize]},
                {BufferType.USHORT_IMAGE485, new T[imgSize]},
                {BufferType.UTF8_ACCEL, new T[utfSize]},
                {BufferType.UTF8_NI6008, new T[utfSize]},
                {BufferType.UTF8_PHIDGETS, new T[utfSize]},
                {BufferType.UTF8_SPATIAL, new T[utfSize]},
                {BufferType.UTF8_UPS, new T[utfSize]},
                {BufferType.UTF8_VCOMM, new T[utfSize]}
            };
    }

    /// <summary>
    /// Create a deep copy of the given data set.
    /// </summary>
    /// <param name="lastDataSet">The DataSet to make a copy of.</param>
    public DataSet(DataSet<T> lastDataSet, Dictionary<BufferType, int> sizes)
	{
	    lastData = new Dictionary<BufferType, T[]>();
        //Array bufferTypes = Enum.GetValues(typeof (BufferType));

        foreach (KeyValuePair<BufferType, T[]> pair in lastDataSet.lastData)
        {
            int size = sizes[pair.Key];
            T[] dest = new T[size];
            Array.Copy(pair.Value, dest, dest.Length);
            lastData[pair.Key] = dest;
        }
    }
}
}
