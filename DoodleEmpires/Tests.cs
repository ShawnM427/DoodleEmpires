using DoodleEmpires.Engine.Terrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoUI;
using DoodleEmpires.Engine.Net.PacketHandlers;
using DoodleEmpires.Engine.Net;
using Lidgren.Network;
using System.Net;
using System.Threading;
using System.Diagnostics;
using DoodleEmpires.Engine.Utilities;

namespace DoodleEmpires
{
    public class Tests : Game
    {
        GraphicsDeviceManager graphicsManager;

        PixelRegion _region;
        PixelBrush _sampleBrush;

        NetManager _manager;
        //SimpleTileMap _map;
        ICamera2D _camera;

        public Tests(string playerName)
            : base()
        {
            graphicsManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            NetPeerConfiguration config = new NetPeerConfiguration("DETests");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            config.EnableMessageType(NetIncomingMessageType.StatusChanged);
            config.EnableMessageType(NetIncomingMessageType.Data);
            config.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            config.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
            config.ConnectionTimeout = 10F;
            config.EnableUPnP = true;
            config.Port = 12345;
            
            NetPeerConfiguration config2 = new NetPeerConfiguration("DETests");
            config2.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            config2.EnableMessageType(NetIncomingMessageType.StatusChanged);
            config2.EnableMessageType(NetIncomingMessageType.Data);
            config2.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            config2.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
            config2.ConnectionTimeout = 10F;
            config2.EnableUPnP = true;
            config2.Port = 12346;

            NetClient peer = new NetClient(config);
            NetServer peer2 = new NetServer(config2);

            peer.Start();
            peer2.Start();

            _manager = new NetManager(NetPacketIdentifierSize.Byte, peer);
            _manager.ImportPacketType(typeof(Packet_PlayerJoined), PlayerJoined);
            _manager.Run();

            NetManager peer2Manager = new NetManager(NetPacketIdentifierSize.Byte, peer2);
            peer2Manager.ImportPacketType(typeof(Packet_PlayerJoined), PlayerJoined);
            peer2Manager.Run();

            NetConnection derp = peer.Connect(new IPEndPoint(NetUtility.Resolve("localhost"), 12346));

            Thread.Sleep(10);

            Packet_PlayerJoined packet = new Packet_PlayerJoined(new PlayerInfo("Dando Cuntrissian"));
            _manager.SendMessage(packet);
        }

        protected void PlayerJoined(object sender, IPacket packet)
        {
            Packet_PlayerJoined inPacket = (Packet_PlayerJoined)packet;
            PlayerInfo info = inPacket.Info;

            Debug.WriteLine(info.UserName);
        }

        protected override void Initialize()
        {
            _region = new PixelRegion(Point.Zero, GraphicsDevice);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _sampleBrush = new PixelBrush(Content.Load<Texture2D>("SampleBrush"));
            _region.Terrain = Content.Load<Texture2D>("SampleRegion");

            TileManager tileManager = new TileManager();
            tileManager.RegisterTile("grass", new Tile(0, 10));
            tileManager.RegisterTile("water", new Tile(1, 34));
            tileManager.RegisterTile("smallTree", new Tile(2, 19));

            //TextureAtlas atlas = new TextureAtlas(Content.Load<Texture2D>("TDTerrain"), 8, 8);

            //_map = new SimpleTileMap(GraphicsDevice, Content.Load<SpriteFont>("GUIFont"), tileManager, atlas, 64, 64, 4);
            //_map.GenTree(5, 5, 0);
            _camera = new Camera2D(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            MouseState state = Mouse.GetState();

            if (state.LeftButton == ButtonState.Pressed)
                _region.ApplyBrush(_sampleBrush, state.Position.ToVector2(), 0);

            if (state.RightButton == ButtonState.Pressed)
            {
                Packet_PlayerJoined packet = new Packet_PlayerJoined(new PlayerInfo("gah"));
                _manager.SendMessage(packet);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _region.Render();

            //_map.Render(_camera);

            base.Draw(gameTime);
        }
    }
}
