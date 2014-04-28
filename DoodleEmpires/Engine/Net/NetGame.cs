using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Net;
using DoodleEmpires.Engine.Terrain;
using DoodleEmpires.Engine.Utilities;

namespace DoodleEmpires.Engine.Net
{
    /// <summary>
    /// Represents the client-side network game
    /// </summary>
    public class NetGame
    {
         #if DEBUG

        /// <summary>
        /// The amount uploaded by our packets
        /// </summary>
        long AccountedUpload = 0;
        /// <summary>
        /// The amount downloaded in our packets
        /// </summary>
        long AccountedDownload = 0;

        #endif

        /// <summary>
        /// The core Net client
        /// </summary>
        NetClient _client;
        /// <summary>
        /// The current port
        /// </summary>
        int _port;
        /// <summary>
        /// A list of all local servers, only populated if an IP and port were not specified
        /// </summary>
        List<ServerInfo> _availableServers = new List<ServerInfo>();

        /// <summary>
        /// A list of all other players currently connected
        /// </summary>
        List<NetPlayer> _players = new List<NetPlayer>();

        /// <summary>
        /// Represents the client-controlled player
        /// </summary>
        NetPlayer _myPlayer;

        SPMap _map;

        protected GraphicsDevice _graphics;
        protected TileManager _tileManager;
        protected TextureAtlas _textureAtlas;

        /// <summary>
        /// Gets the underlying world width for this net game
        /// </summary>
        public int WorldWidth
        {
            get { return _map.WorldWidth; }
        }
        /// <summary>
        /// Gets the underlying world height for this net game
        /// </summary>
        public int WorldHeight
        {
            get { return _map.WorldHeight; }
        }

        #region Events

        /// <summary>
        /// Called when a server has been discovered
        /// </summary>
        public event FoundServerEvent OnFoundServer;
        /// <summary>
        /// Called when this client connects to a server
        /// </summary>
        public event JoinedServerEvent OnJoinedServer;
        /// <summary>
        /// Called when a player has joined
        /// </summary>
        public event PlayerJoinedEvent OnPlayerJoined;
        /// <summary>
        /// Called when a player has left
        /// </summary>
        public event PlayerLeftEvent OnPlayerLeft;
        /// <summary>
        /// Called when the server sent a tile set message
        /// </summary>
        public event TerrainIDSetEvent OnTerrainSet;

        #endregion

        /// <summary>
        /// Begins a new instance of a net game handler
        /// </summary>
        /// <param name="playerName">The playername to sign in as</param>
        /// <param name="IP">The IP to connect to</param>
        /// <param name="port">The port to connect to</param>
        public NetGame(GraphicsDevice graphics, TileManager tileManager, TextureAtlas atlas, 
            string playerName = "<noName>", string IP = null, int? port = null)
        {
            _graphics = graphics;
            _tileManager = tileManager;
            _textureAtlas = atlas;

            _myPlayer = new NetPlayer(new PlayerInfo(playerName, null));

            NetPeerConfiguration config = new NetPeerConfiguration("nettest");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            config.ConnectionTimeout = 10F;

            _client = new NetClient(config);
            _client.Start();
            
            if (port == null)
                _port = GlobalNetVars.DEFAULT_PORT;
            else
                _port = (int)port;

            if (IP == null)
            {
                if (_port == GlobalNetVars.DEFAULT_PORT)
                {
                    for (int i = 0; i <= 10; i++)
                        _client.DiscoverLocalPeers(GlobalNetVars.DEFAULT_PORT + i);
                }
                else
                    _client.DiscoverLocalPeers(_port);
            }
            else
                _client.Connect(IP, _port);
        }

        /// <summary>
        /// Updates this network handler, should be threaded
        /// </summary>
        public void Update()
        {
            // read messages
            NetIncomingMessage msg;
            while ((msg = _client.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryResponse:

                        ServerInfo info = ServerInfo.ReadFromPacket(msg);
                        _availableServers.Add(info);

                        if (OnFoundServer != null)
                            OnFoundServer(info);

                        break;

                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                        if (status == NetConnectionStatus.Connected)
                        {
                            Console.WriteLine("Connected to server, joining game");
                            RequestJoin();
                        }
                        break;

                    case NetIncomingMessageType.Data: //data was received
                        NetPacketType packetType = (NetPacketType)msg.ReadByte(8); //get the packet ID

                        switch (packetType) //toggle based on packet state
                        {
                            case NetPacketType.AcceptedJoin: //server has accepted join
                                HandleJoin(msg);
                                break;

                            case NetPacketType.PlayerJoined: //another player has joined
                                PlayerJoined(msg);
                                break;

                            case NetPacketType.PlayerLeft: //another player has left the game
                                PlayerLeft(msg);
                                break;

                            case NetPacketType.BlockUpdate: //another player's info has updated
                                HandleBlockChanged(msg);
                                break;
                        }
                        break;

                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        Console.WriteLine(msg.ReadString());
                        break;

                    default:
                        Console.WriteLine("received unhandled packet: " + msg.MessageType);
                        break;
                }
                _client.Recycle(msg);
            }
        }

