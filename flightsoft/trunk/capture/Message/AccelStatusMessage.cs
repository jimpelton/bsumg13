

namespace uGCapture
{
    public class  AccelStatusMessage : StatusMessage
    {

        public AccelStatusMessage(Receiver s, StatusStr nstate = StatusStr.STAT_ERR)
            : base(s, nstate)
        {
        }

        public override void execute(Receiver r)
        {
            r.exAccelStatusMessage(r, this);
        }
    }
}
