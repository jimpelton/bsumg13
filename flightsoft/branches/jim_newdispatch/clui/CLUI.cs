using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uGCapture;

namespace clui
{
    class CLUI : Receiver
    {
        private CaptureClass cc = new CaptureClass();

        public CLUI() 
        {
        }

        public override void exLogMessage(Receiver r, Message m)
        {
            
        }
    }
}
