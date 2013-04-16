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
        private bool m_isInit;
        public bool IsInit
        {
            get { return m_isInit; }
            set { m_isInit = value; }
        }

        private Random m_rand;

        public MockController(BufferPool<byte> bp) : base(bp)
        {
            m_maxDataBytes = bp.BufElem;
            SleepMin = 5;
            SleepMax = 500;
        }

        public override void init()
        {
            m_rand = new Random();
        }

        public override void DoFrame(object source, ElapsedEventArgs e)
        {
            if (!IsInit) return;

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
