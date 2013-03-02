using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phidgets;
using Phidgets.Events;


namespace capture
{
    public class CaptureClass
    {
        PhidgetsController phidgey = null;

        CaptureClass()
        {
            phidgey = new PhidgetsController();

        }
    }
}
