using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using uGCapture.Controller;

namespace uGCapture
{
    class Writer : ReceiverController
    {
        public Writer()
        {
            
        }

        /*
         * WriteData
         * Writes data to the disk.
         * returns false if an error occurs.
         */
        public Boolean WriteData()
        {

            return true;
        }

        public override void DoFrame(object source, ElapsedEventArgs e)
        {
            WriteData();
        }
    }
}
