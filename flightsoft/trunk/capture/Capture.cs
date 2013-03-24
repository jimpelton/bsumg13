using System;
using System.Collections.Generic;
using System.Timers;
using Phidgets;
using Phidgets.Events;

namespace uGCapture 
{
    public class CaptureClass : ReceiverController
    {
        private static int FRAME_TIME = 500;
        private String directoryName;

        private Queue<Message> messages;
        private Dispatch dispatch;

        private PhidgetsController phidgetsController;
        private AccelerometerController a1; 
        private AccelerometerController a2;
        private Writer writer;

        private Timer ticker;

        public CaptureClass() : base()
        {
            DateTime time = DateTime.Now;              // Use current time
            directoryName = "yyyyMMMdddHHmm";          // Use this format  
            System.IO.Directory.CreateDirectory("C:\\Data\\"+directoryName);
            //stagingBuffer = new BufferPool<byte>(100);
            messages = new Queue<Message>();
            dispatch = Dispatch.Instance();

            ticker = new Timer(FRAME_TIME);
            ticker.Elapsed += new ElapsedEventHandler(DoFrame);
        }

        public override void init()
        {
            
            phidgetsController = new PhidgetsController(BufferPool);
            a1 = new AccelerometerController(BufferPool);
            a2 = new AccelerometerController(BufferPool);
            writer = new Writer(BufferPool);
        }

        public override void DoFrame(object source, ElapsedEventArgs e)
        {
           
        }

        public void LogDebugMessage(String s)
        {
            LogDebugMessage(s, 0);
        }

        public void LogDebugMessage(String s, int severtity)
        {
            dp.Broadcast
            (
                new LogMessage(this, s, severtity)
            );
        }

        //public Buffer<Byte> GetEmptyByteBuffer()
        //{
        //    return StagingBuffer.PopEmpty();
        //}

        //public void submitFilledByteBuffer(Buffer<Byte> fullbuf)
        //{
        //    StagingBuffer.PostFull(fullbuf);
        //}
    }
}
