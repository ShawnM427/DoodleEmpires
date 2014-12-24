using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoodleEmpires.Engine.Net
{
    public abstract class IPacket
    {
        /// <summary>
        /// Gets the sequence channel to send this packet type over
        /// This can be usefull when the DeliveryMethod is ReliableSequenced to
        /// not drop packets containing important information
        /// </summary>
        public virtual int SequenceChannel
        {
            get { return 0; }
        }
        /// <summary>
        /// Gets the delivery method to use for this packet type
        /// By default this is NetDeliveryMethod.Unreliable
        /// </summary>
        public virtual NetDeliveryMethod DeliveryMethod
        {
            get
            {
                return NetDeliveryMethod.Unreliable;
            }
        }
        /// <summary>
        /// Gets or sets the target reciepient for this packet
        /// </summary>
        public virtual NetConnection Reciepient { get; set; }
        /// <summary>
        /// Gets or sets wheter this packet should be sent to all clients
        /// </summary>
        public virtual bool SendToAll { get; set; }
    }
}
