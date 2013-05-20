
namespace uGCapture
{
    public class PhidgetsTempStatusMessage : StatusMessage
    {

        public PhidgetsTempStatusMessage(Receiver s, StatusStr nstate = StatusStr.STAT_ERR, string mes = "")
            : base(s, nstate, mes)
        {
        }

        public override void execute(Receiver r)
        {
            r.exPhidgetsTempStatusMessage(r, this);
        }
    }
}
