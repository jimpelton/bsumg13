using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phidgets;
using Phidgets.Events;

namespace uGCapture
{
    public class PhidgetsController
    {

        Accelerometer accelCapture = null;
        Accelerometer accelFiltered = null;

        public PhidgetsController()
        {
            try
            {

            }
            catch (PhidgetException ex)
            {

                Console.WriteLine(ex.Description);
            }
        }
    }
}
