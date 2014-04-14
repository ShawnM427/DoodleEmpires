using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Lidgren.Network;

namespace DoodleEmpires.Engine.Net
{
    public static class NetManager
    {
        static volatile bool _exiting;
        static NetMap _map;
        static NetClient _netClient;

        public static bool Exiting
        {
            get{return _exiting;}
            set{_exiting = value;}
        }

        public static void BeginRead()
        {
            Thread t = new Thread(ReadIncoming);
        }

        private static void ReadIncoming()
        {
            while (!Exiting)
            {
                NetIncomingMessage message;
                if ((message = _netClient.ReadMessage()) != null)
                {
                    while (message.PositionInBytes < message.LengthBytes)
                    {
                        PacketTypes MID = (PacketTypes)message.ReadInt16();

                        switch (MID)
                        {
                            case PacketTypes.MapSet:
                                _map.HandleSetMessage(message);
                                break;
                        }
                    }
                }
            }
        }

        public enum PacketTypes : short
        {
            Connect,
            Disconnect,
            PlayerJoined,
            MapSet
        }
    }
}
