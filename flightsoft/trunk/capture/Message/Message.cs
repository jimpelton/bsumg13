

namespace uGCapture
{
    public abstract class Message
    {
        private Receiver m_sender;
        public Receiver Sender
        {
            get { return m_sender; }
            set { m_sender = value; }
        }

        /// <summary>
        /// Creates this message. The receiver that generated this message
        /// should pass itself into this constructor.
        /// </summary>
        /// <param name="s">The Receiver that generated this message.</param>
        public Message(Receiver sender) { m_sender = sender; }

        public abstract void execute(Receiver r);
    }




}