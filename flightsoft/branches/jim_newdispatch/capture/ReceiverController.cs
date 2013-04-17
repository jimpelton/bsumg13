// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-03-24                                                                      
// ******************************************************************************

using System;
using System.Timers;


namespace uGCapture
{
    /// <summary>
    /// ReceiverController adds a BufferPool to the Receiver hierarchy. The ReceiverController is
    /// designed to capture data from some hardware device and shove the data into the bufferpool.
    /// A ReceiverController also adds a Timer loop to the Receiver hierarchy. The default firing time
    /// is 500ms. Currently the Timers are horribly abused.
    /// 
    /// </summary>
    public abstract class ReceiverController : Receiver
    {
        /// <summary>
        /// Enables/Disables this ReceiverController's timer.
        /// </summary>
        public bool TickerEnabled
        {
            get { return m_ticker.Enabled; }
            set { m_ticker.Enabled = value; }
        }
        private readonly Timer m_ticker;

        /// <summary>
        /// Update interval that the DoFrame ElapsedEventHandler is called.
        /// </summary>
        public int FrameTime
        {
            get { return m_frameTime; }
            set
            {
                m_frameTime = value;
                m_ticker.Interval = m_frameTime;
            }
        }
        private int m_frameTime;

        /// <summary>
        /// Main pool that every Receiver controller writes into.
        /// </summary>
        public BufferPool<byte> BufferPool
        {
            get { return m_bufferPool; }
            protected set { m_bufferPool = value; }
        }
        private BufferPool<byte> m_bufferPool;

        /// <summary>
        /// Create a new ReceiverController that will receive messages from the Dispatch.
        /// </summary>
        /// <param name="bp">The BufferPool for this Receiver.</param>
        /// <param name="id">A unique identifier for this ReceiverController.</param>
        /// <param name="receiving">If this ReceiverController should begin life receiving. Defaults to true.</param>
        /// <param name="frame_time">The interval between the DoFrame timer events. Defaults to 500ms.</param>
        protected ReceiverController(BufferPool<byte> bp, string id, 
            bool receiving=true, int frame_time=500) : base(id, receiving)
        {
            m_ticker = new Timer(frame_time);
            m_ticker.Enabled = false;
            m_ticker.Elapsed += DoFrame;

            BufferPool = bp;
            FrameTime = frame_time;
        }

        /// <summary>
        /// A receivercontroller must implement init() which will initialize
        /// hardware and other objects when called.
        /// </summary>
        public abstract void init(); 

        /// <summary>
        /// Called after every FRAME_TIME interval.
        /// </summary>
        /// <param name="source">The sender of the event</param>
        /// <param name="e"></param>
        public abstract void DoFrame(object source, ElapsedEventArgs e);
    }
}