        /// <summary>
        /// Draws this net game
        /// </summary>
        /// <param name="camera">The camera to draw with</param>
        public void Draw(ICamera2D camera)
        {
            _map.Render(camera);
        }
        
        #region Outgoing

        /// <summary>
        /// Called when this client should request to join the server
        /// </summary>
        public void RequestJoin()
        {
            NetOutgoingMessage m = _client.CreateMessage();

            m.Write((byte)NetPacketType.RequestJoin);
            _myPlayer.Info.WriteToPacket(m);

            _client.SendMessage(m, NetDeliveryMethod.ReliableUnordered);
            
            #if DEBUG
            AccountedUpload += m.LengthBytes;
            #endif

            Console.WriteLine("Sent join request");
        }

        /// <summary>
        /// Called when this client is requesting a block update
        /// </summary>
        /// <param name="x">The x coordinates to set (chunk)</param>
        /// <param name="y">The y coordinates to set (chunk)</param>
        /// <param name="newID">The ID to set the block to</param>
        public void RequestBlockChange(int x, int y, byte newID)
        {
            NetOutgoingMessage m = _client.CreateMessage();

            m.Write((short)x);
            m.Write((short)y);
            m.Write(newID);

            _client.SendMessage(m, NetDeliveryMethod.ReliableUnordered);

            #if DEBUG
            AccountedUpload += m.LengthBytes;
            #endif
            Console.WriteLine("Sent join request");
        }

        /// <summary>
        /// Called when we should leave the game
        /// </summary>
        /// <param name="reason"></param>
        public void ExitGame(string reason)
        {
            _client.Shutdown(reason);
        }

        #endregion
        
        #region Incoming

        /// <summary>
        /// Called when the server accepts a clients join attempt
        /// </summary>
        /// <param name="m"></param>
        private void HandleJoin(NetIncomingMessage m)
        {
            ServerInfo serverInfo = ServerInfo.ReadFromPacket(m);

            _myPlayer.Info.PlayerIndex = m.ReadByte();

            SPMap map = SPMap.ReadFromMessage(m, _graphics, _tileManager, _textureAtlas);
                        
            byte playerCount = m.ReadByte();

            for (int i = 0; i < playerCount; i++)
            {
                PlayerInfo pInfo = PlayerInfo.ReadFromPacket(m);

                _players.Add(new NetPlayer(pInfo));
            }

            #if DEBUG
            AccountedUpload += m.LengthBytes;
            #endif

            if (OnJoinedServer != null)
                OnJoinedServer.Invoke(serverInfo);

            Console.WriteLine("Joined a game with {0} players as '{1}'", playerCount, _myPlayer.Info.UserName);
        }

        /// <summary>
        /// Called when the server accepts a clients join attempt
        /// </summary>
        /// <param name="m"></param>
        private void HandleBlockChanged(NetIncomingMessage m)
        {
            int x = m.ReadInt16();
            int y = m.ReadInt16();
            byte newID = m.ReadByte(8);

            if (OnTerrainSet != null)
                OnTerrainSet.Invoke(x, y, newID);

            #if DEBUG
            AccountedDownload += m.LengthBytes;
            #endif
        }

        /// <summary>
        /// Called when another player joins the game
        /// </summary>
        /// <param name="m"></param>
        private void PlayerJoined(NetIncomingMessage m)
        {
            PlayerInfo pInfo = PlayerInfo.ReadFromPacket(m);

            _players.Add(new NetPlayer(pInfo));

            #if DEBUG
            AccountedDownload += m.LengthBytes;
            #endif

            if (OnPlayerJoined != null)
                OnPlayerJoined.Invoke(pInfo);

            Console.WriteLine("{0} has joined the game.", pInfo.UserName);
        }

        /// <summary>
        /// Called when a player leaves the game
        /// </summary>
        /// <param name="m"></param>
        private void PlayerLeft(NetIncomingMessage m)
        {
            PlayerInfo pInfo = PlayerInfo.ReadFromPacket(m);

            NetPlayer player = _players.Find(X => X.PlayerIndex == pInfo.PlayerIndex);

            if (player != null)
            {
                _players.Remove(player);
                Console.WriteLine("{0} has left the game.", player.Info.UserName);

                if (OnPlayerLeft != null)
                    OnPlayerLeft.Invoke(pInfo);
            }
            else
            {
                Console.WriteLine("[WARNING] Unkown player \"{0}\" has disconnected.", pInfo.UserName);
            }

            #if DEBUG
            AccountedDownload += m.LengthBytes;
            #endif
        }

        #endregion
    }
}
