

namespace uGCapture
{
    public abstract class StatusMessage : LogMessage
    {
        private StatusStr state;
        public  StatusStr getState() { return state; }

        public string getMessage()
        {
            return message;
        }

        protected StatusMessage(Receiver s, StatusStr nstate, string mes)
            : base(s,mes)
        {
            state = nstate;
        }


        public override string ToString()
        {
            return state.ToString() + " " + message;
        }

    }
}
