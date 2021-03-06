﻿// ******************************************************************************
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
        //TODO: Remove
        //public virtual void exDataMessage(Receiver r, Message m) { ; }

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

        /// <summary>
        /// Act on a ReceiverCleanUpMessage().
        /// 
        /// The parent method Receiver.exReceiverCleanUpMessage needs to be called or
        /// none of the threads will get IsReceiving=false, or you need to set IsReceiving=false
        /// everywhere exReceiverCleanUpMessage is reimplemented.
        /// 
        /// On the next heartbeat sleeping threads will cycle and break out of there worker loop.
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

    }
}