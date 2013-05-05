using System;
using System.IO;
using System.Threading;
using NUnit.Framework;
using uGCapture;

namespace CaptureTest
{
    public class WriterTest
    {
        private const int NUM_BUFFERS = 10;

        [Test]
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
        }

        private string testdir = @".\TestData\";

        private BufferPool<byte> bufPool;
        private Writer testWriter;
        private Thread t;

        [SetUp]
        public void Setup()
        {

            bufPool = new BufferPool<byte>(NUM_BUFFERS, (int)Math.Pow(2,24));
            if (!Directory.Exists(testdir))
            {
                Directory.CreateDirectory(testdir);
            }
            testWriter = new Writer(bufPool, "TestWriter")
                {
                    DirectoryName = testdir
                };
            t = new Thread(() => Writer.WriteData(testWriter));
            t.Start();
        }

        [Test]
        public void MockDataWritingTest()
        {
            MockController mc = new MockController(bufPool, "MockController");
            mc.Initialize();
            mc.ManualPushData();
        }
        [TearDown]
        public void After()
        {
            testWriter.IsRunning = false;
            try
            {
                t.Join();
            }
            catch (Exception eek)
            {
                Console.WriteLine(eek.StackTrace);
            }
        }
    }
}
