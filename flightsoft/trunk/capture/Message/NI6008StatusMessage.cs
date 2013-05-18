

namespace uGCapture
{
    public class NI6008StatusMessage : StatusMessage
    {
        public NI6008StatusMessage(Receiver s, StatusStr nstate = StatusStr.STAT_ERR)
            : base(s, nstate)
        {
        }

        public override void execute(Receiver r)
        {
            r.exNI6008StatusMessage(r, this);
        }
    }
}
