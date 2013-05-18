

namespace uGCapture
{
    public class SpatialStatusMessage : StatusMessage
    {
        
        public SpatialStatusMessage(Receiver s, StatusStr nstate = StatusStr.STAT_ERR)
            : base(s, nstate)
        {
        }

        public override void execute(Receiver r)
        {
            r.exSpatialStatusMessage(r, this);
        }
    }
}
