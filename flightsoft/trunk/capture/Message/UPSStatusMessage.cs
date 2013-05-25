

namespace uGCapture
{
    public class UPSStatusMessage : StatusMessage
    {
        public UPSStatusMessage(Receiver s, Status nstate, ErrStr mes = 0)
            : base(s, nstate, mes)
        {
        }

        public override void execute(Receiver r)
        {
            base.execute(r);
            r.exUPSStatusMessage(r, this);
        }
    }
}
