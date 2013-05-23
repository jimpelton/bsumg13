

namespace uGCapture
{
    public abstract class StatusMessage : LogMessage
    {
        private Status state;
        public  Status getState() { return state; }

        public string getMessage()
        {
            return message;
        }

        protected StatusMessage(Receiver s, Status nstate, string mes = "")
            : base(s,mes)
        {
            state = nstate;
        }

        protected StatusMessage(Receiver s, Status nstate, ErrStr mes = 0)
            : base(s, mes, nstate)
        {
            state = nstate;
        }

        public override string ToString()
        {
            return state.ToString() + " " + message;
        }

    }
}
