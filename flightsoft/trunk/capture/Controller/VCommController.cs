﻿using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Timers;

namespace uGCapture
{
    public class VCommController : ReceiverController
    {
        private SerialPort port = null;
        private double humidity=0;
        private double temp1=0;
        private double temp2=0;
        private double temp3=0;
        private double pressure=0;
        private double illumunation=0;
        private int recordnum=0;

        public VCommController(BufferPool<byte> bp)
            : base(bp)
        {

        }


        public override void DoFrame(object source, ElapsedEventArgs e)
        {
            Buffer<Byte> buffer = null;
            while (buffer == null)
            {
                buffer = BufferPool.PopEmpty();
                Thread.Sleep(50);
            }
            String outputData = "Weatherboard\n";
            outputData += DateTime.Now.Ticks.ToString() + " ";
            outputData +=humidity.ToString() + " ";
            outputData +=temp1.ToString() + " ";
            outputData +=temp2.ToString() + " ";
            outputData +=temp3.ToString() + " ";
            outputData +=pressure.ToString() + " ";
            outputData +=illumunation.ToString() + " ";
            outputData += recordnum.ToString() + " ";

            if (buffer != null)
            {

                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                buffer.setData(encoding.GetBytes(outputData), BufferType.VCOM);
                buffer.Text = String.Format("Weatherboard");
                BufferPool.PostFull(buffer);

            }
        }

        public override void init()
        {
            port = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
            port.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
            port.ReadTimeout = 500;
            port.WriteTimeout = 500;
            try
            {
                port.Open();
            }
            catch( IOException e)
            {
                throw new VCommControllerNotInitializedException("VComm crapped out on .Open");
            }
            
        }

        private void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (port.BytesToRead > 64)
            {
                string data = port.ReadLine();
                string[] values = data.Split(',');
                if (values[0].Length > 1)
                {
                    values[0] = values[0].Substring(1);
                    humidity = Double.Parse(values[0]);
                }
                if (values[0].Length > 7)
                {
                    values[7] = values[7].Substring(0, 6);
                    recordnum = int.Parse(values[7]);
                }
            
                temp1 = Double.Parse(values[1]);
                temp2 = Double.Parse(values[2]);
                temp3 = Double.Parse(values[3]);
                pressure = Double.Parse(values[4]);
                illumunation = Double.Parse(values[5]);
                
            }
        }

    }
    public class VCommControllerNotInitializedException : Exception
    {
        public VCommControllerNotInitializedException(string message)
            : base(message)
        {
        }
    }
}
