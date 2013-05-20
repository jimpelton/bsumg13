

namespace uGCapture
{
    public abstract class StatusMessage : Message
    {
        private StatusStr state;
        public  StatusStr getState() { return state; }
        private string mes;

        public string getMessage()
        {
            return mes;
        }

        protected StatusMessage(Receiver s, StatusStr nstate, string message)
            : base(s)
        {
            state = nstate;
            mes = message;
        }
    }
}
