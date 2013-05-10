using System;
using NUnit.Framework;
using uGCapture;

namespace captureTest
{
    [TestFixture]
    public class StagingTest
    {

    private BufferPool<byte> bufPool;
    private const int NUM_BUFFERS = 10;
    private Staging<byte> st;
    private Random r;

    [SetUp]
    public void Setup()
    {
        st = new Staging<byte>(2 * 2592 * 1944);
        bufPool = new BufferPool<byte>(NUM_BUFFERS, (int)Math.Pow(2, 24), st);
        r = new Random();
    }

	[TestCase(BufferType.USHORT_IMAGE405)]
	[TestCase(BufferType.USHORT_IMAGE485)]
	[TestCase(BufferType.UTF8_ACCEL)]
	[TestCase(BufferType.UTF8_NI6008)]
	[TestCase(BufferType.UTF8_PHIDGETS)]
	[TestCase(BufferType.UTF8_SPATIAL)]
	[TestCase(BufferType.UTF8_VCOMM)]
    public void TestStagingWithBufferPool(BufferType bt)
	{
	    Buffer<byte> b = bufPool.PopEmpty();
	    int sz = r.Next(1, bufPool.BufElem);
	    byte[] data = new byte[sz];
	    r.NextBytes(data);
	    b.setData(data, bt);
	    bufPool.PostFull(b);
	    byte[] actual = st.ImageData(0);
	    Assert.AreEqual(data, actual);
	}
}
}
