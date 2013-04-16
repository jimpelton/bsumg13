using System;
using System.Collections.Generic;

namespace uGCapture
{

    public abstract class Receiver
    {
        protected Queue<Message> msgs;
        protected Dispatch dp;
        private bool m_receiving; 
        private object receivingMutex = new object();
        private object msgsMutex = new object();

        /// <summary>
        /// True if this Receiver is currently receiving messages.
        /// </summary>
        public bool Receiving
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


        protected Receiver(bool receiving=true)
        {
            m_receiving = receiving;
            msgs = new Queue<Message>();
            dp = Dispatch.Instance();
        }

        /// <summary>
        /// Accept the message and put it in the execution queue if the 
        /// receiving state of this Receiver is true.
        /// </summary>
        /// <param name="m">the message being delivered.</param>
        public void accept(Message m)
        {
            if (!Receiving)
            {
                return;
            }

            lock (msgs)
            {
                msgs.Enqueue(m);
            }
        }
        
        public void ExecuteMessageQueue()
        {
            
        }

        private void executeNextMessage()
        {
            Message m;
            lock (msgs)
            {
                if (msgs.Count > 0)
                {
                    m = msgs.Dequeue();
                }

            }
            m.execute(this);
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
        public virtual void exPhidgetsStatusMessage(Receiver r, Message m) { ; }

        /// <summary>
        /// Execute a DataMessage. 
        /// </summary>
        public virtual void exDataMessage(Receiver r, Message m) { ; }

        /// <summary>
        /// Execute a DataRequestMessage.
        /// </summary>
        public virtual void exDataRequestMessage(Receiver r, Message m) { ; }

        /// <summary>
        /// Act upon given AptinaStatusMessage
        /// </summary>
        public virtual void exAptinaStatusMessage(Receiver r, Message m) { ; }

        /// <summary>
        /// Generate a SetCaptureStateMessage
        /// </summary>
        public virtual void exSetCaptureStateMessage(Receiver r, Message m) { ; }

        /// <summary>
        /// Act on given LogMessage.
        /// </summary>
        public virtual void exLogMessage(Receiver r, Message m) { ; }

        public override string ToString()
        {
            return "Base Receiver";
        }
    }
}