using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phidgets;
using Phidgets.Events;


//lets handle phidgets accelerometers the same and capture vibration and acceleration data on both.

namespace uGCapture
{
    class AccelerometerController : Receiver, IController
    {
        private Accelerometer accel = null;
        private double[] acceleration = null;
        private double[] vibration = null;

        public AccelerometerController()
        {
            acceleration = new double[3];
            vibration = new double[3];
            dp.BroadcastLog(this, "Accelerometer starting up...", 1);
            openAccel();
        }

        private void openAccel()
        {
            try
            {
                dp.BroadcastLog(this, "Waiting for accelerometer 1 to be found", 0);
                accel = new Accelerometer();
                accel.open();
                accel.waitForAttachment(1000);
                accel.Attach += new AttachEventHandler(accel_Attach);
                accel.Detach += new DetachEventHandler(Sensor_Detach);
                accel.Error += new ErrorEventHandler(Sensor_Error);
                accel.AccelerationChange +=
                    new AccelerationChangeEventHandler(accel_AccelerationChange);

                dp.BroadcastLog(this, "Accelerometer 1 found", 0);
            }
            catch (PhidgetException ex)
            {
                dp.BroadcastLog(this,
                    String.Format("Error waiting for Acceler-o-meter: {0}", ex.Description),
                    100);
            }
        }

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
        void accel_AccelerationChange(object sender, AccelerationChangeEventArgs e)
        {
            vibration[e.Index] = e.Acceleration;
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

    }
}
