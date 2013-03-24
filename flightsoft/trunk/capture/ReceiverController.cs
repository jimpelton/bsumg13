using System.Timers;


namespace uGCapture
{
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
        public int FrameTime { get; set; }
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
        
        protected ReceiverController() { }

        protected ReceiverController(BufferPool<byte> bp, int frame_time=500) 
        {
            m_bufferPool = bp;
            m_frameTime = frame_time;

            m_ticker = new Timer(frame_time);
            m_ticker.Elapsed += new ElapsedEventHandler(DoFrame);
            m_ticker.Enabled = false;
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
