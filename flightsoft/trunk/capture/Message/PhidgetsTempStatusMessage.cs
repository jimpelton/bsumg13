
namespace uGCapture
{
    public class PhidgetsTempStatusMessage : StatusMessage
    {

        public PhidgetsTempStatusMessage(Receiver s, Status nstate, ErrStr mes = 0)
            : base(s, nstate, mes)
        {
        }

        public override void execute(Receiver r)
        {
            r.exPhidgetsTempStatusMessage(r, this);
        }
    }
}
