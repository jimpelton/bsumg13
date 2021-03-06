﻿using System;
using System.Threading;
using System.Timers;
using Phidgets;
using Phidgets.Events;


namespace uGCapture
{
public class PhidgetsController : ReceiverController
{

    private TemperatureSensor phidgetTemperature = null;
    private InterfaceKit      phidgets1018       = null;

    // private double phidgetTemperature_AmbientTemp = 0;
    // private double phidgetTemperature_ProbeTemp = 0;

    // private bool[] digitalInputs; // = null;
    // private bool[] digitalOutputs; // = null;
    // private int[] analogInputs; // = null;

    public PhidgetsController(BufferPool<byte> bp) 
        : base(bp)
    {
        //digitalInputs = new bool[8];
        //digitalOutputs = new bool[8];
        //analogInputs = new int[8];     
    }

            
    public override void init()
    {
        openTempSenser();
        openDAQ();
        dp.BroadcastLog(this, "Phidgets started up...", 1);
    }

    private void openTempSenser()
    {
        try
        {
            dp.BroadcastLog(this, "Searching for temperature sensor", 0);
            phidgetTemperature = new TemperatureSensor();
            phidgetTemperature.open();

            phidgetTemperature.waitForAttachment(1000);

            phidgetTemperature.Attach += tempSensor_Attach;
            phidgetTemperature.Detach += Sensor_Detach;
            phidgetTemperature.Error += Sensor_Error;


            //removed due to query in doFrame.
            //phidgetTemperature.TemperatureChange += tempSensor_TemperatureChange;

            phidgetTemperature.thermocouples[0].Sensitivity = 0.001;
            

            dp.BroadcastLog(this, "Temperature Sensor found", 0);
        }
        catch (PhidgetException ex)
        {
            dp.BroadcastLog
                (
                    this,
                    String.Format("Error waiting for temperature sensor: {0}", ex.Description), 
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
            phidgets1018.Attach += ifKit_Attach;
            phidgets1018.Detach += Sensor_Detach;
            phidgets1018.Error += Sensor_Error;
            
            //removed due to query in doFrame.
            //phidgets1018.InputChange += ifKit_InputChange;
            //phidgets1018.OutputChange += ifKit_OutputChange;
            //phidgets1018.SensorChange += ifKit_SensorChange;

            phidgets1018.waitForAttachment(1000);
            dp.BroadcastLog(this, "1018 found", 0);
        }
        catch (PhidgetException ex)
        {
            dp.BroadcastLog(this, String.Format("Error opening Phidgets DAQ {0}", ex.Description), 6);    
        }
    }


    void Sensor_Detach(object sender, DetachEventArgs e)
    {
        Phidget phid = sender as Phidget;
        if (phid == null) return;
        dp.BroadcastLog(this, String.Format("Phidgets Sensor {0} Detached", phid.Name), 5);
    }

    void Sensor_Error(object sender, ErrorEventArgs e)
    {
        Phidget phid = sender as Phidget;
        if (phid == null) return;

        dp.BroadcastLog(this, String.Format("Phidgets Sensor {0} Error: {1}", phid.Name, e.Description), 5);
    }


    public override void DoFrame(object source, ElapsedEventArgs e)
    {
        Buffer<Byte> buffer = BufferPool.PopEmpty();

        String outputData = "Phidgets\n";
        outputData += DateTime.Now.Ticks.ToString() + " ";
        if (phidgetTemperature.Attached)
        {
            outputData += phidgetTemperature.thermocouples[0].Temperature + " ";
            outputData += phidgetTemperature.ambientSensor.Temperature + " ";
        }
        else
            outputData += "0 0 ";

        for (int i = 0; i < 8; i++)
        {
            if (phidgets1018.Attached)
            {
                outputData += phidgets1018.sensors[i].RawValue.ToString() + " ";
                outputData += phidgets1018.inputs[i].ToString() + " ";
                outputData += phidgets1018.outputs[i].ToString() + " ";
            }
            else
                outputData += "0 false false ";
        }

        System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
        buffer.setData(encoding.GetBytes(outputData), BufferType.UTF8_PHIDGETS);
        buffer.Text = "Phidgets"; // String.Format("Phidgets");
        BufferPool.PostFull(buffer);
    }
    void tempSensor_Attach(object sender, AttachEventArgs e)
    {

    }


    void ifKit_Attach(object sender, AttachEventArgs e)
    {

    }

    /*
    void tempSensor_TemperatureChange(object sender, TemperatureChangeEventArgs e)
    {
     
    }

    void ifKit_InputChange(object sender, InputChangeEventArgs e)
    {
       
    }

    void ifKit_OutputChange(object sender, OutputChangeEventArgs e)
    {
        
    }

    void ifKit_SensorChange(object sender, SensorChangeEventArgs e)
    {
        
    }
    */

}
public class PhidgetsControllerNotInitializedException : Exception
{
    public PhidgetsControllerNotInitializedException(string message)
        : base(message)
    {
    }
}

}

