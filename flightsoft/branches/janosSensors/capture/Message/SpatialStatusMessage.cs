

namespace uGCapture
{
    public class SpatialStatusMessage : StatusMessage
    {
        
        public SpatialStatusMessage(Receiver s, Status nstate, ErrStr mes = 0)
            : base(s, nstate, mes)
        {
        }

        public override void execute(Receiver r)
        {
            base.execute(r);
            r.exSpatialStatusMessage(r, this);
        }
    }
}
