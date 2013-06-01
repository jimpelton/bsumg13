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
        private static DateTime date1970 = new DateTime(1970, 1, 1);
        private Status lastStatus = Status.STAT_ERR;

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
        protected ReceiverController(BufferPool<byte> bp, string id, 
            bool receiving=true, bool executing=false) : base(id, receiving, executing)
        {

            BufferPool = bp;
        }

        /// <summary>
        /// Initializes this ReceiverController.
        /// </summary>
        /// <returns></returns>
        public bool Initialize()
        {
            return (IsInit = init());
        }


        /// <summary>
        /// A receivercontroller must implement init() which will initialize
        /// hardware and other objects when called.
        /// </summary>
        /// <param name="lastErr"></param>
        protected abstract bool init();


        protected void CheckedStatusBroadcast(LogMessage msg)
        {
            if (lastStatus != msg.getState())
            {
                lastStatus = msg.getState();
                dp.Broadcast(msg);
            }
        }

        protected void CheckedStatusBroadcast(ref Status lastStat, LogMessage msg)
        {
            if (lastStat != msg.getState())
            {
                lastStat = msg.getState();
                dp.Broadcast(msg);
            }
        }

        /*
        protected void CheckedStatusBroadcast<T>(T m) where T : LogMessage
        {
            LogMessage msg =  m;
            if(lastStatus!=msg.getState())
            {
                lastStatus = msg.getState();
                dp.Broadcast(m);
            }
        }

        protected void CheckedStatusBroadcast<T>(Status lastStatus, T m) where T : LogMessage
        {
            LogMessage msg = m;
            if (lastStatus != msg.getState())
            {
                lastStatus = msg.getState();
                dp.Broadcast(m);
            }
        }*/
    }
}
