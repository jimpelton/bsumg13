
namespace uGCapture
{
    public class PhidgetsTempStatusMessage : StatusMessage
    {

        public PhidgetsTempStatusMessage(Receiver s, StatusStr nstate = StatusStr.STAT_ERR)
            : base(s, nstate)
        {
        }

        public override void execute(Receiver r)
        {
            r.exPhidgetsTempStatusMessage(r, this);
        }
    }
}
