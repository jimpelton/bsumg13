

namespace uGCapture
{
    public class  AccelStatusMessage : StatusMessage
    {

        public AccelStatusMessage(Receiver s, Status nstate, ErrStr mes = 0)
            : base(s, nstate, mes)
        {
        }

        public override void execute(Receiver r)
        {
            r.exAccelStatusMessage(r, this);
        }
    }
}
