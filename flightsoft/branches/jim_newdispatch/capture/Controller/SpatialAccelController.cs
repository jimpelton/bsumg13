// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-03-29                                                                      
// ******************************************************************************


using System;
using System.Timers;
using Phidgets;
using Phidgets.Events;

namespace uGCapture
{
    public class SpatialAccelController : ReceiverController
    {
        private Spatial accel;
        private double[] rawAcceleration;
        private double[] acceleration;
        private double[] vibration;
        private int SerialNumber;

        public SpatialAccelController(BufferPool<byte> bp, string id, int serial, bool receiving = true, int frame_time = 500) : base(bp, id, receiving, frame_time)
        {
            SerialNumber = serial;
            rawAcceleration = new double[3];
            acceleration = new double[3];
            vibration = new double[3];
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
                accel.SpatialData += accel_AccelerationChange;

                dp.BroadcastLog(this, "Accelerometer found.", 0);
            }
            catch (PhidgetException ex)
            {
                rval = false;

                Console.Error.WriteLine(ex.StackTrace);

                dp.BroadcastLog(this,
                    String.Format("Error waiting for Acceler-o-meter: {0}", ex.Description),
                    100);
            }

            return rval;
        }

        /******************************************************
         * Phidgets Event Handlers 
         *******************************************************/

        void accel_Attach(object sender, AttachEventArgs e)
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
                    String.Format("Error while attaching accelerometer: {0}", ex.Description), 100);
            }
        }

        //gets an raw acceleration, a smoothed acceleration, and accumulates the amount of recent noise. (wip)
        void accel_AccelerationChange(object sender, SpatialDataEventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                double acc = e.spatialData[0].Acceleration[i];
                vibration[i] += Math.Abs(acc - rawAcceleration[i]);
                acceleration[i] = (acceleration[i] + rawAcceleration[i])/2.0;                
                rawAcceleration[i] = acc; 
            }
        }

        void Sensor_Detach(object sender, DetachEventArgs e)
        {
            Phidget phid = sender as Phidget;
            if (phid == null) return;
            dp.BroadcastLog(this, String.Format("{0} Detached", phid.Name), 5);
        }

        void Sensor_Error(object sender, ErrorEventArgs e)
        {
            Phidget phid = sender as Phidget;
            if (phid == null) return;

            dp.BroadcastLog(this, String.Format("{0} Error: {1}", phid.Name, e.Description), 5);
        }


        /******************************************************
        * DoFrame()
        *******************************************************/

        public override void DoFrame(object source, ElapsedEventArgs e)
        {
            if (accel.Attached)
            {

                Buffer<Byte> buffer = BufferPool.PopEmpty();
                String outputData = "Accel \n";

                //TODO: maybe change these concatenations to StringBuilder, 
                //      to ease string copies and GC-ing.
                outputData += DateTime.Now.Ticks.ToString() + " ";
                for (int i = 0; i < 3; i++)
                    outputData += rawAcceleration[i] + " ";

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

    public class SpatialControllerNotInitializedException : Exception
    {
        public SpatialControllerNotInitializedException(string message)
            : base(message)
        {
        }
    }

}
