using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoodleEmpires.Engine.Net
{
    public class NetPlayer
    {
        PlayerInfo _playerInfo;

        public PlayerInfo Info
        {
            get { return _playerInfo; }
            set { _playerInfo = value; }
        }
        public byte PlayerIndex
        {
            get { return _playerInfo.PlayerIndex; }
        }

        public NetPlayer(PlayerInfo info)
        {
            _playerInfo = info;
        }
    }
}
