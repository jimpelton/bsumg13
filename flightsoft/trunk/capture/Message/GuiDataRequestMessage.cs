using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uGCapture.Message
{
    class GuiDataRequestMessage : Message
    {     
        public GuiDataRequestMessage(Receiver s)
            : base(s) { ; }

        public override void execute(Receiver r)
        {
            r.exBiteTest(r, this);
        }
    }
}
