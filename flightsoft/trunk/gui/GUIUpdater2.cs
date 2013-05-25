// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-05-23                                                                      
// ******************************************************************************

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uGCapture;

namespace gui
{
    class GUIUpdater2 : Receiver
    {

        private Status lastAccelState = Status.STAT_ERR;
        private Status lastSpatialState = Status.STAT_ERR;
        private Status lastTemperatureState = Status.STAT_ERR;

        public delegate void debug_output(Message m);

        public debug_output DebugOutput { get; set; }

        public void BroadcastCmd(CommandStr m)
        {
            dp.Broadcast(new CommandMessage(this, m));
        }

        public void BroadcastLogError(string s)
        {
            dp.BroadcastLog(this, s, Status.STAT_ERR);
        }

        public GUIUpdater2(string id, bool receiving = true, bool executing = false) 
            : base(id, receiving, executing)
        {}


        public void init()
        {
            dp.Register(this);
        }

        public override void exLogMessage(Receiver r, Message m)
        {
            DebugOutput(m);
        }

        public override void exStatusMessage(Receiver r, Message m)
        {
            DebugOutput(m);
        }

        //override public void exAptinaStatusMessage(Receiver r, Message m)
        //{
        //    try
        //    {
        //        //todo: convert to a switch
        //        AptinaStatusMessage msg = (AptinaStatusMessage)m;
        //        Status status = msg.Stat;
        //        if (status == Status.STAT_FAIL)
        //        {
        //            switch (msg.WaveLength)
        //            {
        //                case 405:
        //                    CAS.b_Camera_405.BackColor = Color.OrangeRed;
        //                    break;
        //                case 485:
        //                    CAS.b_Camera_485.BackColor = Color.OrangeRed;
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }
        //        else if (status == Status.STAT_ERR)
        //        {
        //            switch (msg.WaveLength)
        //            {
        //                case (405):
        //                    CAS.b_Camera_405.BackColor = Color.OrangeRed;
        //                    break;
        //                case (485):
        //                    CAS.b_Camera_485.BackColor = Color.OrangeRed;
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }
        //        else if (status == Status.STAT_GOOD)
        //        {
        //            switch (msg.WaveLength)
        //            {
        //                case (405):
        //                    CAS.b_Camera_405.BackColor = Color.Black;
        //                    break;
        //                case (485):
        //                    CAS.b_Camera_485.BackColor = Color.Black;
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            switch (msg.WaveLength)
        //            {
        //                case (405):
        //                    CAS.b_Camera_405.BackColor = Color.Salmon;
        //                    break;
        //                case (485):
        //                    CAS.b_Camera_485.BackColor = Color.Salmon;
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }
        //    }
        //    catch (InvalidOperationException e)
        //    {
        //        //the wrong thread operated on something...
        //        dp.BroadcastLog(this, e.Message, 0);
        //    }
        //}

        //override public void exPhidgetsStatusMessage(Receiver r, Message m)
        //{
        //    try
        //    {
        //        PhidgetsStatusMessage msg = (PhidgetsStatusMessage)m;
        //        Status status = msg.Stat;
        //        if (status == Status.STAT_FAIL)
        //        {
        //            CAS.b_Phidgets_1018.BackColor = Color.OrangeRed;
        //        }
        //        else if (status == Status.STAT_DISC)
        //        {
        //            CAS.b_Phidgets_1018.BackColor = Color.OrangeRed;
        //        }
        //        else if (status == Status.STAT_ATCH)
        //        {
        //            CAS.b_Phidgets_1018.BackColor = Color.Yellow;
        //        }
        //        else if (status == Status.STAT_GOOD)
        //        {
        //            long now = DateTime.Now.Ticks;
        //            long dist = now - last1018update;
        //            DateTime x = new DateTime(dist);
        //            if (x.Second > 1)
        //                CAS.b_Phidgets_1018.BackColor = Color.Yellow;
        //            else
        //                CAS.b_Phidgets_1018.BackColor = Color.Black;
        //            last1018update = DateTime.Now.Ticks;
        //        }
        //    }
        //    catch (InvalidOperationException e)
        //    {
        //        //the wrong thread operated on something...
        //        dp.BroadcastLog(this, e.Message, 0);
        //    }
        //}

        //public override void exPhidgetsTempStatusMessage(Receiver r, Message m)
        //{
        //    PhidgetsTempStatusMessage msg = (PhidgetsTempStatusMessage)m;
        //    lastTemperatureState = msg.Stat;
        //}


    }
}
