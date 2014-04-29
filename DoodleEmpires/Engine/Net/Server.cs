using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Lidgren.Network;
using System.Threading;
using System.Net;
using DoodleEmpires.Engine.Terrain;
using System.IO;

namespace DoodleEmpires.Engine.Net
{
    public class GameServer
    {
        NetServer _server;
        ServerInfo _serverInfo;

        NetPlayer[] _players = new NetPlayer[4];
        Dictionary<NetConnection, byte> _playerConnections = new Dictionary<NetConnection, byte>();

        ServerMap _map;

        TileManager _tileManager;

        public bool Exiting
        {
            get;
            set;
        }

        public void Run(string[] args)
        {
            int port = GlobalNetVars.DEFAULT_PORT;
            _serverInfo = 
                new ServerInfo(args.Length > 0 ? args[0] : GlobalNetVars.DEFAULT_SERVERNAME);

            if (args.Length > 1)
                port = int.Parse(args[1]);
            
            Console.WriteLine("Opening server");
            Console.WriteLine("Server name set to \"{0}\"", _serverInfo.Name);
            NetPeerConfiguration config = new NetPeerConfiguration("DoodleEmpires");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.Port = port;
            config.EnableUPnP = true;
            Console.WriteLine("Net configuration complete");

            try
            {
                Console.WriteLine("Attemting to bind to port {0}", port);

                _server = new NetServer(config);
                _server.Start();

                _server.UPnP.ForwardPort(config.Port, "DoodleEmpires");

                Console.WriteLine("Server started at {0}:{1}", _server.Configuration.BroadcastAddress, _server.Port);
            }
            catch
            {
                Console.WriteLine("Failed to bind to port {0}\n", port);
                Console.WriteLine("Aborting");
                return;
            }

            _map = new ServerMap(GlobalTileManager.TileManager, 800, 400);
            
            // schedule initial sending of position updates
            double nextSendUpdates = NetTime.Now;

            // run until escape is pressed
            while (!Exiting)
            {
                NetIncomingMessage msg;
                while ((msg = _server.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.DiscoveryRequest:

                            NetOutgoingMessage om = _server.CreateMessage();
                            _serverInfo.WriteToPacket(om);
                            _server.SendDiscoveryResponse(om, msg.SenderEndpoint);

                            Console.WriteLine("Pinged discovery request");
                            break;
                            
                        case NetIncomingMessageType.StatusChanged: //a client's status has changed

                            NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                            if (status == NetConnectionStatus.Disconnected)
                            {
                                string reason = msg.ReadString();
                                _playerConnections.Remove(msg.SenderConnection);

                                NetPlayer p = Array.Find<NetPlayer>
                                    (_players, X => X != null ? X.NetConnection == msg.SenderConnection : false);
                                
                                if (p != null) 
                                    SendPlayerLeft(p.Info);

                                Console.WriteLine("Lost connection to {0} for {1}", msg.SenderEndpoint, reason);
                            }

                            break;
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            //
                            // Just print diagnostic messages to console
                            //
                            Console.WriteLine(msg.ReadString());
                            break;
                            
                        case NetIncomingMessageType.Data:
                            NetPacketType pType = (NetPacketType)msg.ReadByte();

                            switch (pType)
                            {
                                case NetPacketType.RequestJoin:
                                    HandlePlayerWantJoin(msg);
                                    break;
                                case NetPacketType.RequestBlockChanged:
                                    HandleBlockReqChange(msg);
                                    break;
                                default:

                                    break;
                            }
                            break;
                    }
                    _server.Recycle(msg);
                }

                // sleep to allow other processes to run smoothly
                Thread.Sleep(1);
            }

            _server.Shutdown("app exiting");
        }

        public void Save(Stream fileStream)
        {
            Console.WriteLine("Saving Map");
            _map.SaveToStream(fileStream);
        }

        public void Load(Stream fileStream)
        {
            Console.WriteLine("Loading Map");
            _map = ServerMap.ReadFromStream(fileStream, GlobalTileManager.TileManager);
        }

        private void HandlePlayerWantJoin(NetIncomingMessage msg)
        {
            PlayerInfo pInfo = PlayerInfo.ReadFromPacket(msg);

            sbyte ID = -1;

            for (sbyte i = 0; i < 4; i++)
                if (_players[i] == null)
                    ID = i;

            if (ID != -1)
            {
                byte rID = (byte)ID;

                _playerConnections.Add(msg.SenderConnection, rID);

                _players[rID] = new NetPlayer(pInfo, msg.SenderConnection);

                NetOutgoingMessage outM = _server.CreateMessage();

                outM.Write((byte)NetPacketType.AcceptedJoin);

                _serverInfo.WriteToPacket(outM);

                outM.Write(rID, 8);

                _map.WriteToMessage(outM);

                outM.Write((byte)_players.Count(X => X != null));

                foreach (NetPlayer p in _players)
                {
                    if (p != null)
                        p.Info.WriteToPacket(outM);
                }

                _server.SendMessage(outM, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered);

                SendPlayerJoined(pInfo);

                Console.WriteLine("Accepting request from a client");
            }
            else
            {
                NetOutgoingMessage outM = _server.CreateMessage();
                outM.Write((byte)NetPacketType.ConnectionFailed,8);
                outM.Write("Server is full");
                _server.SendMessage(outM, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                Console.WriteLine("Server full, informing client");
            }
        }

        private void HandleBlockReqChange(NetIncomingMessage msg)
        {
            int x = msg.ReadInt16();
            int y = msg.ReadInt16();
            byte newID = msg.ReadByte(8);

            if (x > 0 && y > 0 &&
                x < _map.WorldWidth / ServerMap.TILE_WIDTH && y < _map.WorldWidth / ServerMap.TILE_HEIGHT)
            {
                _map.SetTileSafe(x, y, newID);
                SendBlockChanged(x, y, newID);
            }
        }

        private void SendPlayerJoined(PlayerInfo newPlayer)
        {
            NetOutgoingMessage message = _server.CreateMessage();
            message.Write((byte)NetPacketType.PlayerJoined, 8);
            newPlayer.WriteToPacket(message);

            foreach (NetPlayer p in _players)
            {
                if (p != null && p.PlayerIndex != newPlayer.PlayerIndex)
                {
                    _server.SendMessage(message, p.NetConnection, NetDeliveryMethod.ReliableUnordered);
                }
            }
        }

        private void SendPlayerLeft(PlayerInfo player)
        {
            NetOutgoingMessage message = _server.CreateMessage();
            message.Write((byte)NetPacketType.PlayerLeft, 8);
            player.WriteToPacket(message);

            foreach (NetPlayer p in _players)
            {
                if (p != null && p.PlayerIndex != player.PlayerIndex)
                {
                    _server.SendMessage(message, p.NetConnection, NetDeliveryMethod.ReliableUnordered);
                }
            }
        }

        private void SendBlockChanged(int x, int y, byte newID)
        {
            NetOutgoingMessage message = _server.CreateMessage();
            message.Write((byte)NetPacketType.BlockUpdate, 8);
            message.Write((short)x);
            message.Write((short)y);
            message.Write(newID, 8);

            _server.SendMessage(message, _playerConnections.Keys.ToList(), NetDeliveryMethod.ReliableUnordered, 0);

        }
    }
}
