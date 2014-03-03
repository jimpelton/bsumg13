using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uGCapture
{
    class HeartBeatMessage : Message
    {
        public HeartBeatMessage(Receiver sender)
            : base(sender) { ; }

        public override void execute(Receiver r)
        {
            r.exHeartBeatMessage(r, this);
        }
    }
}
