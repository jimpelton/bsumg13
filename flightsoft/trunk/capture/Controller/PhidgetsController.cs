using System;
using System.Timers;
using Phidgets;
using Phidgets.Events;


namespace uGCapture
{
public class PhidgetsController : ReceiverController
{

    private TemperatureSensor phidgetTemperature = null;
    private InterfaceKit      phidgets1018       = null;

    private double phidgetTemperature_AmbientTemp = 0;
    private double phidgetTemperature_ProbeTemp = 0;

    private bool[] digitalInputs; // = null;
    private bool[] digitalOutputs; // = null;
    private int[] analogInputs; // = null;

    public PhidgetsController(BufferPool<byte> bp) 
        : base(bp)
    {
        digitalInputs = new bool[8];
        digitalOutputs = new bool[8];
        analogInputs = new int[8];
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
            phidgets1018.Attach += new AttachEventHandler(ifKit_Attach);
            phidgets1018.Detach += new DetachEventHandler(Sensor_Detach);
            phidgets1018.Error += new ErrorEventHandler(Sensor_Error);

            phidgets1018.InputChange += new InputChangeEventHandler(ifKit_InputChange);
            phidgets1018.OutputChange += new OutputChangeEventHandler(ifKit_OutputChange);
            phidgets1018.SensorChange += new SensorChangeEventHandler(ifKit_SensorChange);

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
        dp.BroadcastLog(this, String.Format("{0} Detached", phid.Name), 5);
    }

    void Sensor_Error(object sender, ErrorEventArgs e)
    {
        Phidget phid = sender as Phidget;
        if (phid == null) return;

        dp.BroadcastLog(this, String.Format("{0} Error: {1}", phid.Name, e.Description), 5);
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

    public override void DoFrame(object source, ElapsedEventArgs e)
    {
        Buffer<Byte> buffer = null;
        //while(buffer==null)
        //    buffer=StagingBuffer.PopEmpty();
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
        if (buffer != null)
        {

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            buffer.setData(encoding.GetBytes(outputData), BufferType.PHIDGETS);
            buffer.Text = String.Format("Phidgets");
            //buffer.CapacityUtilization = ((uint) encoding.GetByteCount(outputData));
            //buffer.CapacityUtilization = (uint)outputData.Length*sizeof(char); not the case

            //StagingBuffer.PostFull(buffer);
        }
    }


}
}

