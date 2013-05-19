using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uGCapture
{
    class BITE : Receiver
    {
        private CaptureClass capture;
        public BITE(string id, bool receiving = true)
            : base(id, receiving)
        {

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

            while (total < darkLevel*2)
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
                dat = capture.GetLastData();
                lightdat = encoding.GetString(dat.lastData[BufferType.UTF8_PHIDGETS]);
                lightdats = lightdat.Split();
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
            double total = 0;

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
                dat = capture.GetLastData();
                lightdat = encoding.GetString(dat.lastData[BufferType.UTF8_PHIDGETS]);
                lightdats = lightdat.Split();
            }
            return true;
        }




        private bool executeLightTest(DataSet<byte> dat,int timeout)
        {
            long curtime = DateTime.Now.Ticks/10000;//in milliseconds
            UTF8Encoding encoding = new UTF8Encoding();
            dp.Broadcast(new CommandMessage(this, CommandStr.CMD_NI_LIGHT_1_1_OFF));
            dp.Broadcast(new CommandMessage(this, CommandStr.CMD_NI_LIGHT_1_2_OFF));
            dp.Broadcast(new CommandMessage(this, CommandStr.CMD_NI_LIGHT_2_1_OFF));
            dp.Broadcast(new CommandMessage(this, CommandStr.CMD_NI_LIGHT_2_2_OFF));
            
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
                }
                
                //now we test light 1 1
                if(waitForLightToGoBright(CommandStr.CMD_NI_LIGHT_1_1_ON, (int)darkLevel, timeout))
                {

                }
                else
                {

                }


            }
            catch (FormatException e)
            {
                dp.BroadcastLog(this, "BITE ran into a malformed data packet in the staging buffer during the light test.", 1);
                return false;
                //a malformed packet has arrived. Tremble in fear.
            }


            return true;
        }

        public override void exBiteTestMessage(Receiver r, Message m)
        {
            DataSet<byte> dat = capture.GetLastData();
            if (dat != null)
            {
                executeLightTest(dat, 1000);

            }
        }
    }
}
