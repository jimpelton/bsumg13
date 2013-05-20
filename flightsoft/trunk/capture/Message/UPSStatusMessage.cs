

namespace uGCapture
{
    public class UPSStatusMessage : StatusMessage
    {
        public UPSStatusMessage(Receiver s, StatusStr nstate = StatusStr.STAT_ERR, string mes = "")
            : base(s, nstate, mes)
        {
        }

        public override void execute(Receiver r)
        {
            r.exUPSStatusMessage(r, this);
        }
    }
}
