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


namespace uGCapture
{
    public class AccelerometerPhidgetsController : ReceiverController
    {
        private Accelerometer accel;
        private int SerialNumber;
        private String outputData;

        public AccelerometerPhidgetsController(BufferPool<byte> bp, string id,
                                       int serial, bool receiving = true,
                                       int frame_time = 500)
            : base(bp, id, receiving, frame_time)
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
                
                dp.BroadcastLog(this, "Accelerometer found", 0);
            }
            catch (PhidgetException ex)
            {
                rval = false;
                dp.BroadcastLog( this, 
                    "Error waiting for Acceler-o-meter: "+ex.Description,
                    100 );
            }
            return rval;
        }

        private void accel_Attach(object sender, AttachEventArgs e)
        {
            Accelerometer attached = sender as Accelerometer;
            if (attached == null) return;
            try
            {
                attached.axes[0].Sensitivity = 0;
                attached.axes[1].Sensitivity = 0;
                attached.axes[2].Sensitivity = 0;
            }
            catch (PhidgetException ex)
            {
                dp.BroadcastLog(this,
                                String.Format("Error while attaching accelerometer: {0}",
                                              ex.Description), 100);
            }
        }

        //TODO: remove
        //gets n raw acceleration, a smoothed acceleration, and accumulates the amount of recent noise. (wip)
  /*      private void accel_AccelerationChange(object sender, AccelerationChangeEventArgs e)
        {
            //create a mutex. these have a weak identity.
            lock (rawacceleration)
            {
                rawacceleration[e.Index] = e.Acceleration;
                lock (vibration)
                {                  
                    vibration[e.Index] += Math.Abs(e.Acceleration - rawacceleration[e.Index]);          //accumulates total change per axis
                }
                lock (acceleration)
                {
                    //filtered acceleration.
                    acceleration[e.Index] += rawacceleration[e.Index] * 0.01;//add one tenth of the current acceleration.
                    acceleration[e.Index] *= 0.99;//remove one tenth of the accumulated past acceleration.
                }
            }
        }*/

        private void Sensor_Detach(object sender, DetachEventArgs e)
        {
            Phidget phid = sender as Phidget;
            if (phid == null) return;
            dp.BroadcastLog(this, String.Format("{0} Detached", phid.Name), 5);
        }

        private void Sensor_Error(object sender, ErrorEventArgs e)
        {
            Phidget phid = sender as Phidget;
            if (phid == null) return;

            dp.BroadcastLog(this,
                String.Format("{0} Error: {1}", phid.Name, e.Description), 5);
        }

        public override void DoFrame(object source, ElapsedEventArgs e)
        {
            /*if (accel.Attached)
            {
                Buffer<Byte> buffer = BufferPool.PopEmpty();
                String outputData = "Accel \n";

                outputData += DateTime.Now.Ticks.ToString() + " ";
                for (int i = 0; i < 3; i++)
                    outputData += accel.axes[i].Acceleration + " ";
                lock(acceleration)
                    for (int i = 0; i < 3; i++)
                        outputData += acceleration[i] + " ";
                lock(vibration)
                    for (int i = 0; i < 3; i++)
                        outputData += vibration[i] + " ";

                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                buffer.setData(encoding.GetBytes(outputData), BufferType.UTF8_ACCEL);
                buffer.Text = String.Format(accel.SerialNumber.ToString());
                BufferPool.PostFull(buffer);
            }*/
        }

       
        public override void exHeartBeatMessage(Receiver r, Message m)
        {
            base.exHeartBeatMessage(r, m);
            if (accel.Attached)
            {
                Buffer<Byte> buffer = BufferPool.PopEmpty();
                String output = "Accel \n";

                output += DateTime.Now.Ticks.ToString() + " ";
                output += outputData;
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                buffer.setData(encoding.GetBytes(output), BufferType.UTF8_ACCEL);
                buffer.Text = String.Format(accel.SerialNumber.ToString());
                BufferPool.PostFull(buffer);
                outputData = "";
            }

        }

        public override void exAccumulateMessage(Receiver r, Message m)
        {
            if (accel.Attached)
            {
                for (int i = 0; i < 3; i++)
                    outputData += accel.axes[i].Acceleration + " ";
            }
        }


    }

    public class AccelerometerControllerNotInitializedException : Exception
    {
        public AccelerometerControllerNotInitializedException(string message)
            : base(message)
        {
        }
    }
}