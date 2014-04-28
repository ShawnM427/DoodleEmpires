using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace DoodleEmpires.Engine.Net
{
    public class NetPlayer
    {
        PlayerInfo _playerInfo;
        NetConnection _netConnection;

        public PlayerInfo Info
        {
            get { return _playerInfo; }
            set { _playerInfo = value; }
        }
        public byte PlayerIndex
        {
            get { return _playerInfo.PlayerIndex; }
        }
        public NetConnection NetConnection
        {
            get { return _netConnection; }
        }

        public NetPlayer(PlayerInfo info, NetConnection connection)
        {
            _playerInfo = info;
            _netConnection = connection;
        }
    }
}
