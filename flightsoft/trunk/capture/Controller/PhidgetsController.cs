// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-03-29                                                                      
// ******************************************************************************

using System;
using System.Text;
using System.Threading;
using System.Timers;
using Phidgets;
using Phidgets.Events;
using uGCapture;


namespace uGCapture
{
public class PhidgetsController : ReceiverController
{

    private TemperatureSensor phidgetTemperature;
    private InterfaceKit phidgets1018;
    private String outputData;

    public PhidgetsController(BufferPool<byte> bp, string id, bool receiving = true) 
        : base(bp, id, receiving)
    {
        outputData = "";
    }


   
    protected override bool init()
    {
        bool temp_res = openTempSenser();
        bool daq_res = openDAQ();

        bool initSuccess = temp_res && daq_res;
	    
        if (initSuccess)
        {
            dp.BroadcastLog(this, "Phidgets started up...", 1);
        }
        return initSuccess;
    }

   

    /************************************************************************/
    /* PHIDGETS 1018 INIT AND EVENT HANDLERS                                */
    /************************************************************************/
    private bool openDAQ()
    {
        bool success = true;
        try
        {
            dp.BroadcastLog(this, "Searching for 1018 ", 0);
            phidgets1018 = new InterfaceKit();
            phidgets1018.open();
            phidgets1018.Attach += Sensor1018_Attach;
            phidgets1018.Detach += Sensor1018_Detach;
            phidgets1018.Error += Sensor1018_Error;
            phidgets1018.waitForAttachment(1000);

            dp.Broadcast(new PhidgetsStatusMessage(this, StatusStr.STAT_GOOD));
            dp.BroadcastLog(this, "1018 found", 0);
        }
        catch (PhidgetException ex)
        {
            success = false;
            dp.BroadcastLog(this, String.Format("Error opening Phidgets DAQ {0}", ex.Description), 6);
            dp.Broadcast(new PhidgetsStatusMessage(this, StatusStr.STAT_FAIL));
        }
        return success;
    }


    void Sensor1018_Detach(object sender, DetachEventArgs e)
    {
        Phidget phid = sender as Phidget;
        if (phid == null) return;
        dp.BroadcastLog(this, String.Format("Phidgets Sensor {0} Detached", phid.Name), 5);
        dp.Broadcast(new PhidgetsStatusMessage(this, StatusStr.STAT_DISC));
    }

    void Sensor1018_Error(object sender, ErrorEventArgs e)
    {
        Phidget phid = sender as Phidget;
        if (phid == null) return;

        dp.BroadcastLog(this, String.Format("Phidgets Sensor {0} Error: {1}", phid.Name, e.Description), 5);
        dp.Broadcast(new PhidgetsStatusMessage(this, StatusStr.STAT_FAIL));
    }

    void Sensor1018_Attach(object sender, AttachEventArgs e)
    {
        Phidget phid = sender as Phidget;
        if (phid == null) return;
        dp.BroadcastLog(this, String.Format("Phidgets Sensor {0} Attached", phid.Name), 5);
        dp.Broadcast(new PhidgetsStatusMessage(this, StatusStr.STAT_ATCH));
    }

    /************************************************************************/
    /* PHIDGETS TEMP SENSOR INIT AND EVENT HANDLERS                         */
    /************************************************************************/

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
            phidgetTemperature.Detach += SensorTemp_Detach;
            phidgetTemperature.Error += SensorTemp_Error;

            phidgetTemperature.thermocouples[0].Sensitivity = 0.001;


            dp.Broadcast(new PhidgetsStatusMessage(this, StatusStr.STAT_GOOD));
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
            dp.Broadcast(new PhidgetsTempStatusMessage(this, StatusStr.STAT_FAIL));
        }
        return success;
    }

    void tempSensor_Attach(object sender, AttachEventArgs e)
    {
        Phidget phid = sender as Phidget;
        if (phid == null) return;
        dp.BroadcastLog(this, "Phidgets Temp Sensor Attached", 5);
        dp.Broadcast(new PhidgetsTempStatusMessage(this, StatusStr.STAT_ATCH));
    }

    void SensorTemp_Detach(object sender, DetachEventArgs e)
    {
        Phidget phid = sender as Phidget;
        if (phid == null) return;
        dp.BroadcastLog(this, "Phidgets Temp Sensor Detached", 5);
        dp.Broadcast(new PhidgetsTempStatusMessage(this, StatusStr.STAT_DISC));
    }

    void SensorTemp_Error(object sender, ErrorEventArgs e)
    {
        Phidget phid = sender as Phidget;
        if (phid == null) return;

        dp.BroadcastLog(this, "Phidgets Temp Sensor Error: "+e.Description, 5);
        dp.Broadcast(new PhidgetsTempStatusMessage(this, StatusStr.STAT_FAIL));
    }


    /************************************************************************/
    /* MESSAGE HANDLERS                                                     */
    /************************************************************************/

    public override void exHeartBeatMessage(Receiver r, Message m)
    {
        if ((phidgetTemperature.Attached) || (phidgets1018.Attached))
        {
            Buffer<Byte> buffer = BufferPool.PopEmpty();
            String output = "Phidgets \r\n";
            output += GetUTCMillis().ToString() + " ";
            output += outputData;
            UTF8Encoding encoding = new UTF8Encoding();
            buffer.setData(encoding.GetBytes(output), BufferType.UTF8_PHIDGETS);
            BufferPool.PostFull(buffer);
            outputData = "";
        }
        if (phidgets1018.Attached)
            dp.Broadcast(new PhidgetsStatusMessage(this, StatusStr.STAT_GOOD));
        else
            dp.Broadcast(new PhidgetsStatusMessage(this, StatusStr.STAT_FAIL));
        if (phidgetTemperature.Attached)
            dp.Broadcast(new PhidgetsStatusMessage(this, StatusStr.STAT_GOOD));
        else
            dp.Broadcast(new PhidgetsStatusMessage(this, StatusStr.STAT_FAIL));

    }

    public override void exAccumulateMessage(Receiver r, Message m)
    {

        outputData += "\r\n";
        if (phidgetTemperature.Attached)
        {
            outputData += phidgetTemperature.thermocouples[0].Temperature + " ";
            outputData += phidgetTemperature.ambientSensor.Temperature + " ";
        }
        else
        {
            outputData += "0 0 ";
        }

        if (phidgets1018.Attached)
        {
            for (int i = 0; i < 8; i++)
            {
                outputData += phidgets1018.sensors[i].RawValue + " ";
                outputData += phidgets1018.inputs[i] + " ";
                outputData += phidgets1018.outputs[i] + " ";
            }
        }
        else
        {
            for (int i = 0; i < 8; i++)
            {
                outputData += "0 0 0 ";
            }
        }
    }
}


}

