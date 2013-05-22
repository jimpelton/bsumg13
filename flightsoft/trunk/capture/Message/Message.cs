// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-03-06                                                                      
// ******************************************************************************


using System;
using System.Collections.Generic;

namespace uGCapture
{
    public abstract class Message
    {
        public DateTime CreateTime
        {
            get { return m_createTime; }
            private set { m_createTime = value; }
        }
        private DateTime m_createTime;

        /// <summary>
        /// The originator of this message
        /// </summary>
        public Receiver Sender
        {
            get { return m_sender; }
            private set { m_sender = value; }
        }
        private Receiver m_sender;

        /// <summary>
        /// Creates this message. The receiver that generated this message
        /// should pass itself into this constructor.
        /// </summary>
        /// <param name="s">The Receiver that generated this message.</param>
        public Message(Receiver sender)
        {
            m_createTime = DateTime.Now;
            m_sender = sender;
        }

        public virtual void executeLog(Receiver r) { ; }

        public abstract void execute(Receiver r);

        


    }
}