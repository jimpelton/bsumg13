﻿using System;
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

        //digital out
        private DOChannel doChannel1;
        private DOChannel doChannel2;
        private DOChannel doChannel3;
        private DigitalSingleChannelWriter writer1;
        private DigitalSingleChannelWriter writer2;
        private DigitalSingleChannelWriter writer3;

        private Task digitalWriteTask;

        private string[] outs;
        private String outputData;
        private static Object hardwareMutex = new object();

        public enum Outputs {NI_LIGHT1_OUT,NI_LIGHT2_OUT,NI_HEATER_OUT};
        public enum State   {ON,OFF};

        public NIController(BufferPool<byte> bp, string id, bool receiving = true, int frame_time = 500) : base(bp, id, receiving, frame_time)
        {
        }



        public override void DoFrame(object source, ElapsedEventArgs e)
        {
            /*
            double analogDataIn_X_A=0;
            double analogDataIn_Y_A=0;
            double analogDataIn_Z_A=0;
            double analogDataIn_X_T=0;
            double analogDataIn_Y_T=0;
            double analogDataIn_Z_T=0;
            */

            try
            {
                using (Task digitalWriteTask = new Task())
                {                                    
                    
            /*      digitalWriteTask.DOChannels.CreateChannel("Dev1/port1/line0", "",
                        ChannelLineGrouping.OneChannelForAllLines);
                    digitalWriteTask.Start();
                   
                    DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                    togglepo1=((count++/10)%2==1);
                    //togglepo1 = true;
                    writer.WriteSingleSampleSingleLine(true, togglepo1);
                    
                    */
                    
                    
                    
                   // togglepo1 = !togglepo1;
                    //DOChannel myChannel = digitalWriteTask.DOChannels.CreateChannel("Dev1/port0/line1", "", ChannelLineGrouping.OneChannelForAllLines);
                    //myChannel.OutputDriveType = DOOutputDriveType.ActiveDrive;
                   // myChannel.InvertLines = true;
                }
            }
            catch (DaqException ex)
            {
                dp.BroadcastLog(this, ex.Message, 1);
            }

            /*

            lock (hardwareMutex)// if another instance of this method is executing we can loose data.
            {
                analogDataIn_X_A = reader_X_A.ReadSingleSample();
                analogDataIn_Y_A = reader_Y_A.ReadSingleSample();
                analogDataIn_Z_A = reader_Z_A.ReadSingleSample();
                analogDataIn_X_T = reader_X_T.ReadSingleSample();
                analogDataIn_Y_T = reader_Y_T.ReadSingleSample();
                analogDataIn_Z_T = reader_Z_T.ReadSingleSample();
            }


            Buffer<Byte> buffer = BufferPool.PopEmpty();
            String outputData = "NI6008\n";

            outputData += DateTime.Now.Ticks.ToString() + " ";
            outputData += analogDataIn_X_A + " ";
            outputData += analogDataIn_Y_A + " ";
            outputData += analogDataIn_Z_A + " ";
            outputData += analogDataIn_X_T + " ";
            outputData += analogDataIn_Y_T + " ";
            outputData += analogDataIn_Z_T + " ";

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            buffer.setData(encoding.GetBytes(outputData), BufferType.UTF8_NI6008);
            buffer.Text = String.Format("NI6008");
            BufferPool.PostFull(buffer);

            */



        }

        protected override bool init()
        {
            outputData = "";
            bool rval = true;
            try
            {

                outs = DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.DOLine, PhysicalChannelAccess.External);

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


                //set up the digital outs
                digitalWriteTask = new Task();
                doChannel1 = digitalWriteTask.DOChannels.CreateChannel("Dev1/port1/line0", "Chan1",
                    ChannelLineGrouping.OneChannelForAllLines);
                writer1 = new DigitalSingleChannelWriter(digitalWriteTask.Stream); 
                doChannel2 = digitalWriteTask.DOChannels.CreateChannel("Dev1/port1/line1", "Chan2",
                    ChannelLineGrouping.OneChannelForAllLines);                
                writer2 = new DigitalSingleChannelWriter(digitalWriteTask.Stream); 
                doChannel3 = digitalWriteTask.DOChannels.CreateChannel("Dev1/port1/line2", "Chan3",
                    ChannelLineGrouping.OneChannelForAllLines);
                writer3 = new DigitalSingleChannelWriter(digitalWriteTask.Stream);

                digitalWriteTask.Start();
                SetOutputState(Outputs.NI_LIGHT1_OUT, State.ON);
                dp.BroadcastLog(this, "NI-USB-6008 started up...", 1);
            }
            catch (NationalInstruments.DAQmx.DaqException)
            {
                rval = false;
                dp.BroadcastLog(this, "NI-USB-6008 failed to start up...", 1);
                //throw new NIControllerNotInitializedException(
                //    "NI-USB-6008 failed to start up...");
            }
            return rval;
        }

        public override void exHeartBeatMessage(Receiver r, Message m)
        {
            base.exHeartBeatMessage(r, m);
            Buffer<Byte> buffer = BufferPool.PopEmpty();
            String output = "NIDAQ \n";
            output += DateTime.Now.Ticks.ToString() + " ";
            output += outputData;
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            buffer.setData(encoding.GetBytes(output), BufferType.UTF8_NI6008);
            BufferPool.PostFull(buffer);
            outputData = "";         
        }

        public override void exAccumulateMessage(Receiver r, Message m)
        {
            try
            {
                if (DaqSystem.Local.Devices.Length > 0)//we have a (the) NI device connected
                {
                    base.exAccumulateMessage(r, m);
                    double analogDataIn_X_A = 0;
                    double analogDataIn_Y_A = 0;
                    double analogDataIn_Z_A = 0;
                    double analogDataIn_X_T = 0;
                    double analogDataIn_Y_T = 0;
                    double analogDataIn_Z_T = 0;
                    analogDataIn_X_A = reader_X_A.ReadSingleSample();
                    analogDataIn_Y_A = reader_Y_A.ReadSingleSample();
                    analogDataIn_Z_A = reader_Z_A.ReadSingleSample();
                    analogDataIn_X_T = reader_X_T.ReadSingleSample(); //throws Daqexception upon pulling the cable
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
                //lets reset it
                Reset();
            }

        }

        public void SetOutputState(Outputs o, State s)
        {
            try
            {
                if (DaqSystem.Local.Devices.Length > 0)//we have a (the) NI device connected
                {
                    switch (o)
                    {
                        case (Outputs.NI_LIGHT1_OUT):
                            writer1.WriteSingleSampleSingleLine(true, (s == State.ON) ? true : false);
                            break;
                        case (Outputs.NI_LIGHT2_OUT):
                            writer2.WriteSingleSampleSingleLine(true, (s == State.ON) ? true : false);
                            break;
                        case (Outputs.NI_HEATER_OUT):
                            writer3.WriteSingleSampleSingleLine(true, (s == State.ON) ? true : false);
                            break;
                    }
                }
            }
            catch (NationalInstruments.DAQmx.DaqException eeeee)
            {
                //lets reset it
                Reset();
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
            }
        }
    }


    public class NIControllerNotInitializedException : Exception
    {
        public NIControllerNotInitializedException(string message)
            : base(message)
        {
        }
    }





}
