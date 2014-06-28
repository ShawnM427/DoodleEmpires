using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Net
{
    /// <summary>
    /// Represents a player's information
    /// </summary>
    public class PlayerInfo
    {
        string _userName;
        Color _flagColor;
        PlayerID _playerID;
        NetConnection _connection;

        /// <summary>
        /// Gets or sets this player's playerID
        /// </summary>
        public byte PlayerIndex
        {
            get { return (byte)_playerID; }
            set { _playerID = (PlayerID)value; }
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
            p.Write(PlayerIndex);

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
    }
}
