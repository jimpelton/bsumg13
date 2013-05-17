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

        public VCommController(BufferPool<byte> bp, string id, bool receiving = true) 
            : base(bp, id, receiving)
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
                dp.Broadcast(new VcommStatusMessage(this, StatusStr.STAT_FAIL));
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.StackTrace);
                port.Close();              
                rval = false;
                dp.Broadcast(new VcommStatusMessage(this, StatusStr.STAT_FAIL));
            }
            return rval;
        }

        private void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (port.BytesToRead > 64)
                {
                    string data = port.ReadLine();
                    lock (outputData)
                    {
                        outputData = data;
                        hasNewData = true;
                    }
                    dp.Broadcast(new VcommStatusMessage(this, StatusStr.STAT_GOOD));
                }
            }
            catch (InvalidOperationException err) 
            {
                Reset();
                dp.Broadcast(new VcommStatusMessage(this, StatusStr.STAT_FAIL));
            }
            catch (TimeoutException err) 
            {
                Reset();
            }

        }

        private void Reset()
        {
            init();
        }

        public override void exHeartBeatMessage(Receiver r, Message m)
        {
            //test the port.
            try
            {
                if (!port.IsOpen)
                {
                    Reset();
                } 
            }
            catch (UnauthorizedAccessException err)
            {
                Reset();
                dp.Broadcast(new VcommStatusMessage(this, StatusStr.STAT_FAIL));
            }
           
            String output = "Weatherboard \n";
 
            lock (outputData)
            {
                if (hasNewData)
                {
                    output += GetUTCMillis().ToString() + " " + outputData;
                    //output += outputData;
                    hasNewData = false;
                }
                else
                {
                    output = "";
                }
            }

            //break this out here to minimize the time in the lock.
            if (output.Length > 0)
            {
                Buffer<Byte> buffer = BufferPool.PopEmpty();
                UTF8Encoding encoding = new UTF8Encoding();
                buffer.setData(encoding.GetBytes(output), BufferType.UTF8_VCOMM);
                BufferPool.PostFull(buffer);
            }

        }


    }
}
