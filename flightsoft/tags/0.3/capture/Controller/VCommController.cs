// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-04-06                                                                      
// ******************************************************************************

using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Timers;

namespace uGCapture
{
    public class VCommController : ReceiverController
    {
        private SerialPort port = null;
        private String outputData;
        private bool hasNewData;

        public VCommController(BufferPool<byte> bp, string id, 
            bool receiving = true, int frame_time = 500) : base(bp, id, receiving, frame_time)
        {
        }

        protected override bool init()
        {
            outputData = "";
            bool rval = true;
            port = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
            port.DataReceived += sp_DataReceived;
            port.ReadTimeout = 500;
            port.WriteTimeout = 500;
            if (port.IsOpen)
            {
                port.Close();
            }

            try
            {
                port.Open();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.StackTrace);
                rval = false;
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.StackTrace);
                port.Close();
                rval = false;
            }
            return rval;
        }

        public override void DoFrame(object source, ElapsedEventArgs e)
        {
            /*
            Buffer<byte> buffer = BufferPool.PopEmpty();

            String outputData = "Weatherboard\n";
            outputData += DateTime.Now.Ticks.ToString() + " ";
            outputData += humidity.ToString() + " ";
            outputData += temp1.ToString() + " ";
            outputData += temp2.ToString() + " ";
            outputData += temp3.ToString() + " ";
            outputData += pressure.ToString() + " ";
            outputData += illumunation.ToString() + " ";
            outputData += recordnum.ToString() + " ";

            buffer.setData(new UTF8Encoding().GetBytes(outputData), 
                BufferType.UTF8_VCOMM);
            buffer.Text = "Weatherboard"; 
            BufferPool.PostFull(buffer);
            */
        }

        private void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (port.BytesToRead > 64)
            {
                string data = port.ReadLine();
                lock (outputData)
                {
                    outputData = data;
                    hasNewData = true;
                }                              
            }
        }

        public override void exHeartBeatMessage(Receiver r, Message m)
        {
            
            Buffer<Byte> buffer = BufferPool.PopEmpty();
            String output = "Weatherboard \n";

            lock (outputData)
            {
                if (hasNewData)
                {
                    output += DateTime.Now.Ticks.ToString() + " ";
                    output += outputData;                
                    hasNewData = false;
                }
                else
                {
                    output += DateTime.Now.Ticks.ToString() + " ";
                    output += outputData;                 
                }
            }
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            buffer.setData(encoding.GetBytes(output), BufferType.UTF8_VCOMM);
            BufferPool.PostFull(buffer);
            
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
