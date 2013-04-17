// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-03-29                                                                      
// ******************************************************************************

using System;
using System.Collections.Generic;
using System.Threading;

namespace uGCapture
{
    /// <summary>
    /// A simple buffer pool which uses a stack and queue
    /// to keep track of empty and full buffers, respectively.
    /// The BufferPool parameter type must be a value type.
    /// </summary>
    public class BufferPool<T>  
        where T : struct
    {
        private Queue<Buffer<T>> fullBufs;
        private Stack<Buffer<T>> emptyBufs;

        /// <summary>
        /// Get number of elements each buffer was initialized with.
        /// Multiply by sizeof(T) to get size in bytes of each buffer.
        /// </summary>
        public int BufElem
        {
            get { return m_bufElem; }
        }
        private readonly int m_bufElem;

        /// <summary>
        /// Get number of buffers available (should be FullCount+EmptyCount).
        /// </summary>
        public int NumBufs
        {
            get { return m_numBufs; }
        }
        private readonly int m_numBufs;

        /// <summary>
        /// Get the number of full buffers available.
        /// </summary>
        public int FullCount
        {
            get { return fullBufs.Count; }
        }

        /// <summary>
        /// Get the number of empty buffers available.
        /// </summary>
        public int EmptyCount
        {
            get { return emptyBufs.Count; }
        }

        public Type BufferType
        {
            get { return typeof (T); }
        }


        /// <summary>
        /// Instantiate with empty buffers.
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
        /// <param name="nBuffs">Number of buffers to allocate.</param>
        /// <param name="nElem"> Number of elements for each buffer.</param>
        public BufferPool(int nBuffs, int nElem)
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
                Monitor.PulseAll(fullBufs);
            }
        }

        /// <summary>
        /// Post an empty buffer. The buffer will be queued
        /// for empty checkout. (which can be done via popempty()).
        /// </summary>
        /// <param name="b">The buffer to post.</param>
        public void PostEmpty(Buffer<T> b)
        {
            lock (emptyBufs)
            {
                emptyBufs.Push(b);
                Monitor.PulseAll(emptyBufs);
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
                while (fullBufs.Count == 0)
                {
                    Monitor.Wait(fullBufs);
                }
                //if (fullBufs.Count > 0)
                //{
                    b = fullBufs.Dequeue();
                //}
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
                while (emptyBufs.Count == 0)
                {
                    Monitor.Wait(emptyBufs);
                }
                //if (emptyBufs.Count > 0)
                //{
                    b = emptyBufs.Pop();
                //}
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
                while (fullBufs.Count == 0)
                {
                    Monitor.Wait(fullBufs,10000);
                }
                //if (fullBufs.Count > 0)
                //{
                    b = new Buffer<T>(fullBufs.Peek());
                //}
            }
            return b;
        }
    }
}
