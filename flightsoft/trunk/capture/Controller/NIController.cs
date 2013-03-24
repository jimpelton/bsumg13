using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using uGCapture.Controller;

//using NationalInstruments;

namespace uGCapture
{
    public class NIController : ReceiverController, IController
    {
        public override void DoFrame(object source, ElapsedEventArgs e)
        {

        }
    }
}
