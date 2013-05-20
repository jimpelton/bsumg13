

namespace uGCapture
{
    public class  AptinaStatusMessage : StatusMessage
    {
        public AptinaStatusMessage(Receiver s, StatusStr nstate = StatusStr.STAT_ERR, string mes = "")
            : base(s, nstate, mes)
        {
        }

        public override void execute(Receiver r)
        {
            r.exAptinaStatusMessage(r, this);
        }
    }
}

