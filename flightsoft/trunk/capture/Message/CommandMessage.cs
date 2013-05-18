using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uGCapture
{
    public class CommandMessage : Message
    {
        private CommandStr cmd = CommandStr.CMD_NONE;

        public CommandStr getCommand() { return cmd; }

        public CommandMessage(Receiver s, CommandStr ncmd)
            : base(s)
        {
            cmd = ncmd;
        }

        public CommandMessage(Receiver s)
            : base(s) { ; }

        public override void execute(Receiver r)
        {
            r.exCommandMessage(r, this);
        }



    }
}
