using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using uGCapture;
using System.Collections.Generic;

namespace CaptureTest
{
    [TestClass]
    public class BufferPoolTest
    {
        // fill given bufferpool with the arrays in arrs.
        private int fillBufferPool(BufferPool<byte> bp, List<byte[]> arrs)
        {
            int filled = 0;
            foreach(byte[] barr in arrs) {
                Buffer<byte> b = bp.PopEmpty();
                if (b != null)
                {
                    b.setData(barr, BufferType.USHORT_IMAGE405);
                    bp.PostFull(b);
                    filled += 1;
                }
            }
            return filled;
        }
        
        // create a list of byte[] filled with random bytes.
        private List<byte[]> byteArrays(int numbufs, int numelem)
        {
            List<byte[]> arrays = new List<byte[]>(); 
            Random r = new Random();
            for (int i = 0; i < numbufs; ++i)
            {
                byte[] barr = new byte[numelem];
                r.NextBytes(barr);
                arrays.Add(barr);
            }
            return arrays;
        }


        [TestMethod]
        public void CreateBufferPool_Test()
        {
            int numelem = 10;
            int numbuffs = 10;
            BufferPool<byte> bp = new BufferPool<byte>(numbuffs, numelem);
            Assert.AreEqual(numbuffs, bp.NumBufs);
            Assert.AreEqual(numbuffs, bp.BufElem);
            Assert.AreEqual(numbuffs, bp.EmptyCount);
            Assert.AreEqual(0, bp.FullCount);
        }


        [TestMethod]
        public void PeekFullCopy_Test()
        {
            int numelem = 10;
            int numbuffs = 1;

            BufferPool<byte> bp = new BufferPool<byte>(numbuffs, numelem);

            List<byte[]> arrs = byteArrays(bp.NumBufs, bp.BufElem);
            int numfilled = fillBufferPool(bp, arrs);

            Assert.AreEqual(bp.NumBufs, numfilled);

            Buffer<byte> actual = bp.PeekFullCopy();
            int last = arrs.Count-1;
            byte[] expected = arrs[last];
            CollectionAssert.AreEqual(expected, actual.Data);
        }

        [TestMethod]
        public void PopEmpty_Test()
        {
            BufferPool<byte> bp = new BufferPool<byte>();
            Assert.IsNull(bp.PopEmpty());
            int numelem = 10;
            int numbuffs = 2;
            bp = new BufferPool<byte>(numbuffs, numelem);
            Buffer<byte> emptyBuff = bp.PopEmpty();
            Assert.IsNotNull(emptyBuff);
        }
        [TestMethod]
        public void PopFull_Test()
        {
            int numelem = 10;
            int numbuffs = 1;

            BufferPool<byte> bp = new BufferPool<byte>(numbuffs, numelem);

            List<byte[]> arrs = byteArrays(bp.NumBufs, bp.BufElem);
            int numfilled = fillBufferPool(bp, arrs);

            Assert.AreEqual(bp.NumBufs, numfilled);

            Buffer<byte> actual = bp.PopFull();
            int last = arrs.Count-1;
            byte[] expected = arrs[last];
            CollectionAssert.AreEqual(expected, actual.Data);
        }
    } /* class BufferPoolTest */
} /* namepsace CaptureTest */
