using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uGCapture
{
    public abstract class StatusMessage : Message
    {
        private StatusStr state;
        public  StatusStr getState() { return state; }

        protected StatusMessage(Receiver s, StatusStr nstate)
            : base(s)
        {
            state = nstate;
        }

        public override abstract void execute(Receiver r);
    }
}
