using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoodleEmpires.Engine.Terrain;
using Lidgren.Network;

namespace DoodleEmpires.Engine.Net
{
    public class NetMap
    {
        VoxelTerrain _terrain;
        NetClient _netPeer;

        public byte this[int x, int y]
        {
            get { return _terrain[x, y]; }
            set
            {
                _terrain[x, y] = value;
                NetOutgoingMessage message = _netPeer.CreateMessage();
                message.Write((ushort)NetManager.PacketTypes.MapSet);
                message.Write((ushort)x);
                message.Write((ushort)y);
                message.Write(value);
                _netPeer.SendMessage(message, NetDeliveryMethod.ReliableUnordered);
            }
        }

        public NetMap(VoxelTerrain terrain, NetClient netPeer)
        {
            _terrain = terrain;
            _netPeer = netPeer;
        }

        public void HandleSetMessage(NetIncomingMessage message)
        {
            _terrain[message.ReadInt16(), message.ReadInt16()] = message.ReadByte();
        }
    }
}
