using System;
using System.Threading;
using System.Timers;

using NationalInstruments;
using NationalInstruments.DAQmx;

namespace uGCapture
{
    public class NIController : ReceiverController
    {
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

        private static Object hardwareMutex = new object();

        public NIController(BufferPool<byte> bp) 
        : base(bp)
        {
        



        }





        public override void DoFrame(object source, ElapsedEventArgs e)
        {
            double analogDataIn_X_A=0;
            double analogDataIn_Y_A=0;
            double analogDataIn_Z_A=0;
            double analogDataIn_X_T=0;
            double analogDataIn_Y_T=0;
            double analogDataIn_Z_T=0;

            
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
            




        }

        public override void init()
        {
            try
            {




                AIChannel_X_A = analogInTask_X_A.AIChannels.CreateVoltageChannel(
                    "dev1/ai0", "AIChannel_X_A", AITerminalConfiguration.Rse, 0, 5, AIVoltageUnits.Volts);

                AIChannel_Y_A = analogInTask_Y_A.AIChannels.CreateVoltageChannel(
                    "dev1/ai4", "AIChannel_Y_A", AITerminalConfiguration.Rse, 0, 5, AIVoltageUnits.Volts);

                AIChannel_Z_A = analogInTask_Z_A.AIChannels.CreateVoltageChannel(
                    "dev1/ai1", "AIChannel_Z_A", AITerminalConfiguration.Rse, 0, 5, AIVoltageUnits.Volts);

                AIChannel_X_T = analogInTask_X_T.AIChannels.CreateVoltageChannel(
                    "dev1/ai5", "AIChannel_X_T", AITerminalConfiguration.Rse, 0, 5, AIVoltageUnits.Volts);

                AIChannel_Y_T = analogInTask_Y_T.AIChannels.CreateVoltageChannel(
                    "dev1/ai2", "AIChannel_Y_T", AITerminalConfiguration.Rse, 0, 5, AIVoltageUnits.Volts);

                AIChannel_Z_T = analogInTask_Z_T.AIChannels.CreateVoltageChannel(
                    "dev1/ai6", "AIChannel_Z_T", AITerminalConfiguration.Rse, 0, 5, AIVoltageUnits.Volts);

                reader_X_A = new AnalogSingleChannelReader(analogInTask_X_A.Stream);
                reader_Y_A = new AnalogSingleChannelReader(analogInTask_Y_A.Stream);
                reader_Z_A = new AnalogSingleChannelReader(analogInTask_Z_A.Stream);
                reader_X_T = new AnalogSingleChannelReader(analogInTask_X_T.Stream);
                reader_Y_T = new AnalogSingleChannelReader(analogInTask_Y_T.Stream);
                reader_Z_T = new AnalogSingleChannelReader(analogInTask_Z_T.Stream);

                dp.BroadcastLog(this, "NI-USB-6008 started up...", 1);
            }
            catch (NationalInstruments.DAQmx.DaqException)
            {
                dp.BroadcastLog(this, "NI-USB-6008 failed to start up...", 1);
                throw new NIControllerNotInitializedException("NI-USB-6008 failed to start up...");
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
