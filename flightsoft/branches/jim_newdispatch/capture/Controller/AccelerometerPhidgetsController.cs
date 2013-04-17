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
        private double[] rawacceleration;
        private double[] acceleration;
        private double[] vibration;
        private int SerialNumber;

        public AccelerometerPhidgetsController(BufferPool<byte> bp, string id,
                                       int serial, bool receiving = true,
                                       int frame_time = 500)
            : base(bp, id, receiving, frame_time)
        {
            SerialNumber = serial;
            rawacceleration = new double[3];
            acceleration = new double[3];
            vibration = new double[3];
        }


        //public AccelerometerPhidgetsController(BufferPool<byte> bp, int serial)
        //    : base(bp)
        //{
        //    SerialNumber = serial;
        //    rawacceleration = new double[3];
        //    acceleration = new double[3];
        //    vibration = new double[3];
        //}

        public override void init()
        {
            openAccel();
        }


        private void openAccel()
        {
            try
            {
                dp.BroadcastLog(this, "Waiting for accelerometer to be found", 0);
                accel = new Accelerometer();
                accel.open(SerialNumber);
                accel.waitForAttachment(1000);
                accel.Attach += new AttachEventHandler(accel_Attach);
                accel.Detach += new DetachEventHandler(Sensor_Detach);
                accel.Error += new ErrorEventHandler(Sensor_Error);
                accel.AccelerationChange +=
                    new AccelerationChangeEventHandler(accel_AccelerationChange);

                dp.BroadcastLog(this, "Accelerometer found", 0);
            }
            catch (PhidgetException ex)
            {
                dp.BroadcastLog(this,
                                String.Format("Error waiting for Acceler-o-meter: {0}",
                                              ex.Description),
                                100);
            }
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

        //gets n raw acceleration, a smoothed acceleration, and accumulates the amount of recent noise. (wip)
        private void accel_AccelerationChange(object sender, AccelerationChangeEventArgs e)
        {
            vibration[e.Index] += Math.Abs(e.Acceleration - rawacceleration[e.Index]);
                //accumulates total change per axis
            acceleration[e.Index] = (acceleration[e.Index] + rawacceleration[e.Index])/2.0;
                //need a better filter than this
            rawacceleration[e.Index] = e.Acceleration; //what we will use the most
        }

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
            if (accel.Attached)
            {
                Buffer<Byte> buffer = BufferPool.PopEmpty();
                String outputData = "Accel \n";

                outputData += DateTime.Now.Ticks.ToString() + " ";
                for (int i = 0; i < 3; i++)
                    outputData += rawacceleration[i] + " ";

                for (int i = 0; i < 3; i++)
                    outputData += acceleration[i] + " ";

                for (int i = 0; i < 3; i++)
                    outputData += vibration[i] + " ";

                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                buffer.setData(encoding.GetBytes(outputData), BufferType.UTF8_ACCEL);
                buffer.Text = String.Format(accel.SerialNumber.ToString());
                BufferPool.PostFull(buffer);
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