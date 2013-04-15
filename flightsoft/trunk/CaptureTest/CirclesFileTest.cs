using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using uGCapture;

namespace CaptureTest
{
    [TestClass]
    public class CirclesFileTest
    {
        private string testData = "data/CirclesFile/CirclesFile.txt";

        [TestMethod]
        [DeploymentItem(@"data\*")]
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
