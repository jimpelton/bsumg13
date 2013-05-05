using System;
using NUnit.Framework;
using uGCapture;

namespace CaptureTest
{
    public class CirclesFileTest
    {
        private string testData = "CirclesFile.txt";

        [Test]
        public void ParseTest()
        {
            CirclesFile cf = new CirclesFile();
            int i = cf.Open(testData);

            Center actual = cf[0];
            Assert.AreEqual(655, actual.X);
            Assert.AreEqual(215, actual.Y);

            Assert.AreEqual(10, i);

        }
    }
}
