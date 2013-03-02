using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;


namespace CaptureCallTest
{
    class Program
    {

        static void Main(string[] args)
        {
            Int32 initResult = ManagedSimpleCapture.managed_initMidLib(1);

            Console.WriteLine("initResult: " + initResult);

            ManagedSimpleCapture mansc = new ManagedSimpleCapture();
            mansc.managed_openTransport(0);
            AptinaController mysc = new AptinaController(mansc);

            Thread t = new Thread(() => AptinaController.go(mysc));
            t.Start();

            //wait for user to press a key, then stop.
            Console.Read();
            mysc.stop();
            t.Join();

            Console.WriteLine("Done!");
        }
    };

}

