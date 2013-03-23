using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using uGCapture;


namespace gui
{
    public class GuiUpdater : Receiver
    {
        Form1 mainform = null;
        public GuiUpdater(Form1 f)
        {         
            mainform = f;
            this.Receiving = true;
            dp.Register(this, "GuiUpdater");
        }

        public void UpdateGUI(object sender, EventArgs e)
        {
            if (this.msgs.Count > 0)
                msgs.Dequeue().execute(this);
            dp.Broadcast();
        }

        public override void exLogMessage(Receiver r, Message m) 
        {
            LogMessage lm = m as LogMessage;
            if (lm != null)
                mainform.DebugOutput(lm.message, lm.severity);
        }
    }
}
