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
    private Status Status_1018 = Status.STAT_ERR;
    private Status Status_Temp = Status.STAT_ERR;
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

            dp.Broadcast(new PhidgetsStatusMessage(this, Status.STAT_GOOD, ErrStr.INIT_OK_PHID_1018));
        }
        catch (PhidgetException ex)
        {
            success = false;
            dp.BroadcastLog(this, String.Format("Error opening Phidgets DAQ {0}", ex.Description), 6);
            CheckedStatusBroadcast(Status_1018,new PhidgetsStatusMessage(this, Status.STAT_FAIL, ErrStr.INIT_FAIL_PHID_1018));
        }
        return success;
    }


    void Sensor1018_Detach(object sender, DetachEventArgs e)
    {
        Phidget phid = sender as Phidget;
        if (phid == null) return;
        CheckedStatusBroadcast(Status_1018,new PhidgetsStatusMessage(this, Status.STAT_DISC, ErrStr.PHID_1018_STAT_DISC));
    }

    void Sensor1018_Error(object sender, ErrorEventArgs e)
    {
        Phidget phid = sender as Phidget;
        if (phid == null) return;

        dp.BroadcastLog(this, String.Format("Phidgets Sensor {0} Error: {1}", phid.Name, e.Description), 5);
        CheckedStatusBroadcast(Status_1018,new PhidgetsStatusMessage(this, Status.STAT_FAIL, ErrStr.PHID_1018_STAT_ERR));
    }

    void Sensor1018_Attach(object sender, AttachEventArgs e)
    {
        Phidget phid = sender as Phidget;
        if (phid == null) return;
        CheckedStatusBroadcast(Status_1018,new PhidgetsStatusMessage(this, Status.STAT_ATCH, ErrStr.PHID_1018_STAT_ATCH));
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


            CheckedStatusBroadcast(Status_Temp, new PhidgetsTempStatusMessage(this, Status.STAT_GOOD, ErrStr.INIT_OK_PHID_TEMP));
            //dp.BroadcastLog(this, Str.GetErrStr(ErrStr.INIT_OK_PHID_TEMP), 0);
        }
        catch (PhidgetException ex)
        {
            success = false;
            dp.BroadcastLog
                (
                    this,
                    Status.STAT_ERR,
                    "Error waiting for temperature sensor:", ex.Message
                );
            CheckedStatusBroadcast(Status_Temp, new PhidgetsTempStatusMessage(this, Status.STAT_ERR, ErrStr.INIT_FAIL_PHID_TEMP));
        }
        return success;
    }

    void tempSensor_Attach(object sender, AttachEventArgs e)
    {
        Phidget phid = sender as Phidget;
        if (phid == null) return;
        //dp.BroadcastLog(this, "Phidgets Temp Sensor Attached", 5);
        CheckedStatusBroadcast(Status_Temp, new PhidgetsTempStatusMessage(this, Status.STAT_ATCH, ErrStr.PHID_TEMP_STAT_ATCH));
    }

    void SensorTemp_Detach(object sender, DetachEventArgs e)
    {
        Phidget phid = sender as Phidget;
        if (phid == null) return;
        //dp.BroadcastLog(this, "Phidgets Temp Sensor Detached", 5);
        CheckedStatusBroadcast(Status_Temp, new PhidgetsTempStatusMessage(this, Status.STAT_DISC, ErrStr.PHID_TEMP_STAT_DISC));
    }

    void SensorTemp_Error(object sender, ErrorEventArgs e)
    {
        Phidget phid = sender as Phidget;
        if (phid == null) return;

        //dp.BroadcastLog(this, "Phidgets Temp Sensor Error: "+e.Description, 5);
        CheckedStatusBroadcast(Status_Temp, new PhidgetsTempStatusMessage(this, Status.STAT_ERR, ErrStr.PHID_TEMP_STAT_FAIL));
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
        {
            CheckedStatusBroadcast(Status_1018, new PhidgetsStatusMessage(this, Status.STAT_GOOD, 
                ErrStr.PHID_1018_STAT_OK));
        }
        else
        {
            CheckedStatusBroadcast(Status_1018, new PhidgetsStatusMessage(this, Status.STAT_DISC,
                ErrStr.PHID_1018_STAT_DISC));
        }

        if (phidgetTemperature.Attached)
        {
            CheckedStatusBroadcast(Status_Temp, new PhidgetsTempStatusMessage(this, Status.STAT_GOOD, ErrStr.PHID_TEMP_STAT_OK));
        }
        else
        {
            CheckedStatusBroadcast(Status_Temp, new PhidgetsTempStatusMessage(this, Status.STAT_DISC, 
                ErrStr.PHID_TEMP_STAT_DISC));
        }
    }

    public override void exAccumulateMessage(Receiver r, Message m)
    {

        outputData += "\r\n";
        if (phidgetTemperature.Attached)
        {
            try
            {
                outputData += phidgetTemperature.thermocouples[0].Temperature + " ";// if the THERMOCOUPLE is not connected to the temp sensor this will throw.
                outputData += phidgetTemperature.ambientSensor.Temperature + " ";// can also happen if the therocouple is plugged in backwards or just loose.
            }
            catch (PhidgetException Uhhh)
            {
                CheckedStatusBroadcast(Status_Temp, new PhidgetsTempStatusMessage(this, Status.STAT_DISC,
                    ErrStr.PHID_TEMP_STAT_FAIL));
                outputData += "0 0 ";
            }
            catch (IndexOutOfRangeException Err)
            {
                CheckedStatusBroadcast(Status_Temp, new PhidgetsTempStatusMessage(this, Status.STAT_DISC,
                    ErrStr.PHID_TEMP_STAT_FAIL));
                outputData += "0 0 ";
            }
            catch (ArgumentOutOfRangeException Err)
            {
                CheckedStatusBroadcast(Status_Temp, new PhidgetsTempStatusMessage(this, Status.STAT_DISC,
                ErrStr.PHID_TEMP_STAT_FAIL));
                outputData += "0 0 ";
            }
        }
        else
        {
            outputData += "0 0 ";
        }

        if (phidgets1018.Attached)
        {
            for (int i = 0; i < 8; i++)
            {
                try
                {
                outputData += phidgets1018.sensors[i].RawValue + " ";
                outputData += phidgets1018.inputs[i] + " ";
                outputData += phidgets1018.outputs[i] + " ";
                }
                catch (PhidgetException Uhhh)
                {
                    CheckedStatusBroadcast(Status_1018, new PhidgetsStatusMessage(this, Status.STAT_DISC,
                    ErrStr.PHID_TEMP_STAT_FAIL));
                    outputData += "0 0 0 ";
                }
                catch (IndexOutOfRangeException Err)
                {
                    CheckedStatusBroadcast(Status_1018, new PhidgetsStatusMessage(this, Status.STAT_DISC,
                    ErrStr.PHID_TEMP_STAT_FAIL));
                    outputData += "0 0 0 ";
                }
                catch (ArgumentOutOfRangeException Err)
                {
                    CheckedStatusBroadcast(Status_1018, new PhidgetsStatusMessage(this, Status.STAT_DISC,
                    ErrStr.PHID_TEMP_STAT_FAIL));
                    outputData += "0 0 0 ";
                }
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

