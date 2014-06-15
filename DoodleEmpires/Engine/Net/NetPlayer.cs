using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace DoodleEmpires.Engine.Net
{
    /// <summary>
    /// Represents a network instance of a player
    /// </summary>
    public class NetPlayer
    {
        PlayerInfo _playerInfo;
        NetConnection _netConnection;

        /// <summary>
        /// Gets this player's information
        /// </summary>
        public PlayerInfo Info
        {
            get { return _playerInfo; }
        }
        /// <summary>
        /// Gets this player's player index
        /// </summary>
        public byte PlayerIndex
        {
            get { return _playerInfo.PlayerIndex; }
        }
        /// <summary>
        /// Gets this player's network connection
        /// </summary>
        public NetConnection NetConnection
        {
            get { return _netConnection; }
        }

        /// <summary>
        /// Creates a new network player instance
        /// </summary>
        /// <param name="info">The player's infomration</param>
        /// <param name="connection">The player's network connection</param>
        public NetPlayer(PlayerInfo info, NetConnection connection)
        {
            _playerInfo = info;
            _netConnection = connection;
        }
    }
}
