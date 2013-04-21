// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-04-15                                                                      
// ******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace uGCapture
{
    public class MockController : ReceiverController 
    {
        public int SleepMax { get; set; }
        public int SleepMin { get; set; }

        private int m_maxDataBytes;
        private Random m_rand;

        /// <summary>
        /// Instantiate a MockController with a default simulated work time of
        /// between 5ms and 500ms. Set SleepMin and SleepMax to change this.
        /// </summary>
        /// <param name="bp">the BufferPool this MockController should use.</param>
        /// <param name="id">identifier for this mockcontroller</param>
        /// <param name="receiving">start receiving messages</param>
        /// <param name="frame_time">default time between calls to DoFrame</param>
        public MockController(BufferPool<byte> bp, string id, 
            bool receiving = true, int frame_time = 500) : base(bp, id, receiving, frame_time)
        {
            m_maxDataBytes = bp.BufElem; //the bufferpool in this case is always bytes, so the number of bytes
                                         //in the pool = number of elements in the buffer.
            SleepMin = 5;
            SleepMax = 500;
        }

        protected override bool init()
        {
            m_rand = new Random();
            return true;
        }

        public override void DoFrame(object source, ElapsedEventArgs e)
        {
            Buffer<byte> buffer = BufferPool.PopEmpty();
            byte[] data = makeData();
            buffer.setData(data, BufferType.USHORT_IMAGE405);
            Thread.Sleep(m_rand.Next(SleepMin, SleepMax));
            BufferPool.PostFull(buffer);
        }

        private byte[] makeData()
        {
            int size = m_rand.Next(1, m_maxDataBytes);
            byte[] rval = new byte[size];
            m_rand.NextBytes(rval);
            return rval;
        }
    }
}
