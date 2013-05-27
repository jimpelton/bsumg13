using System;
using System.Text;
using System.Management;

namespace uGCapture
{
    class UPSController : ReceiverController
    {
        public UPSController(BufferPool<byte> bp, string id, bool receiving = true) 
            : base(bp, id, receiving)
        {
        }

        public override void exHeartBeatMessage(Receiver r, Message m)
        {
            bool found = false;
            ObjectQuery query = new ObjectQuery("Select * FROM Win32_Battery");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection collection = searcher.Get();
            string charge = "0";
            string lifeRemaining = "0";
            string stat = "Disconnected";
            string chargeStatus = "0";
            foreach (ManagementObject mo in collection)
            {
                //charge = mo["EstimatedChargeRemaining"].ToString();
                //chargeStatus = mo["BatteryStatus"].ToString();
                //lifeRemaining = mo["EstimatedRunTime"].ToString();
                //stat = mo["Status"].ToString();

                foreach (PropertyData property in mo.Properties)
                {
                    if (property.Name.Equals("EstimatedChargeRemaining"))
                    {
                        //property.Value//Estimate of the percentage of full charge remaining. 
                        if (property.Value != null)
                            charge = property.Value.ToString();
                    }

                    if (property.Name.Equals("BatteryStatus"))
                    {
                        //property.Value//Estimate of the percentage of full charge remaining. 
                        if (property.Value != null)
                            chargeStatus = property.Value.ToString();
                    }

                    if (property.Name.Equals("EstimatedRunTime"))
                    {
                        //property.Value//Estimate in minutes of the time to battery charge depletion under the present load conditions
                        if (property.Value != null)
                            lifeRemaining = property.Value.ToString();
                    }

                    if (property.Name.Equals("Status"))
                    {
                        found = true;
                        if (property.Value != null)
                        {
                            stat = property.Value.ToString();
                            //property.Value//"OK""Error""Degraded""Unknown""Starting""Stopping""Service""Stressed""NonRecover""No Contact""Lost Comm"
                            CheckedStatusBroadcast(new UPSStatusMessage(this, Status.STAT_GOOD, ErrStr.UPS_STAT_GOOD));
                        }
                        else
                        {
                            CheckedStatusBroadcast(new UPSStatusMessage(this, Status.STAT_ERR, 
                                ErrStr.UPS_ERR_STATUS_PROPERTY_NOT_FOUND));
                        }
                    }
                }
            }
            if (!found)
            {
                CheckedStatusBroadcast(new UPSStatusMessage(this, Status.STAT_ERR, ErrStr.UPS_ERR_NOT_FOUND));
            }

            Buffer<Byte> buffer = BufferPool.PopEmpty();
            
            String output = "UPS \n" + GetUTCMillis().ToString() + " " + stat + " " + 
                charge + " " + lifeRemaining + " " + chargeStatus;

            UTF8Encoding encoding = new UTF8Encoding();
            buffer.setData(encoding.GetBytes(output), BufferType.UTF8_UPS);
            BufferPool.PostFull(buffer);
        }

        protected override bool init()
        {
            ObjectQuery query = new ObjectQuery("Select * FROM Win32_Battery");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection collection = searcher.Get();
            if (collection.Count == 0)
            {
                CheckedStatusBroadcast(new UPSStatusMessage(this, Status.STAT_ERR, ErrStr.UPS_ERR_NOT_FOUND));
            }
            return true;
        }
    }
}
