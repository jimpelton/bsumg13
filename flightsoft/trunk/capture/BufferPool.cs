using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uGCapture
{
    /// <summary>
    /// A simple buffer pool which uses a stack and queue
    /// to keep track of empty and full buffers, respectively.
    /// The BufferPool parameter type must be a value type.
    /// </summary>
    class BufferPool<T>  
        where T : struct
    {
        private Queue<Buffer<T>> fullBufs;
        private Stack<Buffer<T>> emptyBufs;

        private readonly int m_bufElem;
        public int BufElem
        {
            get { return m_bufElem; }
        }

        private readonly int m_numBufs;
        public int NumBufs
        {
            get { return m_numBufs; }
        }

        /// <summary>
        /// Instantiate 
        /// </summary>
        public BufferPool()
        {
            fullBufs  = new Queue<Buffer<T>>();
            emptyBufs = new Stack<Buffer<T>>();
            m_numBufs = 0;
            m_bufElem = 0;
        }
        
        /// <summary>
        /// Instantiate a new BufferPool with nBuffs empty
        /// buffers initially.
        /// If nBuffs==0 then no buffers are allocated.
        /// </summary>
        /// <param name="nBuffs">Number of buffers to allocation.</param>
        public BufferPool(int nBuffs=0, int nElem=0)
        {
            if (nBuffs > 0)
            {
                fullBufs = new Queue<Buffer<T>>(nBuffs);
                emptyBufs = new Stack<Buffer<T>>(nBuffs);
                for (int i = 0; i < nBuffs; ++i)
                {
                    emptyBufs.Push(new Buffer<T>(nElem));
                }
            }
            else 
            { 
                fullBufs = new Queue<Buffer<T>>();
                emptyBufs = new Stack<Buffer<T>>();
            }
            m_numBufs = nBuffs;
            m_bufElem = nElem;
        }

        /// <summary>
        /// Post a full buffer to the pool. The buffer
        /// will be queued for checkout (via popfull).
        /// </summary>
        /// <param name="b">The buffer to post.</param>
        public void PostFull(Buffer<T> b)
        {
            lock(fullBufs)
            {
                fullBufs.Enqueue(b);
            }
        }

        /// <summary>
        /// Post an empty buffer. The buffer will be queued
        /// for empty checkout. (via popempty).
        /// </summary>
        /// <param name="b">The buffer to post.</param>
        public void PostEmpty(Buffer<T> b)
        {
            lock (emptyBufs)
            {
                emptyBufs.Push(b);
            }
        }

        /// <summary>
        /// Pop a full buffer for consumption. 
        /// </summary>
        public Buffer<T> PopFull()
        {
            Buffer<T> b = null;
            lock (fullBufs)
            {
                if (fullBufs.Count > 0)
                {
                    b = fullBufs.Dequeue();
                }
            }
            return b;
        }

        /// <summary>
        /// Pop an empty buffer for production.
        /// </summary>
        /// <returns>An empty buffer.</returns>
        public Buffer<T> PopEmpty()
        {
            Buffer<T> b = null;
            lock (emptyBufs)
            {
                if (emptyBufs.Count > 0)
                {
                    b = emptyBufs.Pop();
                }
            }
            return b;
        }

        /// <summary>
        /// Peek the current full buffer.
        /// </summary>
        /// <returns>A buffer that is a copy of the next buffer in the queue.</returns>
        public Buffer<T> PeekFullCopy()
        {
            Buffer<T> b = null;
            lock (fullBufs)
            {
                if (fullBufs.Count > 0)
                {
                    b = new Buffer<T>(fullBufs.Peek());
                }
            }
            return b;
        }
    }
}
