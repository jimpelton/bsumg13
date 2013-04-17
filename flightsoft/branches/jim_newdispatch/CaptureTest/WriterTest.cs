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
            
            BufferPool<Byte> bufpool = new BufferPool<byte>(NUM_BUFFERS, (int)Math.Pow(2, 24));
            Writer testWriter = new Writer(bufpool, "Writer");
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
