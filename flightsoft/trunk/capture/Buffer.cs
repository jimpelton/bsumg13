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

        private BufferType type;
        private T[] m_data;

        public int NumElements
        {
            get { return m_data.Length; }
        }

        public String text
        {
            get { return text; }
            set { text = value; }
        }
        public ulong capacityUtilization
        {
            get { return capacityUtilization; }
            set { capacityUtilization = value; }
        }

        public T[] Data 
        {
            get { return m_data; }
        }
    
        /*
         * setData
         * copies passed data into the buffers memory.
         * 
         */
        public void setData(T[] input, BufferType ty)
        {
            type = ty;
            if(input.Length <= m_data.Length)
                for (int i = 0; i < input.Length; i++)
                {
                    m_data[i] = input[i];
                }
        }


        /// <summary>
        /// Makes a buffer of nElements elements.
        /// Number of bytes of this buffer = nElements*sizeof(T).
        /// </summary>
        /// <param name="nElements"></param>
        public Buffer(int nElements=1)
        {
            m_data = new T[nElements];
            capacityUtilization = 0;
        }

        /// <summary>
        /// Copies the data from buff into this 
        /// buffer's data array.
        /// </summary>
        /// <param name="buff">Original buffer</param>
        public Buffer(Buffer<T> buff)
        {
            m_data = new T[buff.Data.Length];
            Array.Copy(buff.Data, m_data, buff.Data.Length);
        }

        public void resize(int nElem)
        {
            m_data = new T[nElem];
            capacityUtilization = 0;
        }

    }
}
