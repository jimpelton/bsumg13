using System;
using System.Text;

namespace uGCapture
{
    public class Logger : Receiver
    {
        private String outputData;
        private const UInt32 MAX_LOG_FILE_LENGTH =1048576; // One megabyte.

        public Logger(String id, bool receiving = true, bool executing = true) 
            : base(id, receiving, executing)
        {
            // Set output data buffer to empty to begin.
            outputData = "";
        }


        /// <summary>
        /// LJX, 12,5,11.
        /// Returns a millisecond wall-clock timestamp in string format.
        /// </summary>
        /// <returns></returns>
        private String timeStamp()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }


        /// <summary>
        /// Main pool that every Receiver derivative writes into.
        /// </summary>
        public BufferPool<byte> BufferPool
        {
            get { return m_bufferPool; }
            protected set { m_bufferPool = value; }
        }
        private BufferPool<byte> m_bufferPool;


        /// <summary>
        /// LJX, 12/5/11.
        /// Store the accumulated log messages.
        /// Write the log file if the log is bigger than a threshold size (currently 1MB).
        /// </summary>    
        public override void exLogMessage(Receiver r, Message m)
        {

            if (outputData.Length > MAX_LOG_FILE_LENGTH)
            {
                Buffer<Byte> buffer = BufferPool.PopEmpty();
                String output = "Log\r\n";
                output += timeStamp() + " ";
                output += outputData; // Add all collected output data.
                output += m; // Finally, add the current message
                UTF8Encoding encoding = new UTF8Encoding();
                buffer.setData(encoding.GetBytes(output), BufferType.UTF8_LOG);
                BufferPool.PostFull(buffer);
                outputData = "";
            }
            else // If we're under the write threshold, write the new log message to the output data
            {
                outputData += m;
            }
        }


        /// <summary>
        /// LJX, 12/5/11.
        /// Force write the logged messages to the log file.
        /// This method can be used to preserve messages in an exception processing situation.
        /// </summary> 
        public void flushLogMessage(Receiver r, Message m)
        {
            Buffer<Byte> buffer = BufferPool.PopEmpty();
            String output = "Log\r\n";
            output += timeStamp() + " ";
            output += outputData;
            output += m;
            UTF8Encoding encoding = new UTF8Encoding();
            buffer.setData(encoding.GetBytes(output), BufferType.UTF8_LOG);
            BufferPool.PostFull(buffer);
            outputData = "";
        }
    }
}
