
using System.Collections.Generic;

namespace uGCapture
{
    public class AccumulateMessage : Message
    {     

        private static IList<ReceiverIdPair> accumulaters = new List<ReceiverIdPair>();

        public AccumulateMessage(Receiver s)
            : base(s) { ; }

        public override void execute(Receiver r)
        {
            r.exAccumulateMessage(r, this);
        }

        public override IList<ReceiverIdPair> GetSpecificReceivers()
        {
            return null;
        }

        public override void AddSpecificReceiver(ReceiverIdPair r) {}
    }
}
