

namespace uGCapture
{
    public class VcommStatusMessage : StatusMessage
    {
        public VcommStatusMessage(Receiver s, Status nstate, ErrStr mes = 0)
            : base(s, nstate, mes)
        {
        }

        public override void execute(Receiver r)
        {
            base.execute(r);
            r.exVcommStatusMessage(r, this);
        }
    }
}
