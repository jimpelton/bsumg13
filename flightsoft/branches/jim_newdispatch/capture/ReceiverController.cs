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
        /// True if this ReceiverController has been initialized correctly.
        /// </summary>
        public bool IsInit
        {
            get; 
            private set; 
        }

        /// <summary>
        /// Create a new ReceiverController that will receive messages from the Dispatch.
        /// The timer is set to disabled.
        /// The Receiving status is set to true.
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
            m_ticker.Elapsed += FrameElapsed;

            BufferPool = bp;
            FrameTime = frame_time;
        }

        // calls DoFrame() when the timer has expired.
        private void FrameElapsed(object source, ElapsedEventArgs e)
        {
            if (!IsInit) return;
            DoFrame(source, e);
        }

        /// <summary>
        /// Initializes this ReceiverController.
        /// </summary>
        /// <returns></returns>
        public bool Initialize()
        {
            return (IsInit = init());
        }

        //TODO: change init() to return bool value, which will be the value given to IsInit.

        /// <summary>
        /// A receivercontroller must implement init() which will initialize
        /// hardware and other objects when called.
        /// </summary>
        protected abstract bool init(); 

        /// <summary>
        /// Called after every FRAME_TIME interval.
        /// </summary>
        /// <param name="source">The sender of the event</param>
        /// <param name="e"></param>
        public abstract void DoFrame(object source, ElapsedEventArgs e);
    }
}
