using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phidgets;
using Phidgets.Events;


namespace uGCapture
{
    public class PhidgetsController : Receiver
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
            vibration = new double[3];
            acceleration = new double[3];
            digitalInputs = new bool[8];
            analogInputs = new int[8];
            CaptureClass.LogDebugMessage("Phidgets starting up...", 1);

            try
            {

                accelFiltered = new Accelerometer();
                accelFiltered.open();
                CaptureClass.LogDebugMessage("Waiting for accelerometer 1 to be found", 0);
                accelFiltered.waitForAttachment();
                CaptureClass.LogDebugMessage("Accelerometer 1 found", 0);
                accelFiltered.Attach += new AttachEventHandler(accel_Attach);
                accelFiltered.Detach += new DetachEventHandler(Sensor_Detach);
                accelFiltered.Error += new ErrorEventHandler(Sensor_Error);
                accelFiltered.AccelerationChange += new AccelerationChangeEventHandler(accel_AccelerationChangeAcc);

                
                //accelCapture = new Accelerometer();
                //accelCapture.open();
                //accelCapture.Attach += new AttachEventHandler(accel_Attach);
                //accelCapture.Detach += new DetachEventHandler(Sensor_Detach);
                //accelCapture.Error += new ErrorEventHandler(Sensor_Error);
                //accelCapture.AccelerationChange += new AccelerationChangeEventHandler(accel_AccelerationChangeAcc);
                //CaptureClass.LogDebugMessage("Waiting for accelerometer 2 to be found", 0);
                //accelCapture.waitForAttachment(100000);
                //CaptureClass.LogDebugMessage("Accelerometer 2 found", 0);

                phidgetTemperature = new TemperatureSensor();
                phidgetTemperature.open();
                CaptureClass.LogDebugMessage("Waiting for Temperature Sensor to be found", 0);
                phidgetTemperature.waitForAttachment();
                CaptureClass.LogDebugMessage("Temperature Sensor found", 0);
                phidgetTemperature.Attach += new AttachEventHandler(tempSensor_Attach);
                phidgetTemperature.Detach += new DetachEventHandler(Sensor_Detach);
                phidgetTemperature.Error += new ErrorEventHandler(Sensor_Error);
                phidgetTemperature.TemperatureChange += new TemperatureChangeEventHandler(tempSensor_TemperatureChange);

                phidgetTemperature.thermocouples[0].Sensitivity = 0.02;

                phidgets1018 = new InterfaceKit();
                phidgets1018.open();
                phidgets1018.Attach += new AttachEventHandler(ifKit_Attach);
                phidgets1018.Detach += new DetachEventHandler(Sensor_Detach);
                phidgets1018.Error += new ErrorEventHandler(Sensor_Error);

                phidgets1018.InputChange += new InputChangeEventHandler(ifKit_InputChange);
                phidgets1018.OutputChange += new OutputChangeEventHandler(ifKit_OutputChange);
                phidgets1018.SensorChange += new SensorChangeEventHandler(ifKit_SensorChange);
                
                CaptureClass.LogDebugMessage("Waiting for 1018 to be found", 0);
                phidgets1018.waitForAttachment(100000);
                CaptureClass.LogDebugMessage("1018 found", 0);



            }
            catch (PhidgetException ex)
            {
                CaptureClass.LogDebugMessage("Error (startup catch) " + ex.Description, 6);             
            }
        }

        void Sensor_Detach(object sender, DetachEventArgs e)
        {
            Phidget phid = (Phidget)sender;
            CaptureClass.LogDebugMessage("" + phid.Name + " Detached", 5);
        }

        void Sensor_Error(object sender, ErrorEventArgs e)
        {
            Phidget phid = (Phidget)sender;
            CaptureClass.LogDebugMessage("" + phid.Name + " Error", 5);
        }

        void accel_Attach(object sender, AttachEventArgs e)
        {
            Accelerometer attached = (Accelerometer)sender;


            try
            {
            attached.axes[0].Sensitivity = 0;
            attached.axes[1].Sensitivity = 0;
            attached.axes[2].Sensitivity = 0;                       }
            catch (PhidgetException ex)
            {
                CaptureClass.LogDebugMessage("Error while attaching accelerometer", 5);
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
            CaptureClass.LogDebugMessage("Temperature Changed! "+e.Temperature, 2);
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
