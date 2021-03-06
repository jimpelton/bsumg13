﻿using System;
using System.Text;

namespace uGCapture
{
    public class Logger : Receiver
    {
        private StringBuilder outputData;
        private Status logLevel;

        public Logger(String id, bool receiving = true, bool executing = true) 
            : base(id, receiving, executing)
        {
            // Set output data buffer to empty to begin.
            outputData = new StringBuilder();
        }


        /// <summary>
        /// LJX, 12,5,11.
        /// Returns a millisecond wall-clock timestamp in string format.
        /// </summary>
        /// <returns></returns>
        private String timeStamp()
        {
            return DateTime.Now.ToString(" yyyy MM dd HH:mm:ss.fff ");
        }


        /// <summary>
        /// Main pool that every Receiver derivative writes into.
        /// </summary>
        public BufferPool<byte> BufferPool
        {
            get { return m_bufferPool; }
            set { m_bufferPool = value; }
        }
        private BufferPool<byte> m_bufferPool;


        /// <summary>
        /// LJX, 12/5/11.
        /// Store the accumulated log messages.
        /// Write the log file if the log is bigger than a threshold size (currently 1MB).
        /// </summary>    
        public override void exLogMessage(Receiver r, Message m)
        {
            if (outputData.Length > MAX_FILE_LENGTH)
            {
                String output;
                lock (outputData)
                {
                    output = "Log\r\n" + GetUTCTimeStamp() + " " + m + outputData.ToString();
                    outputData.Clear();
                }

                Console.WriteLine("Log message" + m);
                Buffer<Byte> buffer = BufferPool.PopEmpty();// let it run for a few hours and it died here. Null Reference Exception. Bufferpool is null?
                UTF8Encoding encoding = new UTF8Encoding();
                buffer.setData(encoding.GetBytes(output), BufferType.UTF8_LOG);
                BufferPool.PostFull(buffer);
            }
            else // If we're under the write threshold, write the new log message to the output data
            {
                string s = GetUTCTimeStamp() + m + "\n";
                lock (outputData)
                {
                    outputData.Append(s);
                }
                Console.WriteLine("Log message: " + s);
            }
        }

        public override void exStatusMessage(Receiver r, Message m)
        {
            if (outputData.Length > MAX_FILE_LENGTH)
            {
                
                String output;
                lock (outputData)
                {
                    output = "Log\r\n" + GetUTCTimeStamp() + " " + m + outputData.ToString();
                    outputData.Clear();
                }

                Console.WriteLine(m);
                Buffer<Byte> buffer = BufferPool.PopEmpty();
                UTF8Encoding encoding = new UTF8Encoding();
                buffer.setData(encoding.GetBytes(output), BufferType.UTF8_LOG);
                BufferPool.PostFull(buffer);
            }
            else // If we're under the write threshold, write the new log message to the output data
            {
                string s = GetUTCTimeStamp() + m + "\n";
                lock (outputData)
                {
                    outputData.Append(s);
                }
                Console.WriteLine(s);
            }
        }

        /// <summary>
        /// LJX, 12/5/11.
        /// Force write the logged messages to the log file.
        /// This method can be used to preserve messages in an exception processing situation.
        /// </summary> 
        public void flushLogMessage(Receiver r, Message m)
        {
            String output;
            lock (outputData)
            {
                output = "Log\r\n" + timeStamp() + "\n" + outputData.ToString() + m;
                outputData.Clear();
            }

            Buffer<Byte> buffer = BufferPool.PopEmpty();
            UTF8Encoding encoding = new UTF8Encoding();
            buffer.setData(encoding.GetBytes(output), BufferType.UTF8_LOG);
            BufferPool.PostFull(buffer);
        }


        public void FlushLogMessage()
        {
            String output;
            lock (outputData)
            {
                output = "Log\r\n" + timeStamp() + "\n" + outputData.ToString();
                outputData.Clear();
            }

            Buffer<Byte> buffer = BufferPool.PopEmpty();
            UTF8Encoding encoding = new UTF8Encoding();
            buffer.setData(encoding.GetBytes(output), BufferType.UTF8_LOG);
            BufferPool.PostFull(buffer);
        }
    }
}
