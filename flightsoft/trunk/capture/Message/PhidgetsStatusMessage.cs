

namespace uGCapture
{
    public class PhidgetsStatusMessage : StatusMessage
    {
        public PhidgetsStatusMessage(Receiver s, StatusStr nstate = StatusStr.STAT_ERR)
            : base(s, nstate)
        {
        }

        public override void execute(Receiver r)
        {
            r.exPhidgetsStatusMessage(r, this);
        }
    }
}
