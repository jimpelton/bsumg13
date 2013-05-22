

namespace uGCapture
{
    public class  AptinaStatusMessage : StatusMessage
    {
        public AptinaStatusMessage(Receiver s, Status nstate, ErrStr mes = 0)
            : base(s, nstate, mes)
        {
        }

        public override void execute(Receiver r)
        {
            r.exAptinaStatusMessage(r, this);
        }
    }
}

