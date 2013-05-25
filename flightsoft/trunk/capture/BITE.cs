using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace uGCapture
{
    class BITE : Receiver
    {
        private Status lastPhidgets1018Status = Status.STAT_ERR;
        private Status lastNI6008Status = Status.STAT_ERR;
        private Status lastPhidgetsTempStatus = Status.STAT_ERR;
        private Status lastAccelStatus = Status.STAT_ERR;
        private Status lastSpatialStatus = Status.STAT_ERR;
        private Status lastUPSStatus = Status.STAT_ERR;
        private Status lastVCommStatus = Status.STAT_ERR;
        private Status lastAptinaStatus = Status.STAT_ERR;



        private CaptureClass capture;
        public BITE(string id, CaptureClass cap, bool receiving = true)
            : base(id, receiving)
        {
            capture = cap;
        }


        private bool waitForLightToGoBright(CommandStr light, int darkLevel, int timeout)
        {
            DataSet<byte> dat = capture.GetLastData();
            UTF8Encoding encoding = new UTF8Encoding();
            dp.Broadcast(new CommandMessage(this, light));
            string lightdat = encoding.GetString(dat.lastData[BufferType.UTF8_PHIDGETS]);
            string[] lightdats = lightdat.Split();
            double l0 = 0, l1 = 0, l2 = 0;
            double total = 0;

            while (total <= darkLevel*4)
            {
                timeout--;
                if (timeout < 0)
                    return false;
                if (lightdats.Length > 8)
                {
                    l0 = double.Parse(lightdats[11]);
                    l1 = double.Parse(lightdats[14]);
                    l2 = double.Parse(lightdats[17]);
                }
                total = l0 + l1 + l2;
                dat = capture.GetLastData();
                lightdat = encoding.GetString(dat.lastData[BufferType.UTF8_PHIDGETS]);
                lightdats = lightdat.Split();
                Thread.Sleep(2);//just a moment
            }
            return true;
        }

        private bool waitForLightToGoDark(CommandStr light, int timeout)
        {
            DataSet<byte> dat = capture.GetLastData();
            UTF8Encoding encoding = new UTF8Encoding();
            dp.Broadcast(new CommandMessage(this, light));
            string lightdat = encoding.GetString(dat.lastData[BufferType.UTF8_PHIDGETS]);
            string[] lightdats = lightdat.Split();
            double l0 = 0, l1 = 0, l2 = 0;
            double total = 10000;

            while (total > 241)
            {
                timeout--;
                if (timeout < 0)
                    return false;
                if (lightdats.Length > 8)
                {
                    l0 = double.Parse(lightdats[11]);
                    l1 = double.Parse(lightdats[14]);
                    l2 = double.Parse(lightdats[17]);
                }
                total = l0 + l1 + l2;
                dat = capture.GetLastData();
                lightdat = encoding.GetString(dat.lastData[BufferType.UTF8_PHIDGETS]);
                lightdats = lightdat.Split();
                Thread.Sleep(2);//wait a moment.
            }
            return true;
        }

        private bool checkCurrentStatusVariables(DataSet<byte> dat)
        {
            bool passed = true;

            if (lastPhidgets1018Status != Status.STAT_GOOD)
            {
                passed = false;
                dp.BroadcastLog(this, "1018 failed BITE test.", 1);
            }
            if (lastPhidgetsTempStatus != Status.STAT_GOOD)
            {
                passed = false;
                dp.BroadcastLog(this, "Temperature Sensor failed BITE test.", 1);
            }
            if (lastAccelStatus != Status.STAT_GOOD)
            {
                passed = false;
                dp.BroadcastLog(this, "Accelerometer failed BITE test.", 1);
            }
            if (lastAptinaStatus != Status.STAT_GOOD)
            {
                passed = false;
                dp.BroadcastLog(this, "Cameras failed BITE test.", 1);
            }
            if (lastSpatialStatus != Status.STAT_GOOD)
            {
                passed = false;
                dp.BroadcastLog(this, "Spatial failed BITE test.", 1);
            }
            if (lastUPSStatus != Status.STAT_GOOD)
            {
                passed = false;
                dp.BroadcastLog(this, "UPS failed BITE test.", 1);
            }
            if (lastVCommStatus != Status.STAT_GOOD)
            {
                passed = false;
                dp.BroadcastLog(this, "Barometer failed BITE test.", 1);
            }



            return passed;
        }
/*
        private bool checkLightSensorOutputs(DataSet<byte> dat, int timeout)
        {

        }
        */




        private bool executeLightTest(DataSet<byte> dat,int timeout)
        {
            long curtime = DateTime.Now.Ticks/10000;//in milliseconds
            UTF8Encoding encoding = new UTF8Encoding();
            dp.Broadcast(new CommandMessage(this, CommandStr.CMD_NI_LIGHT_1_1_OFF));
            dp.Broadcast(new CommandMessage(this, CommandStr.CMD_NI_LIGHT_1_2_OFF));
            dp.Broadcast(new CommandMessage(this, CommandStr.CMD_NI_LIGHT_2_1_OFF));
            dp.Broadcast(new CommandMessage(this, CommandStr.CMD_NI_LIGHT_2_2_OFF));
            bool passed = true;
            try
            {
                string lightdat = encoding.GetString(dat.lastData[BufferType.UTF8_PHIDGETS]);
                string[] lightdats = lightdat.Split();
                double l0 = 0, l1 = 0, l2 = 0;
                double total = 0;
                double darkLevel = 0;
                //get the base light level. Should be near zero with the lights currently off.

                //block for a moment while we wait for the light level to decrease.
                //it reads 80 with tape over it.. so once it drops below 80 * 3 we can say its out.
                while (total > 241)
                {
                    timeout--;
                    if (timeout < 0)
                        return false;
                    if (lightdats.Length > 8)
                    {
                        l0 = double.Parse(lightdats[11]);
                        l1 = double.Parse(lightdats[14]);
                        l2 = double.Parse(lightdats[17]);
                    }
                    darkLevel = total = l0 + l1 + l2;
                    dat = capture.GetLastData();
                    lightdat = encoding.GetString(dat.lastData[BufferType.UTF8_PHIDGETS]);
                    lightdats = lightdat.Split();
                    Thread.Sleep(2);//errrrrmmoment
                }
                
                //now we test light 1 1
                if (waitForLightToGoBright(CommandStr.CMD_NI_LIGHT_1_1_ON, (int)darkLevel, timeout))
                {
                    dp.BroadcastLog(this, "Light one one went bright", 1);
                }
                else
                {
                    dp.BroadcastLog(this, "Light one one failed to go bright", 1);
                    passed = false;
                }

                if (waitForLightToGoDark(CommandStr.CMD_NI_LIGHT_1_1_OFF, timeout))
                {
                    dp.BroadcastLog(this, "Light one one went dark", 1);
                }
                else
                {
                    dp.BroadcastLog(this, "Light one one failed to go dark", 1);
                    passed = false;
                }

                //now we test light 1 2
                if (waitForLightToGoBright(CommandStr.CMD_NI_LIGHT_1_2_ON, (int)darkLevel, timeout))
                {
                    dp.BroadcastLog(this, "Light one two went bright", 1);
                }
                else
                {
                    dp.BroadcastLog(this, "Light one two failed to go bright", 1);
                    passed = false;
                }

                if (waitForLightToGoDark(CommandStr.CMD_NI_LIGHT_1_2_OFF, timeout))
                {
                    dp.BroadcastLog(this, "Light one two went dark", 1);
                }
                else
                {
                    dp.BroadcastLog(this, "Light one two failed to go dark", 1);
                    passed = false;
                }

                //now we test light 2 1
                if (waitForLightToGoBright(CommandStr.CMD_NI_LIGHT_2_1_ON, (int)darkLevel, timeout))
                {
                    dp.BroadcastLog(this, "Light two one went bright", 1);
                }
                else
                {
                    dp.BroadcastLog(this, "Light two one failed to go bright", 1);
                    passed = false;
                }

                if (waitForLightToGoDark(CommandStr.CMD_NI_LIGHT_2_1_OFF, timeout))
                {
                    dp.BroadcastLog(this, "Light two one went dark", 1);
                }
                else
                {
                    dp.BroadcastLog(this, "Light two one failed to go dark", 1);
                    passed = false;
                }

                //now we test light 2 2
                if (waitForLightToGoBright(CommandStr.CMD_NI_LIGHT_2_2_ON, (int)darkLevel, timeout))
                {
                    dp.BroadcastLog(this, "Light two two went bright", 1);
                }
                else
                {
                    dp.BroadcastLog(this, "Light two two failed to go bright", 1);
                    passed = false;
                }

                if (waitForLightToGoDark(CommandStr.CMD_NI_LIGHT_2_2_OFF, timeout))
                {
                    dp.BroadcastLog(this, "Light two two went dark", 1);
                }
                else
                {
                    dp.BroadcastLog(this, "Light two two failed to go dark", 1);
                    passed = false;
                }
            }
            catch (FormatException e)
            {
                dp.BroadcastLog(this, "BITE ran into a malformed data packet in the staging buffer during the light test.", 1);
                return false;
                //a malformed packet has arrived. Tremble in fear.
            }
            return passed;
        }

        public override void exBiteTestMessage(Receiver r, Message m)
        {
            DataSet<byte> dat = capture.GetLastData();
            if (dat != null)
            {
                executeLightTest(dat, 1000);
                checkCurrentStatusVariables(dat);
            }
        }



        public override void exUPSStatusMessage(Receiver r, Message m)
        {
            UPSStatusMessage a = (UPSStatusMessage)m;
            lastUPSStatus = a.getState();
        }
        public override void exAccelStatusMessage(Receiver r, Message m)
        {
            AccelStatusMessage a = (AccelStatusMessage)m;
            lastAccelStatus = a.getState();
        }
        public override void exVcommStatusMessage(Receiver r, Message m)
        {
            VcommStatusMessage a = (VcommStatusMessage)m;
            lastVCommStatus = a.getState();
        }
        public override void exNI6008StatusMessage(Receiver r, Message m)
        {
            NI6008StatusMessage a = (NI6008StatusMessage)m;
            lastNI6008Status = a.getState();
        }
        public override void exAptinaStatusMessage(Receiver r, Message m)
        {
            AptinaStatusMessage a = (AptinaStatusMessage)m;
            lastAptinaStatus = a.getState();
        }
        public override void exSpatialStatusMessage(Receiver r, Message m)
        {
            SpatialStatusMessage a = (SpatialStatusMessage)m;
            lastSpatialStatus = a.getState();
        }
        public override void exPhidgetsStatusMessage(Receiver r, Message m)
        {
            PhidgetsStatusMessage a = (PhidgetsStatusMessage)m;
            lastPhidgets1018Status = a.getState();
        }
        public override void exPhidgetsTempStatusMessage(Receiver r, Message m)
        {
            PhidgetsTempStatusMessage a = (PhidgetsTempStatusMessage)m;
            lastPhidgetsTempStatus = a.getState();
        }
        

    }
}
