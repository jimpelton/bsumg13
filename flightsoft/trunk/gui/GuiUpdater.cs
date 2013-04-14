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
            try
            {
                //while (this.msgs.Count > 0)
                //    msgs.Dequeue().execute(this);
                ExecuteMessageQueue();
            }
            catch (NullReferenceException nel)/// temp fix for monday's test. 5 minutes out.
            {
                Console.WriteLine("UpdateGUI in GuiUpdater.cs threw a null pointer exception at msgs.Dequeue().execute(this)");
 
            }

            dp.Broadcast(new DataRequestMessage(this));
        }

        public override void exLogMessage(Receiver r, Message m) 
        {
            LogMessage lm = m as LogMessage;
            if (lm != null)
                mainform.DebugOutput(lm.message, lm.severity);
        }

        public override void exData(Receiver r, Message m)
        {
            DataPoint dat = new DataPoint();
            dat.NIanaloginputs = ((DataMessage) m).NIanaloginputs;
            dat.UPSstate = ((DataMessage)m).UPSstate;
            dat.VCommstate = ((DataMessage)m).VCommstate;
            dat.WellIntensities = ((DataMessage)m).WellIntensities;
            dat.accel1acceleration = ((DataMessage)m).accel1acceleration;
            dat.accel1rawacceleration = ((DataMessage)m).accel1rawacceleration;
            dat.accel1state = ((DataMessage)m).accel1state;
            dat.accel1vibration = ((DataMessage)m).accel1vibration;
            dat.accel2acceleration = ((DataMessage)m).accel2acceleration;
            dat.accel2rawacceleration = ((DataMessage)m).accel2rawacceleration;
            dat.accel2state = ((DataMessage)m).accel2state;
            dat.accel2vibration = ((DataMessage)m).accel2vibration;
            dat.phidgets888state = ((DataMessage)m).phidgets888state;
            dat.phidgetsanalogInputs = ((DataMessage)m).phidgetsanalogInputs;
            dat.phidgetsdigitalInputs = ((DataMessage)m).phidgetsdigitalInputs;
            dat.phidgetsdigitalOutputs = ((DataMessage)m).phidgetsdigitalOutputs;
            dat.phidgetstempstate = ((DataMessage)m).phidgetstempstate;
            dat.timestamp = ((DataMessage)m).timestamp;          
        }
        public override void exDataRequest(Receiver r, Message m)
        {
            int test = 0;
        }
    }
}
