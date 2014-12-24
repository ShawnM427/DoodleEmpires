using DoodleEmpires.Engine.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmileyFaceWars.Engine
{
    public class Packet_PlayerInput : IPacket
    {
        double _requestedDirection;
        double _requestedSpeed;
        byte _playerID;

        public double RequestedDirection
        {
            get { return _requestedDirection; }
            set { _requestedDirection = value; }
        }
        public double RequestedSpeed
        {
            get { return _requestedSpeed; }
            set { _requestedSpeed = value; }
        }
        public byte PlayerID
        {
            get { return _playerID; }
            set { _playerID = value; }
        }
    }
}
