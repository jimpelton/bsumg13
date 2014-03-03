using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uGCapture;

namespace clui
{
    class CLUI : Receiver
    {
        private CaptureClass cls;
        //private CaptureClass cc = new CaptureClass();
        public CLUI(string id, bool receiving = true) 
            : base(id, receiving)
        {

        }

        public void init()
        {
            dp.Register(this);
            
            cls = new CaptureClass("CaptureClass")
                {
                    StorageDir = @"C:\Data\"
                };
            cls.init();
        }


        public override void exLogMessage(Receiver r, Message m)
        {
            LogMessage mes = m as LogMessage;
            if (mes == null) return;

            Console.WriteLine(mes);
        }
    }
}
