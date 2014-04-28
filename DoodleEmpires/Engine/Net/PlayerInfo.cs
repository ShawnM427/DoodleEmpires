using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Net
{
    public class PlayerInfo
    {
        string _userName;
        Color _flagColor;
        PlayerID _playerID;
        NetConnection _connection;

        public byte PlayerIndex
        {
            get { return (byte)_playerID; }
            set { _playerID = (PlayerID)value; }
        }
        public string UserName
        {
            get { return _userName; }
        }
        public NetConnection Connection
        {
            get { return _connection; }
        }

        public PlayerInfo(string userName)
        {
            _userName = userName;
        }

        public PlayerInfo(string userName, NetConnection connection)
        {
            _userName = userName;
            _connection = connection;
        }

        public void WriteToPacket(NetOutgoingMessage p)
        {
            p.Write(PlayerIndex);

            p.Write(_flagColor.R);
            p.Write(_flagColor.G);
            p.Write(_flagColor.B);
            p.Write(_flagColor.A);

            p.Write(_userName);
        }

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
