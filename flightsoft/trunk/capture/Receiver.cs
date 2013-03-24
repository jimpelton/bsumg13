using System;
using System.Collections.Generic;

namespace uGCapture
{

    public abstract class Receiver
    {
        protected Queue<Message> msgs;
        protected Dispatch dp;
        //public static BufferPool<Byte> StagingBuffer;

        /// <summary>
        /// True if this Receiver is currently receiving messages.
        /// </summary>
        public bool Receiving
        {
            get;
            set;
        }
        private bool receiving = false;

        public Receiver()
        {
            msgs = new Queue<Message>();
            dp = Dispatch.Instance();
        }

        /// <summary>
        /// Accept the message and put it in the execution queue.
        /// </summary>
        /// <param name="m">the message being delivered.</param>
        public void accept(Message m)
        {
            msgs.Enqueue(m);
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
        /// Act upon given AptinaStatusMessage
        /// </summary>
        public virtual void exAptinaStatus(Receiver r, Message m) { ; }

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