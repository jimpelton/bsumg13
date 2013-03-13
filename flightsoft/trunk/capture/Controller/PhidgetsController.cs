using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phidgets;
using Phidgets.Events;


namespace uGCapture
{
    public class PhidgetsController : Receiver, IController
    {

        private Accelerometer     accelCapture       = null;
        private Accelerometer     accelFiltered      = null;
        private TemperatureSensor phidgetTemperature = null;
        private InterfaceKit      phidgets1018       = null;

        private double phidgetTemperature_AmbientTemp = 0;
        private double phidgetTemperature_ProbeTemp = 0;

        private double[] vibration = null;
        private double[] acceleration = null;

        private bool[] digitalInputs = null;
        private int[] analogInputs = null;

        public PhidgetsController()
        {
            dp.BroadcastLog(this, "Phidgets starting up...", 1);

            vibration = new double[3];
            acceleration = new double[3];
            digitalInputs = new bool[8];
            analogInputs = new int[8];

            openAccel1();
            openTempSenser();
            openDAQ();
        }

        private void openAccel1()
        {
                try
                {
                    dp.BroadcastLog(this, "Waiting for accelerometer 1 to be found", 0);
                    accelFiltered = new Accelerometer();
                    accelFiltered.open();
                    accelFiltered.waitForAttachment();
                    accelFiltered.Attach += new AttachEventHandler(accel_Attach);
                    accelFiltered.Detach += new DetachEventHandler(Sensor_Detach);
                    accelFiltered.Error += new ErrorEventHandler(Sensor_Error);
                    accelFiltered.AccelerationChange += 
                        new AccelerationChangeEventHandler(accel_AccelerationChangeAcc);

                    dp.BroadcastLog(this, "Accelerometer 1 found", 0);
                }
                catch (PhidgetException ex)
                {
                    dp.BroadcastLog(this, 
                        String.Format("Error waiting for Acceler-o-meter: %s", ex.Description), 
                        100);	
                }
                
                //accelCapture = new Accelerometer();
                //accelCapture.open();
                //accelCapture.Attach += new AttachEventHandler(accel_Attach);
                //accelCapture.Detach += new DetachEventHandler(Sensor_Detach);
                //accelCapture.Error += new ErrorEventHandler(Sensor_Error);
                //accelCapture.AccelerationChange += new AccelerationChangeEventHandler(accel_AccelerationChangeAcc);
                //CaptureClass.LogDebugMessage("Waiting for accelerometer 2 to be found", 0);
                //accelCapture.waitForAttachment(100000);
                //CaptureClass.LogDebugMessage("Accelerometer 2 found", 0);
        }

        private void openTempSenser()
        {
            try
            {
                dp.BroadcastLog(this, "Searching for temperature sensor", 0);
                phidgetTemperature = new TemperatureSensor();
                phidgetTemperature.open();

                phidgetTemperature.waitForAttachment();

                phidgetTemperature.Attach += new AttachEventHandler(tempSensor_Attach);
                phidgetTemperature.Detach += new DetachEventHandler(Sensor_Detach);
                phidgetTemperature.Error += new ErrorEventHandler(Sensor_Error);
                phidgetTemperature.TemperatureChange += 
                    new TemperatureChangeEventHandler(tempSensor_TemperatureChange);

                phidgetTemperature.thermocouples[0].Sensitivity = 0.02;

                dp.BroadcastLog(this, "Temperature Sensor found", 0);
            }
            catch (PhidgetException ex)
            {
                dp.BroadcastLog
                    (
                        this,
                        String.Format("Error waiting for temperature sensor: %s", ex.Description), 
                        100
                    );
            }
        }

        private void openDAQ()
        {
            try
            {
                dp.BroadcastLog(this, "Searching for 1018 ", 0);
                phidgets1018 = new InterfaceKit();
                phidgets1018.open();
                phidgets1018.Attach += new AttachEventHandler(ifKit_Attach);
                phidgets1018.Detach += new DetachEventHandler(Sensor_Detach);
                phidgets1018.Error += new ErrorEventHandler(Sensor_Error);

                phidgets1018.InputChange += new InputChangeEventHandler(ifKit_InputChange);
                phidgets1018.OutputChange += new OutputChangeEventHandler(ifKit_OutputChange);
                phidgets1018.SensorChange += new SensorChangeEventHandler(ifKit_SensorChange);

                phidgets1018.waitForAttachment(100000);
                dp.BroadcastLog(this, "1018 found", 0);
            }
            catch (PhidgetException ex)
            {                dp.BroadcastLog(this, String.Format("Error opening Phidgets DAQ %s", ex.Description), 6);    
            }
        }


        void Sensor_Detach(object sender, DetachEventArgs e)
        {
            Phidget phid = sender as Phidget;
            if (phid == null) return;
            dp.BroadcastLog(this, String.Format("%s Detached", phid.Name), 5);
        }

        void Sensor_Error(object sender, ErrorEventArgs e)
        {
            Phidget phid = sender as Phidget;
            if (phid == null) return;
            
            dp.BroadcastLog(this, String.Format("%s Error: %s", phid.Name, e.Description), 5);
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
                    String.Format("Error while attaching accelerometer: %s", ex.Description), 5);
            }
        }

        void accel_AccelerationChangeVib(object sender, AccelerationChangeEventArgs e)
        {
            vibration[e.Index] = e.Acceleration;
        }

        void accel_AccelerationChangeAcc(object sender, AccelerationChangeEventArgs e)
        {
            acceleration[e.Index] = e.Acceleration;
        }

        void tempSensor_Attach(object sender, AttachEventArgs e)
        {

        }

        void tempSensor_TemperatureChange(object sender, TemperatureChangeEventArgs e)
        {
            if (e.Index == 0)
                phidgetTemperature_AmbientTemp = e.Temperature;
            else
                phidgetTemperature_ProbeTemp = e.Temperature;

            //debug
            dp.BroadcastLog(this, String.Format("OMG OMG! Temperature Changed! %f", e.Temperature), 2);
        }

        void ifKit_Attach(object sender, AttachEventArgs e)
        {

        }

        void ifKit_InputChange(object sender, InputChangeEventArgs e)
        {
            digitalInputs[e.Index] = e.Value;
        }

        void ifKit_OutputChange(object sender, OutputChangeEventArgs e)
        {
           
        }

        void ifKit_SensorChange(object sender, SensorChangeEventArgs e)
        {
            analogInputs[e.Index] = e.Value;
        }
    }
}
