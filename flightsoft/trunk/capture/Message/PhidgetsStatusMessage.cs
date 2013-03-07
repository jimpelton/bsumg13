using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uGCapture
{
    class PhidgetsStatusMessage : Message
    {
        PhidgetsStatusMessage(Receiver s)
            : base(s) { ; }

        public override void execute(Receiver r)
        {
            r.exPhidgetsStatus(r);
        }
    }
}
