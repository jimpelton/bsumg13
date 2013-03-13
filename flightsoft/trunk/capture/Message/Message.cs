

namespace uGCapture
{
    abstract class Message
    {
        private Receiver m_source;
        public Receiver Source
        {
            get { return m_source; }
            set { m_source = value; }
        }

        /// <summary>
        /// Creates this message. The receiver that generated this message
        /// should pass itself into this constructor.
        /// </summary>
        /// <param name="s">The Receiver that generated this message.</param>
        public Message(Receiver s) { m_source = s; }

        public abstract void execute(Receiver r);
    }




}