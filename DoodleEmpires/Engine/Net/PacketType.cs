using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoodleEmpires.Engine.Net
{
    public interface IPacket
    {
        /// <summary>
        /// gets or sets the delivery method to use for this packet type
        /// </summary>
        NetDeliveryMethod DeliveryMethod { get; set; }
        /// <summary>
        /// Gets or sets the target reciepient for this packet
        /// </summary>
        NetConnection Reciepient { get; set; }
        /// <summary>
        /// Gets or sets wheter this packet should be sent to all clients
        /// </summary>
        bool SendToAll { get; set; }
        /// <summary>
        /// Gets or sets the ID of the packet to handle
        /// </summary>
        int PacketID { get; set; }

        /// <summary>
        /// Handles an incoming network message
        /// </summary>
        /// <param name="msg">The message to handle</param>
        void Handle(NetIncomingMessage msg);

        /// <summary>
        /// Prepares an outgoing message
        /// </summary>
        /// <param name="msg">The message to write data to</param>
        void Send(NetOutgoingMessage msg);
    }
}
