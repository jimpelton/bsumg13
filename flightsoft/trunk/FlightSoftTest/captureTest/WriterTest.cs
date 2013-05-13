using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private MockController controller;

        [TestFixtureSetUp]
        public void Setup()
        {
            bufPool = new BufferPool<byte>(NUM_BUFFERS, (int)Math.Pow(2, 24));
            if (Directory.Exists(testdir))
            {
                Directory.Delete(testdir, true);
            }

            Directory.CreateDirectory(testdir);
            Directory.CreateDirectory(testdir + "NI6008");
            Directory.CreateDirectory(testdir + "Phidgets");
            Directory.CreateDirectory(testdir + "Accel");
            Directory.CreateDirectory(testdir + "Spatial");
            Directory.CreateDirectory(testdir + "Barometer");
            Directory.CreateDirectory(testdir + "Camera405");
            Directory.CreateDirectory(testdir + "Camera485");
            Directory.CreateDirectory(testdir + "UPS");

            testWriter = new Writer(bufPool, "TestWriter")
                {
                    DirectoryName = testdir
                };

            dp = Dispatch.Instance();
            dp.Register(testWriter);

            controller = new MockController(bufPool, "MockController");
            controller.Initialize();
            dp.Register(controller);

            t = new Thread(() => Writer.WriteData(testWriter));
            t.Start();
        }

        [TestCase(BufferType.UTF8_PHIDGETS, "Phidgets\\", "Phidgets_0.txt")]
        [TestCase(BufferType.UTF8_ACCEL, "Accel\\", "Accel_0.txt")]
        [TestCase(BufferType.UTF8_SPATIAL, "Spatial\\", "Spatial_0.txt")]
        [TestCase(BufferType.UTF8_VCOMM, "Barometer\\", "Barometer_0.txt")]
        [TestCase(BufferType.USHORT_IMAGE405, "Camera405\\", "Camera405_0.raw")]
        [TestCase(BufferType.UTF8_NI6008, "NI6008\\", "NI6008_0.txt")]
        [TestCase(BufferType.USHORT_IMAGE485, "Camera485\\", "Camera485_0.raw")]
        public void MockDataWritingTest(BufferType bt, string dir, string fpfx)
        {
            controller.ManualPushData(bt);
            byte[] expected = controller.LastData();

            //give the writer plenty of time to write and close file before we open it.
            Thread.Sleep(250);

            int numFiles = Directory.GetFiles(testdir + dir).Length;
            Assert.AreEqual(1,numFiles);

            byte[] actual = File.ReadAllBytes(testdir + "\\" + dir + fpfx);
            Assert.AreEqual(expected, actual);
        }

		[Test]
        public void MockDatWriterRandomTest()
        {
            int i = 0;
            int[] yars = new int[9];
            string filename = "";
            Random r = new Random();

            while (i < 25)
            {
                BufferType type = intToBufferType(r.Next(9), ref filename, ref yars);
                if (filename.Equals(""))
                {
                    continue;
                }
                controller.ManualPushData(type);
                byte[] expected = controller.LastData();
                Thread.Sleep(250);

                byte[] actual = File.ReadAllBytes(filename);
                Assert.AreEqual(expected, actual);
                i++;
            }
        }


    private BufferType intToBufferType(int i, ref string filename, ref int[] yars)
	{
	    switch (i)
        {
            case 0:
                filename = testdir+"Camera405\\Camera405_" + yars[i].ToString() + "_0.raw";
                Console.WriteLine(filename);
                yars[i]++;
                return BufferType.USHORT_IMAGE405;
            case 1:
                filename = testdir+"Camera485\\Camera485_" + yars[i].ToString() + "_0.raw";
                Console.WriteLine(filename);
                yars[i]++;
                return BufferType.USHORT_IMAGE485;
            case 2:
                filename = testdir+"Accel\\Accel_" + yars[i].ToString() + ".txt";
                Console.WriteLine(filename);
                yars[i]++;
                return BufferType.UTF8_ACCEL;
            case 3:
                filename = testdir+"Spatial\\Spatial_" + yars[i].ToString() + ".txt";
                Console.WriteLine(filename);
                yars[i]++;
                return BufferType.UTF8_SPATIAL;
            case 4:
                filename = testdir+"Phidgets\\Phidgets_" + yars[i].ToString() + ".txt";
                Console.WriteLine(filename);
                yars[i]++;
                return BufferType.UTF8_PHIDGETS;
            case 5:
                filename = testdir+"NI6008\\NI6008_" + yars[i].ToString() + ".txt";
                Console.WriteLine(filename);
                yars[i]++;
                return BufferType.UTF8_NI6008;
            case 6:
                filename = ""; //testdir+"UPS\\UPS_" + yars[i].ToString() + ".txt";
                Console.WriteLine("Skipping UPS test.");
                yars[i]++;
                return BufferType.UTF8_UPS;
            case 7:
                filename = testdir+"Barometer\\Barometer_" + yars[i].ToString() + ".txt";
                Console.WriteLine(filename);
                yars[i]++;
                return BufferType.UTF8_VCOMM;
            case 8:
                filename = "";
                Console.WriteLine("Empty cycle.");
                yars[i]++;
                return BufferType.EMPTY_CYCLE;
        }
	    return BufferType.EMPTY_CYCLE;
	}

        [TestFixtureTearDown]
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
