

namespace uGCapture
{
    public class NI6008StatusMessage : StatusMessage
    {
        public NI6008StatusMessage(Receiver s, Status nstate, ErrStr mes = 0)
            : base(s, nstate, mes)
        {
        }

        public override void execute(Receiver r)
        {
            base.execute(r);
            r.exNI6008StatusMessage(r, this);
        }
    }
}
