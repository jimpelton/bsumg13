using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using uGCapture;
using System.Threading;

namespace CaptureTest
{
    [TestClass]
    public class AptinaControllerTest
    {
        [TestMethod]
        public void InitMidlibTest()
        {
            //barrierSemaphore = new Semaphore(0,2);
            //barrierCounter = 0;

            // 1 is an error condition
            Int32 initResult = 1;
            //do
            //{
                //TODO: fix the memory leak this loop causes in SimpleCapture::initMidLib(). --JP
                //initResult = ManagedSimpleCapture.managed_InitMidLib(2);           
            //} while (initResult!=0);

            //dp.BroadcastLog(this, "initResult (success=0): " + initResult,1);
            //ManagedSimpleCapture mansc1 = new ManagedSimpleCapture();
            //ManagedSimpleCapture mansc2 = new ManagedSimpleCapture();
            ////mansc1.managed_OpenTransport(1);
            //mansc2.managed_OpenTransport(0);

            //AptinaController mysc1 = new AptinaController();
            //AptinaController mysc2 = new AptinaController();

            //Thread t1 = new Thread(() => AptinaController.go(mysc1));
            //Thread t2 = new Thread(() => AptinaController.go(mysc2));
            //t1.Start();
            //t2.Start();

            //mysc.stop();
            //t.Join();

            //dp.BroadcastLog(this, "Done!",1);
        }
    }
}
