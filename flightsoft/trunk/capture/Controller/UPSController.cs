using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace uGCapture
{
    class UPSController : ReceiverController
    {



        public UPSController(BufferPool<byte> bp, string id, bool receiving = true, int frame_time = 500) : base(bp, id, receiving, frame_time)
        {
        }

        public override void DoFrame(object source, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public override void exHeartBeatMessage(Receiver r, Message m)
        {
            bool found = false;
            System.Management.ObjectQuery query = new ObjectQuery("Select * FROM Win32_Battery");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

            ManagementObjectCollection collection = searcher.Get();
            string charge = "0";
            string lifeRemaining = "0";
            string stat = "Disconnected";

            foreach (ManagementObject mo in collection)
            {
                foreach (PropertyData property in mo.Properties)
                {
                    if (property.Name.Equals("EstimatedChargeRemaining"))
                    {
                        //property.Value//Estimate of the percentage of full charge remaining. 
                        charge = property.Value.ToString();
                    }

                    if (property.Name.Equals("EstimatedRunTime"))
                    {
                        //property.Value//Estimate in minutes of the time to battery charge depletion under the present load conditions
                        lifeRemaining = property.Value.ToString();
                    }

                    if (property.Name.Equals("Status"))
                    {
                        found = true;
                        stat = property.Value.ToString();
                        //property.Value//"OK""Error""Degraded""Unknown""Starting""Stopping""Service""Stressed""NonRecover""No Contact""Lost Comm"
                        if (property.Value == null || property.Value.Equals("Error"))
                            dp.Broadcast(new UPSStatusMessage(this, StatusStr.STAT_ERR));
                        else
                            dp.Broadcast(new UPSStatusMessage(this, StatusStr.STAT_GOOD));
                    }
                }
            }
            if(!found)
                 dp.Broadcast(new UPSStatusMessage(this, StatusStr.STAT_ERR));

            Buffer<Byte> buffer = BufferPool.PopEmpty();
            String output = "UPS \n" + DateTime.Now.Ticks.ToString() + " " + stat + " " + charge + " " + lifeRemaining;
            UTF8Encoding encoding = new UTF8Encoding();
            buffer.setData(encoding.GetBytes(output), BufferType.UTF8_UPS);
            BufferPool.PostFull(buffer);



        }

        protected override bool init()
        {
            System.Management.ObjectQuery query = new ObjectQuery("Select * FROM Win32_Battery");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection collection = searcher.Get();
            if (collection.Count == 0)
            {
                dp.BroadcastLog(this, "UPS Battery not detected.", 0);
                dp.Broadcast(new UPSStatusMessage(this, StatusStr.STAT_FAIL));
            }
            return true;
        }
    }
}
