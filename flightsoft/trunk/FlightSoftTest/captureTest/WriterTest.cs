using System;
using System.IO;
using System.Threading;
using NUnit.Framework;
using captureTest;
using uGCapture;

namespace captureTest
{
    public class WriterTest
    {


        private const int NUM_BUFFERS = 10;
        private string testdir = @"C:\TestData\";
        private BufferPool<byte> bufPool;
        private Dispatch dp;
        private Writer testWriter;
        private Thread t;
        private TinyMockSender sender;

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
	    

            dp = Dispatch.Instance();
	    dp.Register(testWriter);

            t = new Thread(() => Writer.WriteData(testWriter));
            t.Start();
        }

        [Test]
        public void MockDataWritingTest()
        {
            MockController mc = new MockController(bufPool, "MockController");
            mc.Initialize();

	    dp.Register(mc);

            mc.ManualPushData();
            byte[] expected = mc.LastData();

            //give the writer plenty of time to write and close file before we open it.
	    Thread.Sleep(1000); 

            byte[] actual = File.ReadAllBytes(testdir + "Camera405_0.raw");
            Assert.AreEqual(expected, actual);
        }

        [TearDown]
        public void After()
        {
            //cycle the buffer pool one last time to kill its worker thread.
            testWriter.IsRunning = false;
            Buffer<byte> b = bufPool.PopEmpty();
            b.Type = BufferType.EMPTY_CYCLE;
            bufPool.PostFull(b);
	    
            try
            {
                t.Join(1000);
            }
            catch (Exception eek)
            {
                Console.WriteLine(eek.StackTrace);
            }
        }

        //[Test]
        //public void PhidgetsFileWritingTest1()
        //{
            
        //    BufferPool<Byte> bufpool = new BufferPool<byte>(NUM_BUFFERS, (int)Math.Pow(2, 24));
        //    Writer testWriter = new Writer(bufpool, "Writer");
        //    testWriter.FrameTime = 500;
        //    testWriter.TickerEnabled = true;
        //    for (byte i = 0; i < 200; i++)
        //    {
        //        byte[] bytes = { i++, i++, i++, i++, i++, i++, i++, i++, i++, i++, i++};
        //        Buffer<Byte> buf;
        //        buf = bufpool.PopEmpty();
        //        buf.setData(bytes, BufferType.UTF8_PHIDGETS);
        //        buf.Text = "test";
        //        testWriter.DirectoryName = "Test";
        //        bufpool.PostFull(buf);
        //    }
        //}
    }
}
