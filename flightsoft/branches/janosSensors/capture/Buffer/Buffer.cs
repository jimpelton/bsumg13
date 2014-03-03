// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-03-24                                                                      
// ******************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uGCapture
{
    /// <summary>
    /// Simple generically typed buffer.
    /// </summary>
    public class Buffer<T>
    {
	
	/// <summary>
	/// Gets this buffers BufferType.
	/// </summary>
        public BufferType Type
        {
            get { return m_type; }
            set { m_type = Type; }
        }
        private BufferType m_type;

	/// <summary>
	/// The number of elements in this Buffers backing array.
	/// </summary>
        public int Length
        {
            get { return m_data.Length; }
        }

	/// <summary>
	/// A string identifier for this buffer. Could be used to uniquely
        /// identify this buffer.
	/// </summary>
        public String Text
        {
            get { return m_text;  }
            set { m_text = value; }
        }
        private String m_text;

        public long FillTime
        {
            get { return m_filltime; }
            set { m_filltime = value; }
        }
        private long m_filltime = 0;

	/// <summary>
	/// The number of filled elements in this buffer.
	/// </summary>
        private int m_capacityUtilization;
        public int CapacityUtilization
        {
            get { return m_capacityUtilization; }
            private set { m_capacityUtilization = value; }
        }

	/// <summary>
	/// The backing array for this Buffer.
	/// </summary>
        private T[] m_data;
        public T[] Data
        {
            get { return m_data; }
            private set { m_data = value; }
        }

	/// <summary>
	/// Copies all elements of input into this buffer starting at an offset of 0 in
        /// both this buffer and the input array. 
	/// ty should be one of the enumerated types in BuffeType and should be
        /// correct in the sense of the content of this buffer fill.
	/// </summary>
	/// <param name="input">Data to copy into this buffer.</param>
	/// <param name="ty">The type of the data contained in input.</param>
        public void setData(T[] input, BufferType ty)
        {
            m_type = ty;
            if (input.Length <= m_data.Length)
            {
                int i = 0;
                for (; i < input.Length; i++)
                {
                    m_data[i] = input[i];
                }
                CapacityUtilization = i;
            }
        }

        /// <summary>
        /// Makes a buffer of nElements elements.
        /// Number of bytes of this buffer = nElements*sizeof(T).
        /// </summary>
        /// <param name="nElements">Number of elements that this buffer should
        /// contain. </param>
        public Buffer(int nElements=1)
        {
            m_data = new T[nElements];
            CapacityUtilization = 0;
            m_text = "";
        }

        /// <summary>
        /// Copies the data from buff into this 
        /// buffer's data array.
        /// </summary>
        /// <param name="buff">Original buffer</param>
        public Buffer(Buffer<T> buff)
        {
            m_data = new T[buff.Data.Length];

            this.CapacityUtilization = buff.CapacityUtilization;
            this.Type = buff.Type;
            this.Text = buff.Text;
            Array.Copy(buff.Data, m_data, buff.Data.Length);
        }

        public void resize(int nElem)
        {
            m_data = new T[nElem];
            CapacityUtilization = 0;
        }

        public override string ToString()
        {
            return Type.ToString();
        }

    }
}
