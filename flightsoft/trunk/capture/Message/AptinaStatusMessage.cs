

namespace uGCapture
{
    public class  AptinaStatusMessage : StatusMessage
    {
        public AptinaStatusMessage(Receiver s, StatusStr nstate = StatusStr.STAT_ERR)
            : base(s, nstate)
        {
        }

        public override void execute(Receiver r)
        {
            r.exAptinaStatusMessage(r, this);
        }
    }
}

