// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-03-29                                                                      
// ******************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phidgets;
using Phidgets.Events;
using System.Timers;
using System.Threading;

namespace uGCapture
{
    public class AccelerometerPhidgetsController : ReceiverController
    {
        private Accelerometer accel;
        private int SerialNumber;
        private String outputData;

        public AccelerometerPhidgetsController(BufferPool<byte> bp, string id,
                                       int serial, bool receiving = true)
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
                accel = new Accelerometer();
                accel.open(SerialNumber);
                accel.waitForAttachment(1000);
                accel.Attach += accel_Attach;
                accel.Detach += Sensor_Detach;
                accel.Error += Sensor_Error;

                dp.BroadcastLog(this, ErrStr.INIT_OK_PHID_ACCEL, Status.STAT_GOOD);
                dp.Broadcast(new AccelStatusMessage(this, Status.STAT_GOOD, 
                    ErrStr.PHID_ACCL_STAT_OK));
            }
            catch (PhidgetException ex)
            {
                rval = false;
                dp.BroadcastLog( this, 
                    "Error waiting for Accelerometer: "+ex.Description,
                    100 );
                dp.Broadcast(new AccelStatusMessage(this,  Status.STAT_FAIL, 
                    ErrStr.INIT_FAIL_PHID_ACCEL));
            }
            return rval;
        }

        private void accel_Attach(object sender, AttachEventArgs e)
        {
            Accelerometer attached = sender as Accelerometer;
            if (attached == null) return;
            try
            {
                dp.BroadcastLog(this, String.Format("{0} Attached", attached.Name), 5);
                dp.Broadcast(new AccelStatusMessage(this, Status.STAT_ATCH, ErrStr.PHID_ACCL_STAT_ATCH));
                attached.axes[0].Sensitivity = 0;
                attached.axes[1].Sensitivity = 0;
                attached.axes[2].Sensitivity = 0;
                this.IsReceiving = true;
            }
            catch (PhidgetException ex)
            {
                dp.BroadcastLog(this, Status.STAT_FAIL,
                    "Error while attaching accelerometer: {0}", ex.Description);
                                              
                //we are probably already holding a bad state. but lets make sure
                dp.Broadcast(new AccelStatusMessage(this, Status.STAT_FAIL, ErrStr.PHID_ACCL_STAT_ERR));
            }
            catch (IndexOutOfRangeException Err)
            {
                dp.Broadcast(new PhidgetsTempStatusMessage(this, Status.STAT_DISC,
                    ErrStr.PHID_TEMP_STAT_FAIL));
            }
            catch (ArgumentOutOfRangeException Err)
            {
                dp.Broadcast(new PhidgetsTempStatusMessage(this, Status.STAT_DISC,
                    ErrStr.PHID_TEMP_STAT_FAIL));
            }
        }

        private void Sensor_Detach(object sender, DetachEventArgs e)
        {
            Phidget phid = sender as Phidget;
            if (phid == null) return;
            dp.BroadcastLog(this, String.Format("{0} Detached", phid.Name), 5);
            dp.Broadcast(new AccelStatusMessage(this, Status.STAT_DISC));
            this.IsReceiving = false;
        }

        private void Sensor_Error(object sender, ErrorEventArgs e)
        {
            Phidget phid = sender as Phidget;
            if (phid == null) return;

            dp.BroadcastLog(this, Status.STAT_ERR,
                String.Format("{0} Error: {1}", phid.Name, e.Description));
            dp.Broadcast(new AccelStatusMessage(this, Status.STAT_FAIL, 
                ErrStr.PHID_ACCL_STAT_ERR));
        }

        public override void exHeartBeatMessage(Receiver r, Message m)
        {
            if (accel.Attached)
            {
                Buffer<Byte> buffer = BufferPool.PopEmpty();
                String output = "Accel \n";

                output += GetUTCMillis().ToString() + " " + outputData;
                //output += outputData;
                UTF8Encoding encoding = new UTF8Encoding();
                buffer.setData(encoding.GetBytes(output), BufferType.UTF8_ACCEL);
                buffer.Text = accel.SerialNumber.ToString();
                BufferPool.PostFull(buffer);
                outputData = "";
                dp.Broadcast(new AccelStatusMessage(this, Status.STAT_GOOD, 
                    ErrStr.PHID_ACCL_STAT_OK));
            }
        }

        public override void exAccumulateMessage(Receiver r, Message m)
        {
            if (accel.Attached)
                {
                try
                {
                    for (int i = 0; i < 3; i++)
                        outputData += accel.axes[i].Acceleration + " ";
                }
                catch (ArgumentOutOfRangeException e)
                {
                    dp.Broadcast(new AccelStatusMessage(this, Status.STAT_FAIL, ErrStr.PHID_ACCL_STAT_ERR));
                }
                catch (IndexOutOfRangeException Err)
                {
                    dp.Broadcast(new AccelStatusMessage(this, Status.STAT_DISC,
                        ErrStr.PHID_TEMP_STAT_FAIL));
                }
            }
        }
    }


}