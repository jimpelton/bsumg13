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

        private BufferType m_type;
        public BufferType Type
        {
            get { return m_type; }
            private set { m_type = Type; }
        }

        public int Length
        {
            get { return m_data.Length; }
        }

        private String m_text;
        public String Text
        {
            get { return m_text;  }
            set { m_text = value; }
        }

        private ulong m_capacityUtilization;
        public ulong CapacityUtilization
        {
            get { return m_capacityUtilization; }
            private set { m_capacityUtilization = value; }
        }

        private T[] m_data;
        public T[] Data
        {
            get { return m_data; }
            private set { m_data = value; }
        }
    
        /*
         * setData
         * copies passed data into the buffers memory.
         */
        public void setData(T[] input, BufferType ty)
        {
            m_type = ty;
            if (input.Length <= m_data.Length)
            {
                Array.Copy(input, m_data,input.Length);
                CapacityUtilization = (ulong)input.Length;
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

    }
}
