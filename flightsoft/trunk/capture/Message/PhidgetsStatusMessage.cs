

namespace uGCapture
{
    public class PhidgetsStatusMessage : StatusMessage
    {
        public PhidgetsStatusMessage(Receiver s, StatusStr nstate = StatusStr.STAT_ERR, string mes = "")
            : base(s, nstate, mes)
        {
        }

        public override void execute(Receiver r)
        {
            r.exPhidgetsStatusMessage(r, this);
        }
    }
}
