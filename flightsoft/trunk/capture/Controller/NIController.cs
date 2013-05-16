// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-05-05                                                                      
// ******************************************************************************

using System;
using System.Text;
using System.Threading;
using System.Timers;
using NationalInstruments;
using NationalInstruments.DAQmx;

namespace uGCapture
{
    public class NIController : ReceiverController
    {
        //analog in
        private Task analogInTask_X_A = new Task();
        private Task analogInTask_Y_A = new Task();
        private Task analogInTask_Z_A = new Task();
        private Task analogInTask_X_T = new Task();
        private Task analogInTask_Y_T = new Task();
        private Task analogInTask_Z_T = new Task();

        private AIChannel AIChannel_X_A;
        private AIChannel AIChannel_Y_A;
        private AIChannel AIChannel_Z_A;
        private AIChannel AIChannel_X_T;
        private AIChannel AIChannel_Y_T;
        private AIChannel AIChannel_Z_T;

        private AnalogSingleChannelReader reader_X_A;
        private AnalogSingleChannelReader reader_Y_A;
        private AnalogSingleChannelReader reader_Z_A;
        private AnalogSingleChannelReader reader_X_T;
        private AnalogSingleChannelReader reader_Y_T;
        private AnalogSingleChannelReader reader_Z_T;

        private string[] outs;
        private String outputData;
        private static Object hardwareMutex = new object();

        public enum Outputs
        {
            NI_LIGHT11_OUT,
            NI_LIGHT21_OUT,
            NI_LIGHT12_OUT,
            NI_LIGHT22_OUT,
            NI_HEATER_OUT
        };

        public enum State
        {
            ON,
            OFF
        };

        public NIController(BufferPool<byte> bp, string id, bool receiving = true)
            : base(bp, id, receiving)
        {
        }


     

        protected override bool init()
        {
            outputData = "";
            bool rval = true;
            try
            {
                outs = DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.DOLine,
                                                           PhysicalChannelAccess.External);

                AIChannel_X_A = analogInTask_X_A.AIChannels.CreateVoltageChannel(
                    "dev1/ai0", "AIChannel_X_A", AITerminalConfiguration.Rse, 0, 5,
                    AIVoltageUnits.Volts);

                AIChannel_Y_A = analogInTask_Y_A.AIChannels.CreateVoltageChannel(
                    "dev1/ai4", "AIChannel_Y_A", AITerminalConfiguration.Rse, 0, 5,
                    AIVoltageUnits.Volts);

                AIChannel_Z_A = analogInTask_Z_A.AIChannels.CreateVoltageChannel(
                    "dev1/ai1", "AIChannel_Z_A", AITerminalConfiguration.Rse, 0, 5,
                    AIVoltageUnits.Volts);

                AIChannel_X_T = analogInTask_X_T.AIChannels.CreateVoltageChannel(
                    "dev1/ai5", "AIChannel_X_T", AITerminalConfiguration.Rse, 0, 5,
                    AIVoltageUnits.Volts);

                AIChannel_Y_T = analogInTask_Y_T.AIChannels.CreateVoltageChannel(
                    "dev1/ai2", "AIChannel_Y_T", AITerminalConfiguration.Rse, 0, 5,
                    AIVoltageUnits.Volts);

                AIChannel_Z_T = analogInTask_Z_T.AIChannels.CreateVoltageChannel(
                    "dev1/ai6", "AIChannel_Z_T", AITerminalConfiguration.Rse, 0, 5,
                    AIVoltageUnits.Volts);

                reader_X_A = new AnalogSingleChannelReader(analogInTask_X_A.Stream);
                reader_Y_A = new AnalogSingleChannelReader(analogInTask_Y_A.Stream);
                reader_Z_A = new AnalogSingleChannelReader(analogInTask_Z_A.Stream);
                reader_X_T = new AnalogSingleChannelReader(analogInTask_X_T.Stream);
                reader_Y_T = new AnalogSingleChannelReader(analogInTask_Y_T.Stream);
                reader_Z_T = new AnalogSingleChannelReader(analogInTask_Z_T.Stream);

                SetOutputState(Outputs.NI_HEATER_OUT, State.ON);
                SetOutputState(Outputs.NI_LIGHT11_OUT, State.ON);
                SetOutputState(Outputs.NI_LIGHT12_OUT, State.ON);
                SetOutputState(Outputs.NI_LIGHT21_OUT, State.ON);
                SetOutputState(Outputs.NI_LIGHT22_OUT, State.ON);

                dp.BroadcastLog(this, "NI-USB-6008 started up...", 1);
                dp.Broadcast(new NI6008StatusMessage(this, StatusStr.STAT_GOOD_NI6008DAQ));
            }
            catch (DaqException)
            {
                rval = false;
                dp.BroadcastLog(this, "NI-USB-6008 failed to start up...", 1);
                dp.Broadcast(new NI6008StatusMessage(this, StatusStr.STAT_FAIL_NI6008DAQ));
            }
            return rval;
        }

