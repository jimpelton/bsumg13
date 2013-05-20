

namespace uGCapture
{
    public class SpatialStatusMessage : StatusMessage
    {
        
        public SpatialStatusMessage(Receiver s, StatusStr nstate = StatusStr.STAT_ERR, string mes = "")
            : base(s, nstate, mes)
        {
        }

        public override void execute(Receiver r)
        {
            r.exSpatialStatusMessage(r, this);
        }
    }
}
