// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-04-15                                                                      
// ******************************************************************************/

using System;

namespace uGCapture
{
    public class MockController : ReceiverController
    {
        public int SleepMax { get; set; }
        public int SleepMin { get; set; }
        public int LastSizeBytes { get; set; }

        private int m_maxDataBytes;
        private Random m_rand;
        private byte[] lastData;
        private String outputData;
        /// <summary>
        /// Instantiate a MockController with a default simulated work time of
        /// between 5ms and 500ms. Set SleepMin and SleepMax to change this.
        /// </summary>
        /// <param name="bp">the BufferPool this MockController should use.</param>
        /// <param name="id">identifier for this mockcontroller</param>
        /// <param name="receiving">start receiving messages</param>
        /// <param name="frame_time">default time between calls to DoFrame</param>
        public MockController(BufferPool<byte> bp, string id,
                              bool receiving = true)
            : base(bp, id, receiving)
        {
            m_maxDataBytes = bp.BufElem;
	        lastData = new byte[m_maxDataBytes];

            //the bufferpool in this case is always bytes, so the number of bytes
            //in the pool = number of elements in the buffer.
            SleepMin = 5;
            SleepMax = 500;
        }

        protected override bool init()
        {
            m_rand = new Random();
            return true;
        }

        /// <summary>
        /// Pops an empty buffer and fills it, then sleeps for a random amount
        /// of time between SleepMin and SleepMax, and finally posts the
        /// full buffer to the buffer pool.
        ///
        /// This is a convenience method for initiating the pop, fill, push
        /// sequence without relying on the heartbeat message.
        /// </summary>
        public void ManualPushData(BufferType bt)
        {
            Buffer<byte> buffer = BufferPool.PopEmpty();
            makeData();
            buffer.setData(lastData, bt);
            buffer.Text = Convert.ToString(DateTime.Now.Millisecond);
            BufferPool.PostFull(buffer);
        }

        public byte[] LastData()
        {
            byte[] rval;
            rval = new byte[LastSizeBytes];
            Array.Copy(lastData, rval, LastSizeBytes);
            return rval;
        }

        private void makeData()
        {
            int size = m_rand.Next(1, m_maxDataBytes);
            LastSizeBytes = size;
            lastData = new byte[size];
            m_rand.NextBytes(lastData);
        }

        /// <summary>
        /// Convenience method to use this MockController's
        ///  random number generator to generate a new array of random data. 
        /// The data is returned to the caller with no effects on this MockController.
        /// </summary>
        /// <returns>Array of random bytes.</returns>
        public byte[] GetRandomData()
        {
            int size = m_rand.Next(1, m_maxDataBytes);
            byte[] rval = new byte[size];
            m_rand.NextBytes(rval);
            return rval;
        }
        public override void exHeartBeatMessage(Receiver r, Message m)
        {
            dp.BroadcastLog(this, Status.STAT_NONE, "thump, thump");
        }
        public override void exAccumulateMessage(Receiver r, Message m)
        {
//                    outputData += GetUTCMillis().ToString() + " ";
//                    for (int i = 0; i < 3; i++)
//                        outputData += accel.axes[i].Acceleration + " ";
//                    outputData += "\n";
        }
    }
}
