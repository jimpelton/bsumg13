

namespace uGCapture
{
    public class  AptinaStatusMessage : StatusMessage
    {
        public int WaveLength { get; set; }

        public AptinaStatusMessage(int wl, Receiver s, Status nstate, ErrStr mes = 0)
            : base(s, nstate, mes)
        {
            WaveLength = wl;
        }

        public AptinaStatusMessage(int wl, Receiver s, Status nstate, string mes = "")
            : base(s,nstate,mes)
        {
            WaveLength = wl;
        }

        public override void execute(Receiver r)
        {
            base.execute(r);
            r.exAptinaStatusMessage(r, this);
        }

        public override string ToString()
        {
            return base.ToString() + " " + WaveLength;
        }
    }
}

