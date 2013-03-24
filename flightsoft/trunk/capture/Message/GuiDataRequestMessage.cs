using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uGCapture
{
    public class GuiDataRequestMessage : Message
    {     
        public GuiDataRequestMessage(Receiver s)
            : base(s) { ; }

        public override void execute(Receiver r)
        {
            r.exBiteTest(r, this);
        }
    }
}
