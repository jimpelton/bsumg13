

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

        public Message(Receiver s) { m_source = s; }

        public abstract void execute(Receiver r);
    }




}