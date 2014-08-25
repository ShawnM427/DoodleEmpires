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
        IPEndPoint _internalEndPoint;
        IPEndPoint _externalEndPoint;
        string _name;
        string _message;
        double _ping;
        int _playerCount;
        int _maxPlayerCount;

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
        public IPEndPoint InternalEndPoint
        {
            get { return _internalEndPoint; }
            set { _internalEndPoint = value; }
        }
        /// <summary>
        /// The external endpoint for this server
        /// </summary>
        public IPEndPoint ExternalEndPoint
        {
            get { return _externalEndPoint; }
            set { _externalEndPoint = value; }
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
        public double Ping
        {
            get { return _ping; }
            set { _ping = value; }
        }
        /// <summary>
        /// Gets or sets the player count for this info
        /// </summary>
        public int PlayerCount
        {
            get { return _playerCount; }
            set { _playerCount = value; }
        }
        /// <summary>
        /// Gets or sets the maximum player count for this info
        /// </summary>
        public int MaxPlayerCount
        {
            get { return _maxPlayerCount; }
            set { _maxPlayerCount = value; }
        }
        
        /// <summary>
        /// Creates a new server info
        /// </summary>
        /// <param name="name">The name of the server</param>
        /// <param name="internalEndpoint">The server's endpoint</param>
        /// <param name="message">The message for this server to display</param>
        public ServerInfo(string name, IPEndPoint internalEndpoint, string message = "")
        {
            _ip = internalEndpoint.Address;
            _name = name;
            _internalEndPoint = internalEndpoint;
            _externalEndPoint = null;
            _ping = 0;
            _message = message;
            _playerCount = 0;
            _maxPlayerCount = 4;
        }

        /// <summary>
        /// Creates a new server info
        /// </summary>
        /// <param name="name">The name of the server</param>
        /// <param name="message">The message for this server to display</param>
        public ServerInfo(string name, string message = "")
        {
            _ip = NetUtility.Resolve("localhost");
            _internalEndPoint = new IPEndPoint(_ip, GlobalNetVars.DEFAULT_PORT);
            _externalEndPoint = null;
            _name = name;
            _ping = 0;
            _message = message;
            _playerCount = 0;
            _maxPlayerCount = 4;
        }

        /// <summary>
        /// Writes this server info to a packet
        /// </summary>
        /// <param name="message">The packet to write to</param>
        public void WriteToPacket(NetOutgoingMessage message)
        {
            message.Write(Name);
            message.Write(Message);
            message.Write(PlayerCount);
            message.Write(MaxPlayerCount);
            message.Write(InternalEndPoint);
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
            int playerCount = message.ReadInt32();
            int maxPlayerCount = message.ReadInt32();
            IPEndPoint endpoint = message.ReadIPEndpoint();

            ServerInfo ret = new ServerInfo(name, endpoint, serverMessage);
            ret.PlayerCount = playerCount;
            ret.MaxPlayerCount = maxPlayerCount;
            return ret;
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
                return _name == ((ServerInfo)obj)._name;
        }

        /// <summary>
        /// Returns the hash code for this instance
        /// </summary>
        /// <returns>The hash code for this instance</returns>
        public override int GetHashCode()
        {
            return _name.GetHashCode();
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
            batch.DrawString(font, _serverInfo.InternalEndPoint.ToString(), bounds.Location.ToVector2() + new Vector2(5, 4 + font.MeasureString(" ").Y), _textColor);
            batch.DrawString(font, _serverInfo.Message, bounds.Location.ToVector2() + new Vector2(5, 4 + font.MeasureString(" ").Y * 2), _textColor);

            string text = _serverInfo.Ping.ToString("0.##") + " ms";
            batch.DrawString(font, text, bounds.Location.ToVector2() + new Vector2(bounds.Width - font.MeasureString(text).X - 5, 4), _textColor);

            text = _serverInfo.PlayerCount + "/" + _serverInfo.MaxPlayerCount;
            batch.DrawString(font, text, bounds.Location.ToVector2() + new Vector2(bounds.Width - font.MeasureString(text).X - 5, 4 + font.MeasureString(" ").Y), _textColor);
        }
    }
}
