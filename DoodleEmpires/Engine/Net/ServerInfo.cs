using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Lidgren.Network;

namespace DoodleEmpires.Engine.Net
{
    /// <summary>
    /// Represents basic info about a server
    /// </summary>
    public struct ServerInfo
    {
        string _ip;
        IPEndPoint _endPoint;
        string _name;

        /// <summary>
        /// Gets the IP of this server, represented as a string
        /// </summary>
        public string IP
        {
            get { return _ip; }
        }
        /// <summary>
        /// The network endpoint for this server
        /// </summary>
        public IPEndPoint EndPoint
        {
            get { return _endPoint; }
        }
        /// <summary>
        /// The name of this server
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Creates a new server info
        /// </summary>
        /// <param name="name">The name of the server</param>
        /// <param name="endpoint">The server's endpoint</param>
        public ServerInfo(string name, IPEndPoint endpoint)
        {
            _ip = endpoint.Address.ToString();
            _name = name;
            _endPoint = endpoint;
        }

        /// <summary>
        /// Writes this server info to a packet
        /// </summary>
        /// <param name="message">The packet to write to</param>
        public void WriteToPacket(NetOutgoingMessage message)
        {
            message.Write(Name);
            message.Write(EndPoint);
        }

        /// <summary>
        /// Reads a server info from a packet
        /// </summary>
        /// <param name="message">The packet to read from</param>
        /// <returns>A server info read from the packet</returns>
        public static ServerInfo ReadFromPacket(NetIncomingMessage message)
        {
            string name = message.ReadString();
            IPEndPoint endpoint = message.ReadIPEndpoint();

            return new ServerInfo(name, endpoint);
        }
    }
}
