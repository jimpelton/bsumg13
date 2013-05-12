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

    [TestCase(BufferType.UTF8_ACCEL)]
    [TestCase(BufferType.UTF8_NI6008)]
    [TestCase(BufferType.UTF8_PHIDGETS)]
    [TestCase(BufferType.UTF8_SPATIAL)]
    [TestCase(BufferType.UTF8_VCOMM)]
    public void TestStagingWithText(BufferType bt)
    {
        Buffer<byte> b = bufPool.PopEmpty();
        byte[] data = new byte[4096];
        r.NextBytes(data);
        b.setData(data, bt);
        bufPool.PostFull(b);
        DataSet<byte> ds = st.GetLastData();
        Assert.AreEqual(data, ds.lastData[(int)bt]);
    }

    [TestCase(BufferType.USHORT_IMAGE405)]
    [TestCase(BufferType.USHORT_IMAGE485)]
    public void TestStagingImageBuffers(BufferType bt)
    {
        Buffer<byte> b = bufPool.PopEmpty();
        byte[] data = new byte[2*2592*1944];
        r.NextBytes(data);
        b.setData(data, bt);
        bufPool.PostFull(b);
        DataSet<byte> ds = st.GetLastData();
        byte[] dtron = ds.lastData[(int) bt];
        Assert.AreEqual(data, dtron);
    }

}
}
