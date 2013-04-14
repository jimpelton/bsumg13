using System;
using System.Collections.Generic;

namespace uGCapture
{

    public abstract class Receiver
    {
        protected Queue<Message> msgs;
        protected Dispatch dp;

        /// <summary>
        /// True if this Receiver is currently receiving messages.
        /// </summary>
        public bool Receiving
        {
            get;
            set;
        }
        //private bool receiving = false;


        protected Receiver()
        {
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
            //if (receiving)
            //{
            lock (msgs)
            {
                msgs.Enqueue(m);
            }
            //}
        }
        
        public void ExecuteMessageQueue()
        {
            lock (msgs)
            {
                while (msgs.Count > 0)
                {
                    msgs.Dequeue().execute(this);
                }
            }
        }

        /// <summary>
        /// Any receiver that should respond to the Bite test message should
        /// override this method.
        ///
        /// This method could generate a BiteTestResultMessage, however the default behavior 
        /// is to do nothing.
        /// </summary>
        public virtual void exBiteTest(Receiver r, Message m) { ; }

        /// <summary>
        /// Generate a PhidgetsStatusMessage
        /// </summary>
        public virtual void exPhidgetsStatus(Receiver r, Message m) { ; }

        /// <summary>
        /// Generate a DataMessage
        /// </summary>
        public virtual void exData(Receiver r, Message m) { ; }

        /// <summary>
        /// Generate a DataMessage
        /// </summary>
        public virtual void exDataRequest(Receiver r, Message m) { ; }

        /// <summary>
        /// Act upon given AptinaStatusMessage
        /// </summary>
        public virtual void exAptinaStatus(Receiver r, Message m) { ; }

        /// <summary>
        /// Generate a SetCaptureStateMessage
        /// </summary>
        public virtual void exSetCaptureState(Receiver r, Message m) { ; }

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