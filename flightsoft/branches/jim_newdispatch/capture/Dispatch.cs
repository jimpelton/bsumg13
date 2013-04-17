// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-04-13                                                                      
// ******************************************************************************

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace uGCapture
{
    internal class ReceiverIdPair
    {
        public Receiver Receiver
        {
            get { return m_dude; }
        }
        private Receiver m_dude;

        public string Id
        {
            get { return m_dude.Id; }
        }

        public BlockingCollection<Message> MesWait
        {
            get { return mesWait; }
        }
        private BlockingCollection<Message> mesWait;

        public ReceiverIdPair(Receiver dude)
        {
            m_dude = dude;
            mesWait = new BlockingCollection<Message>();
        }

        public Thread T { get; set; }

        public void Enqueue(Message m)
        {
            mesWait.Add(m);
            Console.WriteLine("Enqueued message: type [{0}], Sender [{1}] Receiver [{2}].", 
                m.GetType(), m.Sender.Id, Id);
        }

        public Message Dequeue()
        {
            Message rval;
            rval = mesWait.Take(); //(out rval);

            Console.WriteLine("Dequeued message: type [{0}], Sender [{1}] Receiver [{2}].", 
                rval.GetType(), rval.Sender.Id, Id);

            return rval;
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
        private ConcurrentDictionary<string, ReceiverIdPair> receivers;
        private static Dispatch me = null;

        private Dispatch()
        {
            receivers = new ConcurrentDictionary<string, ReceiverIdPair>();
        }

        public static Dispatch Instance()
        {
            if (me == null)
            {
                me = new Dispatch();
                Console.WriteLine("Dispatch Created.");
            }
            return me;
        }

        /// <summary>
        /// Add Receiver r with unique id, id, to the receivers list.
        /// </summary>
        /// <param name="r">The dude to add.</param>
        /// <param name="id">The id of the dude.</param>
        public void Register(Receiver r)
        {
            ReceiverIdPair p = receivers.GetOrAdd(r.Id, makeNewQueue(r));
            p.T = new Thread(() => Receiver.ExecuteMessageQueue(r));
            p.T.Start();

            //try
            //{
            //    Parallel.Invoke(() => Receiver.ExecuteMessageQueue(r));
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.StackTrace);
            //}
            Console.WriteLine("Dispatch: Registered Id: [{0}]", r.Id);
        }

        private ReceiverIdPair makeNewQueue(Receiver r)
        {
            return new ReceiverIdPair(r);
        }

        /// <summary>
        /// Remove Receiver specified by id from the list of receivers.
        /// </summary>
        /// <param name="id">The id of the dude to remove.</param>
        //public void Remove(string id)
        //{
        //    receivers.
        //}
        /// <summary>
        /// Broadcast this message to all known receivers.
        /// </summary>
        /// <param name="m">The message to enqueue.</param>
        public void Broadcast(Message m)
        {
            Parallel.ForEach(receivers, q => q.Value.Enqueue(m));
        }

        public void BroadcastLog(Receiver sender, string message, int severity)
        {
            Broadcast
                (
                    new LogMessage(sender, message, severity)
                );
        }

        public void BroadcastTo(Receiver dest, Message m)
        {
            if (dest.IsReceiving)
            {
                receivers[dest.Id].Enqueue(m);
            }
        }

        public void BroadcastTo(string toId, Message m)
        {
            receivers[toId].Enqueue(m);
        }

        public Message Next(Receiver r)
        {
            return receivers[r.Id].Dequeue();
        }

        public Message Next(string id)
        {
            return receivers[id].Dequeue();
        }

        //we need to deliver the messages. At the moment it is bound to a timer. Should we do it this way?
        //private void ProcessMessages(object source, ElapsedEventArgs e)
        //{
        //    deliver();
        //}

        //private void deliver()      
        //{

        //            foreach (ReceiverIdPair r in receivers.Values)

        //            {
        //                foreach (Message m in mesWait)
        //                {
        //                    r.dude.accept(m);
        //                }
        //            }

        //            mesWait.Clear(); // after we send them out once lets not send them again.
        //        }
        //    }
        //    catch (InvalidOperationException e)
        //    {
        //        Console.WriteLine(e.StackTrace);
        //    }
        //}

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