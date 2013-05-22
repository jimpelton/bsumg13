
namespace uGCapture
{
    public class AccumulateMessage : Message
    {     
        public AccumulateMessage(Receiver s)
            : base(s) { ; }

        public override void execute(Receiver r)
        {
            r.exAccumulateMessage(r, this);
        }
    }
}
