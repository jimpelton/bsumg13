// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-03-29                                                                      
// ******************************************************************************

using System;
using System.Threading;
using System.Timers;
using Phidgets;
using Phidgets.Events;


namespace uGCapture
{
public class PhidgetsController : ReceiverController
{

    private TemperatureSensor phidgetTemperature;
    private InterfaceKit phidgets1018;

    private double phidgetTemperature_AmbientTemp = 0;
    private double phidgetTemperature_ProbeTemp = 0;

    private bool[] digitalInputs; // = null;
    private bool[] digitalOutputs; // = null;
    private int[] analogInputs; // = null;

    public PhidgetsController(BufferPool<byte> bp, string id, bool receiving = true, int frame_time = 500) : base(bp, id, receiving, frame_time)
    {
        digitalInputs = new bool[8];
        digitalOutputs = new bool[8];
        analogInputs = new int[8];     
    }


    //public PhidgetsController(BufferPool<byte> bp) 
    //    : base(bp)
    //{
    //    digitalInputs = new bool[8];
    //    digitalOutputs = new bool[8];
    //    analogInputs = new int[8];     
    //}

            
    protected override bool init()
    {
        bool initSuccess = openTempSenser() && openDAQ();
        if (initSuccess)
        {
            dp.BroadcastLog(this, "Phidgets started up...", 1);
        }
        return initSuccess;
    }

    private bool openTempSenser()
    {
        bool success = true;
        try
        {
            dp.BroadcastLog(this, "Searching for temperature sensor", 0);
            phidgetTemperature = new TemperatureSensor();
            phidgetTemperature.open();

            phidgetTemperature.waitForAttachment(1000);

            phidgetTemperature.Attach += tempSensor_Attach;
            phidgetTemperature.Detach += Sensor_Detach;
            phidgetTemperature.Error += Sensor_Error;
            phidgetTemperature.TemperatureChange += tempSensor_TemperatureChange;

            phidgetTemperature.thermocouples[0].Sensitivity = 0.001;


            dp.BroadcastLog(this, "Temperature Sensor found", 0);
        }
        catch (PhidgetException ex)
        {
            success = false;
            dp.BroadcastLog
                (
                    this,
                    String.Format("Error waiting for temperature sensor: {0}", ex.Description), 
                    100
                );
        }
        return success;
    }

    private bool openDAQ()
    {
        bool success = true;
        try
        {
            dp.BroadcastLog(this, "Searching for 1018 ", 0);
            phidgets1018 = new InterfaceKit();
            phidgets1018.open();
            phidgets1018.Attach += ifKit_Attach;
            phidgets1018.Detach += Sensor_Detach;
            phidgets1018.Error += Sensor_Error;

            phidgets1018.InputChange += ifKit_InputChange;
            phidgets1018.OutputChange += ifKit_OutputChange;
            phidgets1018.SensorChange += ifKit_SensorChange;

            phidgets1018.waitForAttachment(1000);
            dp.BroadcastLog(this, "1018 found", 0);
        }
        catch (PhidgetException ex)
        {
            success = false;
            dp.BroadcastLog(this, String.Format("Error opening Phidgets DAQ {0}", ex.Description), 6);    
        }
        return success;
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

    void tempSensor_Attach(object sender, AttachEventArgs e)
    {

    }

    void tempSensor_TemperatureChange(object sender, TemperatureChangeEventArgs e)
    {
        if (e.Index == 0)
            phidgetTemperature_ProbeTemp = e.Temperature;

        phidgetTemperature_AmbientTemp = phidgetTemperature.ambientSensor.Temperature;

        //debug
        //dp.BroadcastLog(this, String.Format("OMG OMG! Temperature Changed! {0}", e.Temperature), 2);
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
        digitalOutputs[e.Index] = e.Value;
    }

    void ifKit_SensorChange(object sender, SensorChangeEventArgs e)
    {
        analogInputs[e.Index] = e.Value;
    }

    public override void DoFrame(object source, ElapsedEventArgs e)
    {
 
        Buffer<Byte> buffer = BufferPool.PopEmpty();

        String outputData = "Phidgets\n";
        outputData += DateTime.Now.Ticks.ToString() + " ";
        outputData += phidgetTemperature_ProbeTemp + " ";
        outputData += phidgetTemperature_AmbientTemp + " ";
        for (int i = 0; i < 8; i++)
        {
            outputData += analogInputs[i].ToString() + " ";
            outputData += digitalInputs[i].ToString() + " ";
            outputData += digitalOutputs[i].ToString() + " ";
        }

        System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
        buffer.setData(encoding.GetBytes(outputData), BufferType.UTF8_PHIDGETS);
        buffer.Text = "Phidgets"; // String.Format("Phidgets");
        BufferPool.PostFull(buffer);
    }

    public override void exHeartBeatMessage(Receiver r, Message m)
    {
        base.exHeartBeatMessage(r, m);
        throw new NotImplementedException();

    }

    public override void exAccumulateMessage(Receiver r, Message m)
    {
        base.exAccumulateMessage(r, m);
        throw new NotImplementedException();
    }


}
public class PhidgetsControllerNotInitializedException : Exception
{
    public PhidgetsControllerNotInitializedException(string message)
        : base(message)
    {
    }
}

}

