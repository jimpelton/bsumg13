﻿// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-04-13                                                                      
// ******************************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace uGCapture
{
    /// <summary>
    /// A ReceiverIdPair manages a Receiver's own message queue, and 
    /// associates it with a string identifier.
    ///
    /// It also contains a reference to a Thread that runs the
    /// worker method Receiver.ExecuteMessageQueue().
    /// </summary>
    public class ReceiverIdPair
    {
        public Receiver Receiver
        {
            get { return m_rec; }
        }
        private Receiver m_rec;

        public string Id
        {
            get { return m_rec.Id; }
        }

        public BlockingCollection<Message> MesWait
        {
            get { return m_mesWait; }
        }
        private BlockingCollection<Message> m_mesWait;

        public Thread T { get; set; }

        public ReceiverIdPair(Receiver rec)
        {
            m_rec = rec;
            m_mesWait = new BlockingCollection<Message>();
        }

        public void Enqueue(Message m)
        {
            m_mesWait.Add(m);
        }

        /// <summary>
        /// Dequeue a message from the message queue.
        /// </summary>
        /// <returns>The message dequeued.</returns>
        public Message Dequeue()
        {
            Message rval;
            rval = m_mesWait.Take(); 

            //Console.WriteLine("Dequeued message: type [{0}], Sender [{1}] Receiver [{2}].", 
            //    rval.GetType(), rval.Sender.Id, Id);

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
        private ConcurrentDictionary<string, ReceiverIdPair> m_receiversMap;
        private static Dispatch me;
        
        public static Scheduler Scheduler
        {
            get { return m_sch; }
        }
        private static Scheduler m_sch;

        /// <summary>
        /// Millis to wait until thread joins timeout.
        /// </summary>
        public int ThreadJoinTimeoutIntervalMillis
        {
            set { m_joinTimeOutInterval = value; }
            get { return m_joinTimeOutInterval;  }
        }
        private int m_joinTimeOutInterval = 500;

        private Dispatch()
        {
            m_receiversMap = new ConcurrentDictionary<string, ReceiverIdPair>();
        }

        public static Dispatch Instance()
        {
            if (me == null)
            {
                me = new Dispatch();
                m_sch = new Scheduler("Scheduler", false);
                Console.WriteLine("Dispatch Created.");
            }
            return me;
        }

        public void CleanUpMessageThreads()
        {
            Console.Error.WriteLine("Cleaning up messaging threads.");
            ICollection<ReceiverIdPair> rips = m_receiversMap.Values;
            foreach (ReceiverIdPair pair in rips)
            {
                pair.Receiver.IsExecuting = false;
                pair.Receiver.IsReceiving = true;
            }
            
            Broadcast(new EmptyCycleMessage(null));

            foreach (ReceiverIdPair rip in rips)
            {
                try
                {
                    rip.T.Join(ThreadJoinTimeoutIntervalMillis);
                    if (rip.T.IsAlive)
                    {
                        Console.Error.WriteLine("A message thread wasn't joined.");
                    }
                }
                catch (ThreadStateException eek)
                {
                    Console.WriteLine(eek.StackTrace);
                }
                catch (ArgumentOutOfRangeException eek)
                {
                    Console.WriteLine(eek.StackTrace);
                }
            }

        }

        //public void StartAllExecuting()
        //{
        //    foreach (ReceiverIdPair pair in m_receiversMap.Values)
        //    {
        //        pair.Receiver.IsReceiving = true;
        //        pair.Receiver.IsExecuting = true;
        //        pair.T = new Thread(() => ExecuteMessageQueue(pair.Receiver));
        //        pair.T.Start();
        //        if (pair.T.IsAlive)
        //        {
        //            Console.WriteLine("[{0}] message thread started.", pair.Id);
        //        }
        //    }
        //}

        //public void psauce()
        //{
        //    Receiver q;
        //    q.IsExecuting = true;
            
        //}

        /// <summary>
        /// Add Receiver r with unique id, id, to the m_receiversMap list.
        /// </summary>
        /// <param name="r">The receiver to add.</param>
        public void Register(Receiver r)
        {
            ReceiverIdPair p = m_receiversMap.GetOrAdd(r.Id, makeNewQueue(r));
            p.Receiver.IsExecuting = true;            
            p.T = new Thread(() => ExecuteMessageQueue(r));
            p.T.Name = "mes_thread" + r.Id;
            p.T.Start();

            Console.WriteLine("Dispatch: Registered Id: [{0}]", r.Id);
        }

        public void RegisterAsLogger(Receiver r) { }

        private ReceiverIdPair makeNewQueue(Receiver r)
        {
            return new ReceiverIdPair(r);
        }

        /// <summary>
        /// Remove Receiver specified by id from the list of Receivers.
        /// </summary>
        /// <param name="id">The id of the rec to remove.</param>
        public void Remove(string id)
        {
            ReceiverIdPair pair;
            m_receiversMap.TryRemove(id, out pair);
            if (pair != null)
            {
                try
                {
                    pair.T.Join(ThreadJoinTimeoutIntervalMillis);
                }
                catch (ThreadStateException eek)
                {
                    Console.Error.WriteLine(eek.StackTrace);
                }
                catch (ArgumentOutOfRangeException eek)
                {
                    Console.Error.WriteLine(eek.StackTrace);
                }
            }
        }



      
        /// <summary>
        /// Broadcast this message to all known Receivers.
        /// </summary>
        /// <param name="m">The message to enqueue.</param>
        public void Broadcast(Message m)
        {
            ICollection<ReceiverIdPair> receivers = 
                m.GetSpecificReceivers() ?? m_receiversMap.Values;

            Parallel.ForEach(receivers,
                                 (q) =>
                                     {
                                         if (q.Receiver.IsReceiving)
                                             q.Enqueue(m);
                                     });
        }

        /// <summary>
        /// Broadcast a log message to the receiver, specified by its id.
        /// This method simply generates a new LogMessage. Calling BroadcastLog is
        /// identical to calling Broadcast(new LogMessage(...)).
        /// </summary>
        /// <param name="toId">Id of the receiving Receiver</param>
        /// <param name="m">Message to send.</param>
        public void BroadcastLog(Receiver sender, string message, int severity)
        {
            Broadcast
                (
                    new LogMessage(sender, message, severity)
                );
        }

        public void BroadcastLog(Receiver sender, string message, Status severity)
        {
            Broadcast
                (
                    new LogMessage(sender, message, severity)
                );
        }

        public void BroadcastLog(Receiver sender, ErrStr message, Status severity)
        {
            Broadcast
                (
                    new LogMessage(sender, message, severity)
                );
        }

        public void BroadcastLog(Receiver sender, Status severity, params string[] messages)
        {
            StringBuilder mes = new StringBuilder(256);
            foreach (string s in messages)
            {
                mes.Append(s).Append(" ");
            }
            Broadcast(new LogMessage(sender, mes.ToString(), severity));
        }

        /// <summary>
        /// Broadcast message to the receiver, specified by its id.
        /// </summary>
        /// <param name="toId">Id of the receiving Receiver</param>
        /// <param name="m">Message to send.</param>
        public void BroadcastTo(Receiver dest, Message m)
        {
            if (dest.IsReceiving)
            {
                m_receiversMap[dest.Id].Enqueue(m);
            }
        }

        /// <summary>
        /// Broadcast message to the receiver, specified by its id.
        /// </summary>
        /// <param name="toId">Id of the receiving Receiver</param>
        /// <param name="m">Message to send.</param>
        public void BroadcastTo(string toId, Message m)
        {
            m_receiversMap[toId].Enqueue(m);
        }

        /// <summary>
        /// Dequeue the next Message for the specified receiver. 
        /// Blocks until a new message is available.
        /// </summary>
        /// <param name="r">The Receiver to dequeue the message from</param>
        /// <returns>The next message in the queue</returns>
        public Message Next(Receiver r)
        {
            return m_receiversMap[r.Id].Dequeue();
        }

        /// <summary>
        /// Dequeue the next Message for the receiver specified by id.
        /// Blocks until a new message is available.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The next message</returns>
        public Message Next(string id)
        {
            return m_receiversMap[id].Dequeue();
        }

        public bool HasNext(Receiver r)
        {
            return (m_receiversMap[r.Id].MesWait.Count != 0);
        }
        
        /// <summary>
        /// Worker method for Message execution.
        /// </summary>
        /// <param name="r"></param>
        public static void ExecuteMessageQueue(Receiver r)
        {
            Dispatch dp = Instance();
            while (true)
            {
                if (!r.IsExecuting && !dp.HasNext(r))
                {
                    return;
                }
                Message m = dp.Next(r.Id);
                m.execute(r);
                //Instance().Next(r.Id).execute(r);
                //Console.WriteLine("Receiver: {0} Executed {1}", r.Id, m.GetType());
            }

        }

    }

}