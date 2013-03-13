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
        
        private Queue<Message> messages;
        private Dispatch dispatch;
        private Receiver[] controllers;

        //PhidgetsController phidgetsController = null;
        //NIController niController = null;
        //UPSController upsController = null;
        //VCommController vCommController = null;

        public CaptureClass()
        {
            messages = new Queue<Message>();

            dispatch = Dispatch.Instance();
            controllers = new Receiver[4];
            controllers[0] = new PhidgetsController();
            controllers[1] = new NIController();
            controllers[2] = new UPSController();
            controllers[3] = new VCommController();
        }

        public void LogDebugMessage(String s)
        {
            LogDebugMessage(s, 0);
        }

        public void LogDebugMessage(String s, int severtity)
        {
            Dispatch.Instance().Broadcast
            (
                new LogMessage(this, s, severtity)
            );

            ////if (DebugMessages != null)
            ////{
            //    //LogMessage l = new LogMessage(this, s, severtity);
            //    //DebugMessages.Enqueue(l);
            //}
        }





    }
}
