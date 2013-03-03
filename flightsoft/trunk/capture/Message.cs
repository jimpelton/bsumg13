

namespace uGCapture
{
    abstract class Message
    {
        private Sender m_source;
        public Sender Source
        {
            get { return m_source; }
            set { m_source = value; }
        }

        public Message(Sender s) { m_source = s; }

        public abstract void execute(Receiver r);
    }

    class BiteTestMessage : Message
    {
        public BiteTestMessage(Sender s)
            : base(s) { ; }

        public override void execute(Receiver r)
        {
            r.exBiteTest(r);
        }
    }

    class PhidgetsStatusMessage : Message
    {
        PhidgetsStatusMessage(Sender s)
            : base(s) { ; }

        public override void execute(Receiver r)
        {
            r.exPhidgetsStatus(r);
        }
    }
}