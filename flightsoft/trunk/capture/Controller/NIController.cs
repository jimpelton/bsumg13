using System;
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

        public override void DoFrame(object source, ElapsedEventArgs e)
        {

            double analogDataIn_X_A = reader_X_A.ReadSingleSample();
            double analogDataIn_Y_A = reader_Y_A.ReadSingleSample();
            double analogDataIn_Z_A = reader_Z_A.ReadSingleSample();
            double analogDataIn_X_T = reader_X_T.ReadSingleSample();
            double analogDataIn_Y_T = reader_Y_T.ReadSingleSample();
            double analogDataIn_Z_T = reader_Z_T.ReadSingleSample();



        }

        public override void init()
        {

            AIChannel_X_A = analogInTask_X_A.AIChannels.CreateVoltageChannel(
"dev1/ai0", "AIChannel_X_A", AITerminalConfiguration.Differential, 0, 5, AIVoltageUnits.Volts);

            AIChannel_Y_A = analogInTask_Y_A.AIChannels.CreateVoltageChannel(
"dev1/ai4", "AIChannel_Y_A", AITerminalConfiguration.Differential, 0, 5, AIVoltageUnits.Volts);

            AIChannel_Z_A = analogInTask_Z_A.AIChannels.CreateVoltageChannel(
"dev1/ai1", "AIChannel_Z_A", AITerminalConfiguration.Differential, 0, 5, AIVoltageUnits.Volts);

            AIChannel_X_T = analogInTask_X_T.AIChannels.CreateVoltageChannel(
"dev1/ai5", "AIChannel_X_T", AITerminalConfiguration.Differential, 0, 5, AIVoltageUnits.Volts);

            AIChannel_Y_T = analogInTask_Y_T.AIChannels.CreateVoltageChannel(
"dev1/ai2", "AIChannel_Y_T", AITerminalConfiguration.Differential, 0, 5, AIVoltageUnits.Volts);

            AIChannel_Z_T = analogInTask_Z_T.AIChannels.CreateVoltageChannel(
"dev1/ai6", "AIChannel_Z_T", AITerminalConfiguration.Differential, 0, 5, AIVoltageUnits.Volts);

            AnalogSingleChannelReader reader_X_A = new AnalogSingleChannelReader(analogInTask_X_A.Stream);
            AnalogSingleChannelReader reader_Y_A = new AnalogSingleChannelReader(analogInTask_Y_A.Stream);
            AnalogSingleChannelReader reader_Z_A = new AnalogSingleChannelReader(analogInTask_Z_A.Stream);
            AnalogSingleChannelReader reader_X_T = new AnalogSingleChannelReader(analogInTask_X_T.Stream);
            AnalogSingleChannelReader reader_Y_T = new AnalogSingleChannelReader(analogInTask_Y_T.Stream);
            AnalogSingleChannelReader reader_Z_T = new AnalogSingleChannelReader(analogInTask_Z_T.Stream);

        }
    }
}
