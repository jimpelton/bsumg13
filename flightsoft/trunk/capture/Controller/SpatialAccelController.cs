// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-03-29                                                                      
// ******************************************************************************


using System;
using Phidgets;
using Phidgets.Events;
using System.Text;
using System.Threading;

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
                //dp.BroadcastLog(this, "Waiting for accelerometer to be found", 0);
                accel = new Spatial();
                accel.open(SerialNumber);
                accel.waitForAttachment(1000);
                accel.Attach += accel_Attach;
                accel.Detach += Sensor_Detach;
                accel.Error += Sensor_Error;

                CheckedStatusBroadcast(new SpatialStatusMessage(this, Status.STAT_GOOD, ErrStr.INIT_OK_PHID_SPTL));
                //dp.BroadcastLog(this, "Accelerometer found.", 0);
            }
            catch (PhidgetException ex)
            {
                rval = false;

                Console.Error.WriteLine(ex.StackTrace);

                //dp.BroadcastLog(this,
                //    String.Format("Error waiting for Acceler-o-meter: {0}", ex.Description),
                //    100);
                CheckedStatusBroadcast(new SpatialStatusMessage(this, Status.STAT_FAIL, ErrStr.INIT_FAIL_PHID_SPTL));
            }

            return rval;
        }

        /******************************************************
         * Phidgets Event Handlers 
         *******************************************************/

        void accel_Attach(object sender, AttachEventArgs e)
        {
            try
            {
                CheckedStatusBroadcast(new SpatialStatusMessage(this, Status.STAT_ATCH, ErrStr.PHID_SPTL_STAT_ATCH));
                this.IsReceiving = true;
            }
            catch (PhidgetException ex)
            {
                dp.BroadcastLog(this, Status.STAT_FAIL, ex.Message);
                CheckedStatusBroadcast(new SpatialStatusMessage(this, Status.STAT_FAIL, ErrStr.PHID_SPTL_STAT_ERR));
            }
        }


        void Sensor_Detach(object sender, DetachEventArgs e)
        {
            CheckedStatusBroadcast(new SpatialStatusMessage(this, Status.STAT_DISC, ErrStr.PHID_SPTL_STAT_DISC));
            this.IsReceiving = false;
        }

        void Sensor_Error(object sender, ErrorEventArgs e)
        {
            CheckedStatusBroadcast(new SpatialStatusMessage(this, Status.STAT_ERR, ErrStr.PHID_SPTL_STAT_ERR));
        }


        public override void exHeartBeatMessage(Receiver r, Message m)
        {
            if (accel.Attached)
            {
                dp.Broadcast(new AccelerometerStatusMessage(this, Status.STAT_GOOD, 0, accel.accelerometerAxes[0].Acceleration));

                if (outputData.Length > MAX_FILE_LENGTH)
                {
                    Buffer<Byte> buffer = BufferPool.PopEmpty();
                    String output = "Accel \n";

                    output += " " + outputData;
                    UTF8Encoding encoding = new UTF8Encoding();
                    buffer.setData(encoding.GetBytes(output), BufferType.UTF8_SPATIAL);
                    buffer.Text = accel.SerialNumber.ToString();
                    BufferPool.PostFull(buffer);
                    outputData = "";
                    CheckedStatusBroadcast(new SpatialStatusMessage(this, Status.STAT_GOOD, ErrStr.PHID_SPTL_STAT_OK));
                }
            }
        }

        public override void exAccumulateMessage(Receiver r, Message m)
        {
            try
            {
                if (accel.Attached)
                {

                    outputData += GetUTCMillis().ToString() + " ";
                    for (int i = 0; i < 3; i++)
                        outputData += accel.accelerometerAxes[i].Acceleration + " ";// arguementoutofrangeexception was unhandled with plugging in the accelerometer while it was running.
                    outputData += "\n";

                }
            }
            catch (PhidgetException e)
            {
                CheckedStatusBroadcast(new SpatialStatusMessage(this, Status.STAT_FAIL));
            }
            catch (ArgumentOutOfRangeException)
            {
                CheckedStatusBroadcast(new SpatialStatusMessage(this, Status.STAT_FAIL, ErrStr.PHID_SPTL_STAT_ERR));
            }
        }
    }
}
