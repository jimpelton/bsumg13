using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Phidgets;
using Phidgets.Events;


namespace uGCapture 
{
    public class CaptureClass : Receiver
    {       
        
        public static Queue<LogMessage> DebugMessages = null;
      
        PhidgetsController phidgetsController = null;
        NIController niController = null;
        UPSController upsController = null;
        VCommController vCommController = null;

        public CaptureClass()
        {
            if (DebugMessages == null)
            {
                DebugMessages = new Queue<LogMessage>();
            }

            phidgetsController = new PhidgetsController();
            niController = new NIController();
            upsController = new UPSController();
            vCommController = new VCommController();
        }

        public static void LogDebugMessage(String s)
        {
            LogDebugMessage(s, 0);
        }

        public static void LogDebugMessage(String s, int severtity)
        {
            if (DebugMessages != null)
            {
                LogMessage l = new LogMessage(s, severtity);
                DebugMessages.Enqueue(l);
            }
        }





    }
}
