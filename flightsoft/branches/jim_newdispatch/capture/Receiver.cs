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

        public string Id
        {
            get { return m_id; }
        }
        private string m_id;


        protected Receiver(string id, bool receiving=true)
        {
            m_id = id;
            m_receiving = receiving;
            dp = Dispatch.Instance();
        }

        /// <summary>
        /// Accept the message and put it in the execution queue if the 
        /// receiving state of this Receiver is true.
        /// </summary>
        /// <param name="m">the message being delivered.</param>
        //public void accept(Message m)
        //{
        //    if (!IsReceiving)
        //    {
        //        return;
        //    }

        //    lock (msgs)
        //    {
        //        msgs.Enqueue(m);
        //    }
        //}
        
        public static void ExecuteMessageQueue(Receiver r)
        {
            while (true)
            {
                if (!r.IsReceiving)
                {
                    break;
                }
                r.dp.Next(r.Id).execute(r);
                Console.WriteLine("Receiver: {0} Executed {1}", r.Id, r.GetType());
            }
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
            return Id;
        }
    }
}