

namespace uGCapture
{
    public class  AccelStatusMessage : StatusMessage
    {

        public AccelStatusMessage(Receiver s, StatusStr nstate = StatusStr.STAT_ERR, string mes = "")
            : base(s, nstate, mes)
        {
        }

        public override void execute(Receiver r)
        {
            r.exAccelStatusMessage(r, this);
        }
    }
}
