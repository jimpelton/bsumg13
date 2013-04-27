using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uGCapture
{
    class AccumulateMessage : Message
    {     
        public AccumulateMessage(Receiver s)
            : base(s) { ; }

        public override void execute(Receiver r)
        {
            r.exAccumulateMessage(r, this);
        }
    }
}
