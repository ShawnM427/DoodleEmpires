using DoodleEmpires.Engine.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmileyFaceWars.Engine
{
    public class Packet_SendInfo : IPacket
    {
        ServerInfo _serverInfo;

        public ServerInfo ServerInfo
        {
            get { return _serverInfo; }
            set { _serverInfo = value; }
        }
    }
}
