﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uGCapture
{
    /// <summary>
    /// Simple generically typed buffer.
    /// </summary>
    class Buffer<T>
    {

        public int NumElements
        {
            get { return m_data.Length; }
        }

        private T[] m_data;
        public T[] Data 
        {
            get { return m_data; }
        }

        private  Buffer() { }
        /// <summary>
        /// Makes a buffer of nElements elements.
        /// Number of bytes of this buffer = nElements*sizeof(T).
        /// </summary>
        /// <param name="nElements"></param>
        public Buffer(int nElements=1)
        {
            m_data = new T[nElements];
        }

        /// <summary>
        /// Copies the data from buff into this 
        /// buffers data array.
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
        }
    }
}
