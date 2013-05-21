// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-03-29                                                                      
// ******************************************************************************


using System;
using Phidgets;
using Phidgets.Events;
using System.Text;

namespace uGCapture
{
    public class SpatialAccelController : ReceiverController
    {
        private Spatial accel;
        private int SerialNumber;
        private String outputData;
        public SpatialAccelController(BufferPool<byte> bp, string id, int serial, bool receiving = true) 
            : base(bp, id, receiving)
        {
            SerialNumber = serial;
            outputData = "";
        }

        protected override bool init()
        {
            return openAccel();
        }

        private bool openAccel()
        {
            bool rval = true;
            try
            {
                dp.BroadcastLog(this, "Waiting for accelerometer to be found", 0);
                accel = new Spatial();
                accel.open(SerialNumber);
                accel.waitForAttachment(1000);
                accel.Attach += accel_Attach;
                accel.Detach += Sensor_Detach;
                accel.Error += Sensor_Error;

                dp.Broadcast(new SpatialStatusMessage(this, Status.STAT_GOOD));
                dp.BroadcastLog(this, "Accelerometer found.", 0);
            }
            catch (PhidgetException ex)
            {
                rval = false;

                Console.Error.WriteLine(ex.StackTrace);

                dp.BroadcastLog(this,
                    String.Format("Error waiting for Acceler-o-meter: {0}", ex.Description),
                    100);
                dp.Broadcast(new SpatialStatusMessage(this, Status.STAT_FAIL, ErrStr.INIT_FAIL_PHID_SPTL));
            }

            return rval;
        }

        /******************************************************
         * Phidgets Event Handlers 
         *******************************************************/

        void accel_Attach(object sender, AttachEventArgs e)
        {
            Spatial attached = sender as Spatial;
            if (attached == null) return;
            try
            {
                Phidget phid = sender as Phidget;
                if (phid == null) return;
                //dp.BroadcastLog(this, String.Format("{0} Attached", phid.Name), 5);
                dp.Broadcast(new SpatialStatusMessage(this, Status.STAT_ATCH, ErrStr.PHID_SPTL_STAT_ATCH));
            }
            catch (PhidgetException ex)
            {
                dp.BroadcastLog(this, ex.Message, 100);
                dp.Broadcast(new SpatialStatusMessage(this, Status.STAT_FAIL, ErrStr.PHID_SPTL_STAT_FAIL));
            }
        }


        void Sensor_Detach(object sender, DetachEventArgs e)
        {
            Phidget phid = sender as Phidget;
            if (phid == null) return;
            //dp.BroadcastLog(this, String.Format("{0} Detached", phid.Name), 5);
            dp.Broadcast(new SpatialStatusMessage(this, Status.STAT_DISC, ErrStr.PHID_SPTL_STAT_DISC));
        }

        void Sensor_Error(object sender, ErrorEventArgs e)
        {
            Phidget phid = sender as Phidget;
            if (phid == null) return;

            dp.BroadcastLog(this, String.Format("{0} Error: {1}", phid.Name, e.Description), 5);
            dp.Broadcast(new SpatialStatusMessage(this, Status.STAT_FAIL, ErrStr.PHID_SPTL_STAT_FAIL));
        }


        public override void exHeartBeatMessage(Receiver r, Message m)
        {
            if (accel.Attached)
            {
                Buffer<Byte> buffer = BufferPool.PopEmpty();
                String output = "Accel \n";

                output += GetUTCMillis().ToString() + " ";
                output += outputData;
                UTF8Encoding encoding = new UTF8Encoding();
                buffer.setData(encoding.GetBytes(output), BufferType.UTF8_SPATIAL);
                buffer.Text = accel.SerialNumber.ToString(); 
                BufferPool.PostFull(buffer);
                outputData = "";
                dp.Broadcast(new SpatialStatusMessage(this, Status.STAT_GOOD));
            }
        }

        public override void exAccumulateMessage(Receiver r, Message m)
        {
            try
            {
                if (accel.Attached)
                {
                    for (int i = 0; i < 3; i++)
                        outputData += accel.accelerometerAxes[i].Acceleration + " ";// arguementoutofrangeexception was unhandled with plugging in the accelerometer while it was running.
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                dp.Broadcast(new SpatialStatusMessage(this, Status.STAT_FAIL));
            }
        }


    }

}
