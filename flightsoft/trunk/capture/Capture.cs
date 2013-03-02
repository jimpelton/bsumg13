using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phidgets;
using Phidgets.Events;


namespace uGCapture
{
    public class CaptureClass
    {
        PhidgetsController phidgey = null;

        public CaptureClass()
        {
            phidgey = new PhidgetsController();

        }
    }
}
