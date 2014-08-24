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
using DoodleEmpires.Engine.Economy;

namespace DoodleEmpires.Engine.Net
{
    /// <summary>
    /// The main game server
    /// </summary>
    public class GameServer
    {
        NetServer _server;
        ServerInfo _serverInfo;

        NetPlayer[] _players = new NetPlayer[4];
        Dictionary<NetConnection, byte> _playerConnections = new Dictionary<NetConnection, byte>();

        /// <summary>
        /// Gets a list of all non-null player currently connected
        /// </summary>
        public NetPlayer[] Players
        {
            get
            {
                return Array.FindAll(_players, X => X != null);
            }
        }
        /// <summary>
        /// Gets or sets this server's message
        /// </summary>
        public string Message
        {
            get { return _serverInfo.Message; }
            set { _serverInfo.Message = value; }
        }

        ServerMap _map;

        /// <summary>
        /// Gets or sets whether this server should exit at the end of it's current loop
        /// </summary>
        public bool Exiting
        {
            get;
            set;
        }

        /// <summary>
        /// Runs this server with the given command-line arguments
        /// </summary>
        /// <param name="args"></param>
        public void Run(string[] args)
        {
            int port = GlobalNetVars.DEFAULT_PORT;

            _serverInfo = new ServerInfo(args.Length > 0 ? args[0] : GlobalNetVars.DEFAULT_SERVER_NAME);
            if (args.Length > 1)
                port = int.Parse(args[1]);
            _serverInfo.Message = args.Length > 2 ? args[2] : GlobalNetVars.DEFAULT_SERVER_MESSAGE;
            if (args.Length > 3)
            {
                int playercount = 4;
                int.TryParse(args[3], out playercount);

                _players = new NetPlayer[playercount];
                _serverInfo.MaxPlayerCount = playercount;
            }
            
            Console.WriteLine("Opening server");
            Console.WriteLine("Server name set to \"{0}\"", _serverInfo.Name);
            NetPeerConfiguration config = new NetPeerConfiguration("DoodleEmpires");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            config.Port = port;
            config.EnableUPnP = true;
            //config.LocalAddress = IPAddress.Parse("25.11.245.37");
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
            _map.OnTerrainSet += SendBlockChanged;
            _map.OnZoneAdded += SendZoneAdded;
            
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

                            break;
                            
                        case NetIncomingMessageType.StatusChanged: //a client's status has changed

                            NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                            if (status == NetConnectionStatus.Disconnected)
                            {
                                string reason = msg.ReadString();

                                byte pID;
                                
                                if (_playerConnections.TryGetValue(msg.SenderConnection, out pID)) 
                                {
                                    Console.WriteLine("Player with ID {0} left, internal ID of {1}", pID, _players[pID].PlayerIndex);
                                    SendPlayerLeft(_players[pID].Info);
                                    _players[pID] = null;
                                    _playerConnections.Remove(msg.SenderConnection);

                                    _serverInfo.PlayerCount = Players.Length;
                                }

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

                                case NetPacketType.ReqZoneadded:
                                    HandleReqZone(msg);
                                    break;

                                case NetPacketType.ReqZoneRemoved:
                                    HandleReqDelZone(msg);
                                    break;
                            }
                            break;

                        case NetIncomingMessageType.UnconnectedData:
                            
                            NetPacketType pType2 = (NetPacketType)msg.ReadByte();

                            switch (pType2)
                            {
                                case NetPacketType.PingMessage:
                                    NetOutgoingMessage pingMSG = _server.CreateMessage();
                                    pingMSG.Write((byte)NetPacketType.PingMessage, 8);
                                    _server.SendUnconnectedMessage(pingMSG, msg.SenderEndpoint);
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

        /// <summary>
        /// Saves this server to a stream
        /// </summary>
        /// <param name="fileStream">The file stream to save to</param>
        public void Save(Stream fileStream)
        {
            Console.WriteLine("Saving Map");
            _map.SaveToStream(fileStream);
        }

        /// <summary>
        /// Loads a server map from a stream
        /// </summary>
        /// <param name="fileStream">The stream to load from</param>
        public void Load(Stream fileStream)
        {
            Console.WriteLine("Loading Map");
            _map = null;
            _map = ServerMap.ReadFromStream(fileStream, GlobalTileManager.TileManager);
            _map.OnTerrainSet += SendBlockChanged;
            _map.OnZoneAdded += SendZoneAdded;

            SendMapChanged();
        }

        private void HandlePlayerWantJoin(NetIncomingMessage msg)
        {
            PlayerInfo pInfo = PlayerInfo.ReadFromPacket(msg);

            sbyte ID = -1;

            for (sbyte i = 0; i < _players.Length; i++)
                if (_players[i] == null)
                {
                    ID = i;
                    break;
                }

            if (ID != -1)
            {
                byte rID = (byte)ID;
                pInfo.PlayerIndex = rID;

                _playerConnections.Add(msg.SenderConnection, rID);

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

                _players[rID] = new NetPlayer(pInfo, msg.SenderConnection);

                _serverInfo.PlayerCount = Players.Length;
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

            if (x >= 0 && y >= 0 &&
                x < _map.WorldWidth / ServerMap.TILE_WIDTH && y < _map.WorldHeight / ServerMap.TILE_HEIGHT)
            {
                _map.SetTileSafe(x, y, newID);
            }
        }
        
        private void HandleReqZone(NetIncomingMessage msg)
        {
            Zoning zone = Zoning.ReadFromPacket(msg);

            _map.DefineZone(zone);
        }

        private void HandleReqDelZone(NetIncomingMessage msg)
        {
            short x = msg.ReadInt16();
            short y = msg.ReadInt16();

            byte playerID = msg.ReadByte();

            foreach (Zoning zone in _map.Zones)
            {
                if (zone.Bounds.Contains(x, y) && playerID == zone.PlayerID)
                {
                    _map.DeleteZone(zone);
                    SendZoneRemoved(x, y);
                    return;
                }
            }
        }

        private void SendMapChanged()
        {
            NetOutgoingMessage msg = _server.CreateMessage();

            msg.Write((byte)NetPacketType.MapChanged, 8);

            _map.WriteToMessage(msg);

            if (_playerConnections.Keys.Count > 0)
                _server.SendMessage(msg, _playerConnections.Keys.ToList(), NetDeliveryMethod.ReliableUnordered, 0);
        }

        private void SendZoneAdded(Zoning newZone)
        {
            NetOutgoingMessage msg = _server.CreateMessage();

            msg.Write((byte)NetPacketType.ZoneAdded, 8);

            newZone.WriteToPacket(msg);

            _server.SendMessage(msg, _playerConnections.Keys.ToList(), NetDeliveryMethod.ReliableUnordered, 0);
        }

        private void SendZoneRemoved(short x, short y)
        {
            NetOutgoingMessage msg = _server.CreateMessage();

            msg.Write((byte)NetPacketType.ZoneRemoved, 8);

            msg.Write(x);
            msg.Write(y);

            _server.SendMessage(msg, _playerConnections.Keys.ToList(), NetDeliveryMethod.ReliableUnordered, 0);
        }
        
        private void SendPlayerJoined(PlayerInfo newPlayer)
        {
            Console.WriteLine("Accepting join request from \"{0}\"", newPlayer.UserName);
            
            NetOutgoingMessage message = _server.CreateMessage();
            message.Write((byte)NetPacketType.PlayerJoined, 8);
            newPlayer.WriteToPacket(message);

            _server.SendMessage(message, _playerConnections.Keys.ToList(), NetDeliveryMethod.ReliableUnordered, 0);            
        }

        private void SendPlayerLeft(PlayerInfo player)
        {
            NetOutgoingMessage message = _server.CreateMessage();
            message.Write((byte)NetPacketType.PlayerLeft, 8);
            player.WriteToPacket(message);

            _server.SendMessage(message, _playerConnections.Keys.ToList(), NetDeliveryMethod.ReliableUnordered, 0);
            
            Console.WriteLine("Player {0} has left the game", player.UserName);
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
