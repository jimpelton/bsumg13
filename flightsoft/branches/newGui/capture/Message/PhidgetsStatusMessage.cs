

namespace uGCapture
{
    public class PhidgetsStatusMessage : StatusMessage
    {
        public PhidgetsStatusMessage(Receiver s, Status nstate, ErrStr mes = 0)
            : base(s, nstate, mes)
        {
        }

        public override void execute(Receiver r)
        {
            base.execute(r);
            r.exPhidgetsStatusMessage(r, this);
        }
    }
}