        public override void exHeartBeatMessage(Receiver r, Message m)
        {
            try
            {
                if (DaqSystem.Local.Devices.Length > 0)
                    //we have a (the) NI device connected
                {
                    Buffer<Byte> buffer = BufferPool.PopEmpty();
                    String output = "NIDAQ \n";
                    output += DateTime.Now.Ticks.ToString() + " ";
                    output += outputData;
                    UTF8Encoding encoding = new UTF8Encoding();
                    buffer.setData(encoding.GetBytes(output), BufferType.UTF8_NI6008);
                    BufferPool.PostFull(buffer);
                    outputData = "";
                }
            }
            catch (DaqException eeeee)
            {
                dp.Broadcast(new NI6008StatusMessage(this, StatusStr.STAT_FAIL_NI6008DAQ));
                //lets reset it
                Reset();
            }
        }

        public override void exAccumulateMessage(Receiver r, Message m)
        {
            try
            {
                if (DaqSystem.Local.Devices.Length > 0)
                    //we have a (the) NI device connected
                {
                    base.exAccumulateMessage(r, m);
                    double analogDataIn_X_A = 0;
                    double analogDataIn_Y_A = 0;
                    double analogDataIn_Z_A = 0;
                    double analogDataIn_X_T = 0;
                    double analogDataIn_Y_T = 0;
                    double analogDataIn_Z_T = 0;
                    analogDataIn_X_A = reader_X_A.ReadSingleSample();
                        //throws a null reference exception if we start with it unplugged and then plug it in.
                    analogDataIn_Y_A = reader_Y_A.ReadSingleSample();
                    analogDataIn_Z_A = reader_Z_A.ReadSingleSample();
                    analogDataIn_X_T = reader_X_T.ReadSingleSample();
                        //throws Daqexception upon pulling the cable
                    analogDataIn_Y_T = reader_Y_T.ReadSingleSample();
                    analogDataIn_Z_T = reader_Z_T.ReadSingleSample();
                    outputData += analogDataIn_X_A + " ";
                    outputData += analogDataIn_Y_A + " ";
                    outputData += analogDataIn_Z_A + " ";
                    outputData += analogDataIn_X_T + " ";
                    outputData += analogDataIn_Y_T + " ";
                    outputData += analogDataIn_Z_T + " ";
                }
            }
            catch (NationalInstruments.DAQmx.DaqException eeeee)
            {
                dp.Broadcast(new NI6008StatusMessage(this, StatusStr.STAT_FAIL_NI6008DAQ));
                //lets reset it
                Reset();
            }
            catch (NullReferenceException Squeee)
            {
                dp.Broadcast(new NI6008StatusMessage(this, StatusStr.STAT_FAIL_NI6008DAQ));
                Reset();
            }
        }

        public void SetOutputState(Outputs o, State s)
        {
            try
            {
                if (DaqSystem.Local.Devices.Length > 0)
                    //we have a (the) NI device connected
                {
                    switch (o)
                    {
                        case (Outputs.NI_HEATER_OUT):
                            WriteState("Dev1/port0/line0", (s == State.ON) ? true : false);
                            break;
                        case (Outputs.NI_LIGHT11_OUT):
                            WriteState("Dev1/port0/line1", (s == State.ON) ? true : false);
                            break;
                        case (Outputs.NI_LIGHT12_OUT):
                            WriteState("Dev1/port0/line2", (s == State.ON) ? true : false);
                            break;
                        case (Outputs.NI_LIGHT21_OUT):
                            WriteState("Dev1/port0/line3", (s == State.ON) ? true : false);
                            break;
                        case (Outputs.NI_LIGHT22_OUT):
                            WriteState("Dev1/port0/line4", (s == State.ON) ? true : false);
                            break;
                    }
                }
            }
            catch (NationalInstruments.DAQmx.DaqException eeeee)
            {
                dp.Broadcast(new NI6008StatusMessage(this, StatusStr.STAT_FAIL_NI6008DAQ));
                //lets reset it
                Reset();
            }
        }

        private void WriteState(String sline, Boolean state)
        {
            try
            {
                using (Task digitalWriteTask = new Task())
                {
                    digitalWriteTask.DOChannels.CreateChannel(sline, "",
                                                              ChannelLineGrouping
                                                                  .OneChannelForAllLines);
                    DigitalSingleChannelWriter writer =
                        new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                    writer.WriteSingleSampleSingleLine(true, state);
                    digitalWriteTask.Start();
                }
            }
            catch (DaqException ex)
            {
                dp.Broadcast(new NI6008StatusMessage(this, StatusStr.STAT_FAIL_NI6008DAQ));
                dp.BroadcastLog(this, ex.Message, 1);
            }
        }

        private void Reset()
        {
            try
            {
                Device dev = DaqSystem.Local.LoadDevice("dev1");
                dev.Reset();
                init();
            }
            catch (NationalInstruments.DAQmx.DaqException eeeee)
            {
                //probably not connected at this point. Try again next time.
                dp.Broadcast(new NI6008StatusMessage(this, StatusStr.STAT_DISC_NI6008DAQ));
            }
        }
    }
}