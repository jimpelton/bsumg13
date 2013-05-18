

namespace uGCapture
{
    public class UPSStatusMessage : StatusMessage
    {
        public UPSStatusMessage(Receiver s, StatusStr nstate = StatusStr.STAT_ERR)
            : base(s, nstate)
        {
        }

        public override void execute(Receiver r)
        {
            r.exUPSStatusMessage(r, this);
        }
    }
}
