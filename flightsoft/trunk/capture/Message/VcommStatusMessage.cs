

namespace uGCapture
{
    public class VcommStatusMessage : StatusMessage
    {
        public VcommStatusMessage(Receiver s, StatusStr nstate=StatusStr.STAT_ERR, string mes = "")
            : base(s, nstate, mes)
        {
        }

        public override void execute(Receiver r)
        {
            r.exVcommStatusMessage(r, this);
        }
    }
}
