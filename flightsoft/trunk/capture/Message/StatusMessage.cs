

namespace uGCapture
{
    public abstract class StatusMessage : LogMessage
    {
        private Status state;
        public  Status getState() { return state; }

        protected StatusMessage(Receiver s, Status nstate, string mes = "")
            : base(s, mes, nstate)
        {
            state = nstate;
        }

        protected StatusMessage(Receiver s, Status nstate, ErrStr mes = 0)
            : base(s, mes, nstate)
        {
            state = nstate;
        }

        public override void execute(Receiver r)
        {
            r.exStatusMessage(r, this);
        }

        //public override string ToString()
        //{
        //    return state.ToString() + " " + message;
        //}

    }
}
