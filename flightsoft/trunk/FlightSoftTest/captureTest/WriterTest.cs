using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using NUnit.Framework;
using captureTest;
using uGCapture;
using System.Collections;

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
        private MockController controller;

        [TestFixtureSetUp]
        public void Setup()
        {
            
        }

        private void setupWriter()
        {
            bufPool = new BufferPool<byte>(NUM_BUFFERS, 1024);

            if (Directory.Exists(testdir))
            {
                Directory.Delete(testdir, true);
            }

            testWriter = new Writer(bufPool, "TestWriter")
            {
                BasePath = testdir
            };

            if (!testWriter.Initialize())
            {
                Assert.Fail("Writer couldn't initialize.");
            }

            dp = Dispatch.Instance();
            dp.Register(testWriter);

            controller = new MockController(bufPool, "MockController");
            controller.Initialize();
            dp.Register(controller);

            t = new Thread(() => Writer.WriteData(testWriter));
            t.Start();
        }

        [Test]
        public void CreateWriterTest()
        {
            BufferPool<byte> aBufferPool = new BufferPool<byte>(NUM_BUFFERS, 1024);
            Writer w = new Writer(aBufferPool, "aWriter");
            w.BasePath = testdir;
            if (!w.Initialize())
            {
                Assert.Fail("Writer couldn't initialize");
            }
            t = new Thread(() => Writer.WriteData(w));
            t.Start();

            Thread.Sleep(1000);

            w.stop();
            t.Join(1000);
        }

        [Test]
        public void WriteMethodTest()
        {
            BufferPool<byte> aBufferPool = new BufferPool<byte>(NUM_BUFFERS, 1024);
            Writer w = new Writer(aBufferPool, "aWriter");
            w.BasePath = testdir;
            if (!w.Initialize())
            {
                Assert.Fail("Writer couldn't initialize");
            }

            Buffer<byte> b = aBufferPool.PopEmpty();
            byte[] expected = Encoding.UTF8.GetBytes("Hello Goulag!");
            b.setData(expected, BufferType.UTF8_PHIDGETS);
            aBufferPool.PostFull(b);
            Writer.DoWrite(w);

            byte[] actual = File.ReadAllBytes(testdir + Str.Dirs[DirStr.DIR_PHIDGETS] +
                              Str.Pfx[DirStr.DIR_PHIDGETS] + "_0.txt");

            Assert.AreEqual(expected, actual);
        }

        [TestCase(BufferType.UTF8_PHIDGETS, DirStr.DIR_PHIDGETS)]
        [TestCase(BufferType.UTF8_ACCEL, DirStr.DIR_ACCEL)]
        [TestCase(BufferType.UTF8_SPATIAL, DirStr.DIR_SPATIAL)]
        [TestCase(BufferType.UTF8_VCOMM, DirStr.DIR_VCOMM)]
        [TestCase(BufferType.UTF8_NI6008, DirStr.DIR_NI_DAQ)]
        public void MockDataWritingTest(BufferType bt, DirStr dirStr)
        {
            controller.ManualPushData(bt);
            byte[] expected = controller.LastData();

            //give the writer plenty of time to write and close file before we open it.
            Thread.Sleep(250);

            IList<string> files = Directory.GetFiles(testdir + Str.Dirs[dirStr]);
            Assert.IsTrue(files.Count > 0);
            foreach (string file in files)
            {
                byte[] actual = File.ReadAllBytes(testdir+Str.Dirs[dirStr]+file);
                Assert.AreEqual(expected, actual);
            }
            
        }

        //[Test]
        //[TestCase(BufferType.UTF8_ACCEL)]
        //public void MockDatWriterRandomTest(BufferType bt)
        //{
        //    int i = 0;
		   

        //    //int[] yars = new int[Str.Dirs.Count];
        //    List<byte[]> expected = new List<byte[]>();
        //    List<string> expectedFiles = new List<string>();
        //    string filename = "";
        //    Random r = new Random();

        //    while (i < 250)
        //    {
        //        BufferType type = intToBufferType((int)bt, ref filename);
        //        if (filename.Equals(""))
        //        {
        //            continue;
        //        }
        //        controller.ManualPushData(type);
        //        expected.Add(controller.LastData());
        //        expectedFiles.Add(filename);
        //        i++;
        //    }

        //        IList files = Directory.GetFiles(testdir + dir);
        //        Assert.IsNotEmpty(files);
        //        foreach (string f in files)
        //        {
        //            byte[] actual = File.ReadAllBytes(filename);
        //            Assert.AreEqual(expected, actual);
        //            Assert.Contains(f, files);
        //        }
        //    }


        //}


    public void MockDataWriterTest(BufferType bt)
    {
        List<byte[]> expected = new List<byte[]>();
        List<byte[]> expectedFiles = new List<byte[]>();

        
    }

    private BufferType intToBufferType(int i, ref string filename, ref int[] yars)
	{
	    switch (i)
        {
            case 0:
                filename = testdir + Str.Dirs[DirStr.DIR_CAMERA405] + Str.Pfx[DirStr.DIR_CAMERA405] + "_" + yars[i].ToString() + ".txt";
                Console.WriteLine(filename);
                yars[i]++;
                return BufferType.USHORT_IMAGE405;
            case 1:
                filename = testdir + Str.Dirs[DirStr.DIR_CAMERA485] + Str.Pfx[DirStr.DIR_CAMERA485] + "_" + yars[i].ToString() + ".txt";
                Console.WriteLine(filename);
                yars[i]++;
                return BufferType.USHORT_IMAGE485;
            case 2:
                filename = testdir + Str.Dirs[DirStr.DIR_ACCEL] + Str.Pfx[DirStr.DIR_ACCEL] + "_" + yars[i].ToString() + ".txt";
                Console.WriteLine(filename);
                yars[i]++;
                return BufferType.UTF8_ACCEL;
            case 3:
                filename = testdir + Str.Dirs[DirStr.DIR_SPATIAL] + Str.Pfx[DirStr.DIR_SPATIAL] + "_" + yars[i].ToString() + ".txt";
                Console.WriteLine(filename);
                yars[i]++;
                return BufferType.UTF8_SPATIAL;
            case 4:
                filename = testdir + Str.Dirs[DirStr.DIR_PHIDGETS] + Str.Pfx[DirStr.DIR_PHIDGETS] + "_" + yars[i].ToString() + ".txt";
                Console.WriteLine(filename);
                yars[i]++;
                return BufferType.UTF8_PHIDGETS;
            case 5:
                filename = testdir + Str.Dirs[DirStr.DIR_NI_DAQ] + Str.Pfx[DirStr.DIR_NI_DAQ] + "_" + yars[i].ToString() + ".txt";
                Console.WriteLine(filename);
                yars[i]++;
                return BufferType.UTF8_NI6008;
            case 6:
                filename = testdir + Str.Dirs[DirStr.DIR_UPS] + Str.Pfx[DirStr.DIR_UPS] + "_" + yars[i].ToString() + ".txt";
                Console.WriteLine("Skipping UPS test.");
                yars[i]++;
                return BufferType.UTF8_UPS;
            case 7:
                filename = testdir+Str.Dirs[DirStr.DIR_VCOMM]+Str.Pfx[DirStr.DIR_VCOMM] + "_" + yars[i].ToString() + ".txt";
                Console.WriteLine(filename);
                yars[i]++;
                return BufferType.UTF8_VCOMM;
            case 8:
                filename = "";
                Console.WriteLine("Empty cycle.");
                yars[i]++;
                return BufferType.EMPTY_CYCLE;
            case 9:
                filename = testdir + Str.Dirs[DirStr.DIR_LOGGER] + Str.Pfx[DirStr.DIR_LOGGER] + "_" + yars[i].ToString() + ".txt";
                Console.WriteLine(filename);
                yars[i]++;
                return BufferType.UTF8_LOG;
        }
	    return BufferType.EMPTY_CYCLE;
	}

 

        //[TestFixtureTearDown]
        public void After()
        {
            //cycle the buffer pool one last time to kill its worker thread.
            testWriter.IsRunning = false;
            //Buffer<byte> b = bufPool.PopEmpty();
            Buffer<byte> b = new Buffer<byte>();
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



    }
}
