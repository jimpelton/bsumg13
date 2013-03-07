using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uGCapture
{
    public class LogMessage /*: Message*/
    {

        public long time;
        public String message;
        public int severity;

        public LogMessage(String m)
        {
            time = System.DateTime.Now.Ticks;
            message = m;
            severity = 0;
        }

        public LogMessage(String m, int s) 
        {
            time = System.DateTime.Now.Ticks;
            message = m;
            severity = s;
        }

    }
}
