using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uGCapture
{
    public class ReceiverCleanupMessage : Message
    {
        public ReceiverCleanupMessage(Receiver sender) : base(sender)
        {
        }

        public override void execute(Receiver r)
        {
            r.exReceiverCleanUpMessage(r, this);
        }
    }
}
