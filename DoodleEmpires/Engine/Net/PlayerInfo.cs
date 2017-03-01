using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using MonoUI;
using Microsoft.Xna.Framework.Graphics;

namespace DoodleEmpires.Engine.Net
{
    /// <summary>
    /// Represents a player's information
    /// </summary>
    public class PlayerInfo : INetworkable
    {
        string _userName;
        Color _flagColor;
        byte _playerID;
        NetConnection _connection;

        /// <summary>
        /// Gets or sets this player's playerID
        /// </summary>
        public byte PlayerIndex
        {
            get { return (byte)_playerID; }
            set { _playerID = value; }
        }
        /// <summary>
        /// Gets this player's name
        /// </summary>
        public string UserName
        {
            get { return _userName; }
        }
        /// <summary>
        /// Gets this player's network connection
        /// </summary>
        public NetConnection Connection
        {
            get { return _connection; }
        }

        public PlayerInfo()
        {
        }
        
        /// <summary>
        /// Creates a new player info
        /// </summary>
        /// <param name="userName">The player's name</param>
        public PlayerInfo(string userName)
        {
            _userName = userName;
        }

        /// <summary>
        /// Creates a new player info
        /// </summary>
        /// <param name="userName">The player's name</param>
        /// <param name="connection">The player's network connection</param>
        public PlayerInfo(string userName, NetConnection connection)
        {
            _userName = userName;
            _connection = connection;
        }

        /// <summary>
        /// Writes this player info to a packet
        /// </summary>
        /// <param name="p">The packet to write to</param>
        public void WriteToPacket(NetOutgoingMessage p)
        {
            p.Write(PlayerIndex, 8);

            p.Write(_flagColor.R);
            p.Write(_flagColor.G);
            p.Write(_flagColor.B);
            p.Write(_flagColor.A);

            p.Write(_userName);
        }

        /// <summary>
        /// Reads a player info from a network packet
        /// </summary>
        /// <param name="p">The packet to read from</param>
        /// <returns>A player info read from the packet</returns>
        public static PlayerInfo ReadFromPacket(NetIncomingMessage p)
        {
            byte playerIndex = p.ReadByte(8);

            byte R = p.ReadByte(8);
            byte G = p.ReadByte(8);
            byte B = p.ReadByte(8);
            byte A = p.ReadByte(8);

            string userName = p.ReadString();

            PlayerInfo r = new PlayerInfo(userName);
            r._flagColor = new Color(R, G, B, A);
            r.PlayerIndex = playerIndex;

            return r;
        }

        public INetworkable Read(NetIncomingMessage msg)
        {
            return ReadFromPacket(msg);
        }

        public void Write(NetOutgoingMessage msg)
        {
            WriteToPacket(msg);
        }
    }

    /// <summary>
    /// A ListViewItem for drawing player info
    /// </summary>
    public class PlayerInfoListItem : ListViewItem
    {
        /// <summary>
        /// Gets or sets the player info associated with this item
        /// </summary>
        public PlayerInfo Info
        {
            get { return (PlayerInfo)Tag; }
            set { Tag = value; }
        }

        /// <summary>
        /// Creates a new player info list item
        /// </summary>
        /// <param name="info">The info of the player to draw</param>
        public PlayerInfoListItem(PlayerInfo info)
        {
            _tag = info;
            Text = info?.UserName;
        }

        /// <summary>
        /// Renders this list view item
        /// </summary>
        /// <param name="batch">The spritebatch to use for drawing</param>
        /// <param name="font">The font to use for rendering text for this items</param>
        /// <param name="bounds">The bounds to render in</param>
        public override void Render(SpriteBatch batch, SpriteFont font, Rectangle bounds)
        {
            batch.DrawString(font, ((PlayerInfo)Tag).UserName, bounds.Location.ToVector2() + new Vector2(5, 4), _textColor);
        }
    }
}
