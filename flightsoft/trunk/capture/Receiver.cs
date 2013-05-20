// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-04-13                                                                      
// ******************************************************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace uGCapture
{

    public abstract class Receiver
    {
        protected Dispatch dp;
        private bool m_receiving; 
        private object receivingMutex = new object();

        // 
        protected static DateTime dateTime1970 =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// True if this Receiver is currently receiving messages.
        /// </summary>
        public bool IsReceiving
        {
            get 
            { 
                bool rval;
                lock (receivingMutex)
                {
                    rval = m_receiving;
                }
                return rval;
            }

            set
            {
                lock (receivingMutex)
                {
                    m_receiving = value;
                }
            }
        }

        /// <summary>
        /// String which is a unique identifier for this Receiver.
        /// </summary>
        public string Id
        {
            get { return m_id; }
        }
        private string m_id;

        /// <summary>
        /// Last error value.
        /// </summary>
        public ErrStr Errno
        {
            get { return m_errno; }
            protected set { m_errno = value; }
        }
        private ErrStr m_errno;


        protected Receiver(string id, bool receiving=true)
        {
            m_id = id;
            m_receiving = receiving;
            dp = Dispatch.Instance();
        }
        

        /// <summary>
        /// Milliseconds since the unix epoch (Jan 1, 1970).
        /// </summary>
        /// <returns></returns>
        protected long GetUTCMillis()
        {
            return (long) Math.Floor((DateTime.UtcNow - dateTime1970).TotalMilliseconds);
        }

        /// <summary>
        /// Any receiver that should respond to the Bite test message should
        /// override this method.
        ///
        /// This method could generate a BiteTestResultMessage, however the default behavior 
        /// is to do nothing.
        /// </summary>
        public virtual void exBiteTestMessage(Receiver r, Message m) { ; }

        /// <summary>
        /// Generate a PhidgetsStatusMessage
        /// </summary>
        

        /// <summary>
        /// Execute a DataRequestMessage.
        /// </summary>
        public virtual void exDataRequestMessage(Receiver r, Message m) { ; }

        /// <summary>
        /// Generate a SetCaptureStateMessage
        /// </summary>
        public virtual void exSetCaptureStateMessage(Receiver r, Message m) { ; }

        /// <summary>
        /// Act on an AccumulateMessage.
        /// </summary>
        public virtual void exAccumulateMessage(Receiver r, Message m) { ; }

        /// <summary>
        /// Act on a HeartBeatMessage.
        /// </summary>
        public virtual void exHeartBeatMessage(Receiver r, Message m) { ; }

        /// <summary>
        /// Act on given LogMessage.
        /// </summary>
        public virtual void exLogMessage(Receiver r, Message m) { ; }

        // Updates given to the gui for updating as messages are passed throug the following:
        public virtual void exUPSStatusMessage(Receiver r, Message m) { ; }
        public virtual void exAccelStatusMessage(Receiver r, Message m) { ; }
        public virtual void exVcommStatusMessage(Receiver r, Message m) { ; }
        public virtual void exNI6008StatusMessage(Receiver r, Message m) { ; }
        public virtual void exAptinaStatusMessage(Receiver r, Message m) { ; }
        public virtual void exSpatialStatusMessage(Receiver r, Message m) { ; }
        public virtual void exPhidgetsStatusMessage(Receiver r, Message m) { ; }
        public virtual void exPhidgetsTempStatusMessage(Receiver r, Message m) { ; }
        
        //our interdevice command message.
        public virtual void exCommandMessage(Receiver r, Message m) { ; }

        /// <summary>
        /// Act on a ReceiverCleanUpMessage(). Default behavior of 
        /// Receiver.exReceiverCleanUpMessage() is to set IsReceiving=false,
        ///  causing the message processing thread for this Receiver to exit 
        /// on the next heart beat.
        /// 
        /// The parent method Receiver.exReceiverCleanUpMessage needs to be called or
        /// none of the threads will get IsReceiving=false, or you need to set 
        /// IsReceiving=false manually.
        /// 
        /// On the next heartbeat sleeping threads will cycle and break out of the worker loop.
        /// </summary>
        public virtual void exReceiverCleanUpMessage(Receiver r, Message m)
        {
            IsReceiving = false;
        }

        public virtual void exTest(Receiver r, Message m) { ; }

        public override string ToString()
        {
            return Id;
        }


        public virtual void log(Receiver r, StatusMessage statusMessage)
        {
            
        }
    }
}