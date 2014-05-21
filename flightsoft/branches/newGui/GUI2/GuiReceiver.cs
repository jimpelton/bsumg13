using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using uGCapture;

namespace GUI2
{
    class GuiReceiver : Receiver
    {
        public delegate void UpdateComponent(Queue<String> myQ);
        public Thread oThread;
        public Form1 f;
        public int delegateIndex;
        public Queue<String> myQ = new Queue<String>();
        public GuiReceiver(Form1 f, int delegateIndex, string id, bool receiving = true)
            : base(id, receiving)
        {
            this.f = f;
            this.delegateIndex = delegateIndex;
            oThread = new Thread(new ThreadStart(pollQueue));
            dp.Register(this);
        }
        //public override void exMessage(Receiver r, uGCapture.Message m)
        //{
        //    lock (myQ)
        //    {
        //        //myQ.enqueue(message);
        //    }
        //}
        public void Start()
        {
            oThread.Start();
        }
        private void pollQueue()
        {
            while (true)
            {
                lock (myQ)
                {
                    Queue<String> q = new Queue<String>();
                    q.Enqueue("blueberry");
                    f.Invoke(f.updaters[delegateIndex],
                               new Object[] { q });
                }
            }
        }
    }
}
