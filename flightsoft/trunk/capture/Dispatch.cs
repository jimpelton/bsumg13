using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace uGCapture
{
    public class ReceiverDudes{
        public Receiver dude;
        public string id;
        public ReceiverDudes(Receiver dude, string id)
        {
            this.dude = dude;
            this.id = id;
        }

    }
    /// <summary>
    /// Maintains a list of Receivers and a Queue of Messages to send to those
    /// Receivers.
    /// To receive a message all an object must do is extend Receiver and then
    /// register with the Dispatch singleton. It will then receive any broadcasts
    /// given to the Dispatch in the same order the Dispatch has received them.
    /// </summary>
    public class Dispatch
    {
        private const int DISPATCH_INTERVAL = 100;
        private Queue<Message> mesWait;
        private Dictionary<string, ReceiverDudes> receivers;

        private Timer ticker = null;

        private static Dispatch me=null;
        private Dispatch()
        {
            mesWait = new Queue<Message>();
            receivers = new Dictionary<string, ReceiverDudes>();
            ticker = new Timer(DISPATCH_INTERVAL);
            ticker.Elapsed += new ElapsedEventHandler(ProcessMessages);
            ticker.Enabled = true;
        }

        public static Dispatch Instance()
        {
            if (me == null)
            {
                me = new Dispatch();
            }
            return me;
        }

        /// <summary>
        /// Add Receiver r with unique id, id, to the receivers list.
        /// </summary>
        /// <param name="r">The dude to add.</param>
        /// <param name="id">The id of the dude.</param>
        public void Register(Receiver r, string id)
        {
            ReceiverDudes rd = new ReceiverDudes(r, id);
            receivers.Add(id, rd);
        }

        /// <summary>
        /// Remove Receiver specified by id from the list of receivers.
        /// </summary>
        /// <param name="id">The id of the dude to remove.</param>
        public void Remove(string id)
        {
            receivers.Remove(id);
        }
        
        /// <summary>
        /// Broadcast this message to all known receivers.
        /// </summary>
        /// <param name="m"></param>
        public void Broadcast(Message m)
        {
            lock (mesWait)
            {
                mesWait.Enqueue(m);
            }
        }

        public void BroadcastLog(Receiver sender, string message, int severity)
        {
            Broadcast
            (
                new LogMessage(sender, message, severity)
            );
        }

        //we need to deliver the messages. At the moment it is bound to a timer. Should we do it this way?
        private void ProcessMessages(object source, ElapsedEventArgs e)
        {
            deliver();
        }

        private void deliver()
        {
            foreach (ReceiverDudes r in receivers.Values)
            {
                foreach (Message m in mesWait)
                {
                    r.dude.accept(m);
                }
            }

            mesWait.Clear();// after we send them out once lets not send them again.

        }

        /// <summary>
        /// Broadcast Message m to receiver specified by string.
        /// </summary>
        /// <param name="m">The message to broadcast.</param>
        /// <param name="receiverId">The receiver of the message.</param>
        //public void Broadcast(Message m, string receiverId)
        //{
        //    singleReceiverBroadcastMutex.WaitOne(TimeSpan.FromMilliseconds(100.0));
        //    Receiver r = receivers[receiverId].dude;
        //    r.accept(m);
        //    singleReceiverBroadcastMutex.ReleaseMutex();
        //}
 

    }
}
