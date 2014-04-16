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
        static NetClient _netClient;
        static GameClient _game;

        public static void Initialize(GameClient client)
        {
            _game = client;
        }

        public static bool Exiting
        {
            get { return _exiting; }
            set { _exiting = value; }
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
                        _game.HandleNetMassage(message);
                    }
                }
            }
        }

        public static NetOutgoingMessage BuildMessage()
        {
            return _netClient.CreateMessage();
        }

        public static void SendMessage(NetOutgoingMessage message, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.ReliableUnordered)
        {
            _netClient.SendMessage(message , deliveryMethod);
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
