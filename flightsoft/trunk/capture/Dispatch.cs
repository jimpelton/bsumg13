using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uGCapture
{
    struct ReceiverDudes{
        Receiver dude;
        string id;
    }
    /// <summary>
    /// Maintains a list of Receivers and a Queue of Messages to send to those
    /// Receivers.
    /// To receive a message all an object must do is extend Receiver and then
    /// register with the Dispatch singleton. It will then receive any broadcasts
    /// given to the Dispatch in the same order the Dispatch has received them.
    /// </summary>
    class Dispatch
    {
        private Queue<Message> mesWait;
        private Dictionary<string, ReceiverDudes> receivers;

        private static Dispatch me=null;
        private Dispatch()
        {
            mesWait = new Queue<Message>();
            receivers = new Dictionary<string, ReceiverDudes>();
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
            throw new NotImplementedException("Register(Receiver,string) not implemented yet.");
        }

        /// <summary>
        /// Remove Receiver specified by id from the list of receivers.
        /// </summary>
        /// <param name="id">The id of the dude to remove.</param>
        public void Remove(string id)
        {
            throw new NotImplementedException("Remove(string) not implemented yet.");
        }

        /// <summary>
        /// Broadcast this message to all known receivers.
        /// </summary>
        /// <param name="m"></param>
        public void Broadcast(Message m)
        {
            throw new NotImplementedException("Broadcast(Message) not implemented yet");
        }
        /// <summary>
        /// Broadcast Message m to receiver specified by string.
        /// </summary>
        /// <param name="m">The message to broadcast.</param>
        /// <param name="receiverId">The receiver of the message.</param>
        public void Broadcast(Message m, string receiverId)
        {
            throw new NotImplementedException("Broadcast(Message,string) not implemented yet");
        }
 

    }
}
