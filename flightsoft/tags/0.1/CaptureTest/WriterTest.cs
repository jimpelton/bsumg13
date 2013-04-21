using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using uGCapture;

namespace CaptureTest
{
    [TestClass]
    public class WriterTest
    {
        private const int NUM_BUFFERS = 10;
        [TestMethod]
        public void PhidgetsFileWritingTest1()
        {
            
            uGCapture.BufferPool<Byte> bufpool = new BufferPool<byte>(NUM_BUFFERS, (int)Math.Pow(2, 24));
            uGCapture.Writer testWriter = new Writer(bufpool);
            testWriter.FrameTime = 500;
            testWriter.TickerEnabled = true;
            for (byte i = 0; i < 200; i++)
            {
                byte[] bytes = { i++, i++, i++, i++, i++, i++, i++, i++, i++, i++, i++};
                Buffer<Byte> buf;
                buf = bufpool.PopEmpty();
                buf.setData(bytes, BufferType.UTF8_PHIDGETS);
                buf.Text = "test";
                testWriter.DirectoryName = "Test";
                bufpool.PostFull(buf);
            }
            //while (bufpool.FullCount>0) ;
        }
    }
}
