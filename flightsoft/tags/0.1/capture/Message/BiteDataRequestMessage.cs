using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uGCapture
{
    public class BiteDataRequestMessage : Message
    {     
        public BiteDataRequestMessage(Receiver s)
            : base(s) { ; }

        public override void execute(Receiver r)
        {
            r.exBiteTest(r, this);
        }
    }
}
