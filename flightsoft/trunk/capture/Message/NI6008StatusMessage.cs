

namespace uGCapture
{
    public class NI6008StatusMessage : StatusMessage
    {
        public NI6008StatusMessage(Receiver s, StatusStr nstate = StatusStr.STAT_ERR, string mes = "")
            : base(s, nstate, mes)
        {
        }

        public override void execute(Receiver r)
        {
            r.exNI6008StatusMessage(r, this);
        }
    }
}
