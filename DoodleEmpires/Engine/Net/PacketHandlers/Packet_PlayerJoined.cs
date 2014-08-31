using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleEmpires.Engine.Net.PacketHandlers
{
    public class Packet_PlayerJoined : IPacket
    {
        PlayerInfo _info;

        public PlayerInfo Info
        {
            get { return _info; }
            set { _info = value; }
        }

        public Packet_PlayerJoined()
        {
            _info = null;
        }

        public Packet_PlayerJoined(PlayerInfo info)
        {
            _info = info;
        }
    }
}
