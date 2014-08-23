using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Lidgren.Network;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DoodleEmpires.Engine.Utilities;

namespace DoodleEmpires.Engine.Net
{
    /// <summary>
    /// Represents basic info about a server
    /// </summary>
    public struct ServerInfo
    {
        IPAddress _ip;
        IPEndPoint _endPoint;
        string _name;
        string _message;
        float _ping;

        /// <summary>
        /// Gets the IP of this server, represented as a string
        /// </summary>
        public string IP
        {
            get { return _ip.ToString(); }
        }
        /// <summary>
        /// The network endpoint for this server
        /// </summary>
        public IPEndPoint EndPoint
        {
            get { return _endPoint; }
            set { _endPoint = value; }
        }
        /// <summary>
        /// The name of this server
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
        /// <summary>
        /// Gets or sets the server message to be displayed
        /// </summary>
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }
        /// <summary>
        /// Gets or sets the ping to the server in seconds
        /// </summary>
        public float Ping
        {
            get { return _ping; }
            set { _ping = value; }
        }
        
        /// <summary>
        /// Creates a new server info
        /// </summary>
        /// <param name="name">The name of the server</param>
        /// <param name="endpoint">The server's endpoint</param>
        public ServerInfo(string name, IPEndPoint endpoint, string message = "")
        {
            _ip = endpoint.Address;
            _name = name;
            _endPoint = endpoint;
            _ping = 0;
            _message = message;
        }

        /// <summary>
        /// Creates a new server info
        /// </summary>
        /// <param name="name">The name of the server</param>
        public ServerInfo(string name, string message = "")
        {
            _ip = NetUtility.Resolve("localhost");
            _endPoint = new IPEndPoint(_ip, GlobalNetVars.DEFAULT_PORT);
            _name = name;
            _ping = 0;
            _message = message;
        }

        /// <summary>
        /// Writes this server info to a packet
        /// </summary>
        /// <param name="message">The packet to write to</param>
        public void WriteToPacket(NetOutgoingMessage message)
        {
            message.Write(Name);
            message.Write(Message);
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
            string serverMessage = message.ReadString();
            IPEndPoint endpoint = message.ReadIPEndpoint();

            return new ServerInfo(name, endpoint, serverMessage);
        }

        /// <summary>
        /// Checks if this object is equal to another
        /// </summary>
        /// <param name="obj">The object to check against</param>
        /// <returns>True if this object is equal to the given one</returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(ServerInfo))
                return false;
            else
                return _endPoint == ((ServerInfo)obj)._endPoint;
        }
    }

    /// <summary>
    /// A ListViewItem for drawing server info
    /// </summary>
    public class ServerInfoListItem : GUI.ListViewItem
    {
        ServerInfo _serverInfo;

        /// <summary>
        /// Gets or sets the server info associated with this item
        /// </summary>
        public ServerInfo Info
        {
            get { return _serverInfo; }
            set { _serverInfo = value; }
        }

        /// <summary>
        /// Creates a new server info list item
        /// </summary>
        /// <param name="info">The info of the server to draw</param>
        public ServerInfoListItem(ServerInfo info)
        {
            _tag = info;
            _serverInfo = info;
        }

        /// <summary>
        /// Renders this list view item
        /// </summary>
        /// <param name="batch">The spritebatch to use for drawing</param>
        /// <param name="font">The font to use for rendering text for this items</param>
        /// <param name="bounds">The bounds to render in</param>
        public override void Render(SpriteBatch batch, SpriteFont font, Rectangle bounds)
        {
            batch.DrawString(font, _serverInfo.Name,bounds.Location.ToVector2() + new Vector2(5, 4), _textColor);
            batch.DrawString(font, _serverInfo.EndPoint.ToString(), bounds.Location.ToVector2() + new Vector2(5, 4 + font.MeasureString(" ").Y), _textColor);
            batch.DrawString(font, _serverInfo.Message, bounds.Location.ToVector2() + new Vector2(5, 4 + font.MeasureString(" ").Y * 2), _textColor);
        }
    }
}
