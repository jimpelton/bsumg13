using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uGCapture
{
    public class DataRequestMessage : Message
    {
        public DataRequestMessage(Receiver s)
            : base(s)
        {
            ;
        }

        public override void execute(Receiver r)
        {
            r.exDataRequest(r, this);
        }
    }
}
