#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using SmileyFaceWars.Engine;
using DoodleEmpires.Engine.Utilities;
using DoodleEmpires.Engine.Terrain;
using DoodleEmpires.Engine.Entities;
using DoodleEmpires.Engine.Net;
using Lidgren.Network;
using System.Net;
using Microsoft.Xna.Framework.Media;
using MonoUI;
#endregion

namespace SmileyFaceWars
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : AdvancedGame
    {
        GraphicsDeviceManager graphics;
        SpriteBatch _spriteBatch;

        TileTerrain _terrain;
        Camera2D _camera;

        GameState _gameState = GameState.Splash;

        NetManager _netManager;
        NetPeer _peer;
        List<PlayerInfo> _players;
        PlayerInfo _myPlayer;
        ServerInfo _myServer;
        List<ServerInfo> _knownServers;

        PlayerInstance _myPlayerInstance;
        List<PlayerInstance> _playerInstances;

        Texture2D _textureSplash;

        GUIContainer _mainMenuUI;
        GUIContainer _serverListUI;
        GUIListView _serverList;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("SmileyFaceWars");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            config.EnableMessageType(NetIncomingMessageType.StatusChanged);
            config.EnableMessageType(NetIncomingMessageType.Data);
            config.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            config.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
            config.ConnectionTimeout = 10F;

            _peer = new NetPeer(config);
            _peer.Start();

            _netManager = new NetManager(NetPacketIdentifierSize.Byte, _peer);
            _netManager.ImportPacketType(typeof(Packet_PlayerInput), PlayerRequestMove);
            _netManager.ImportPacketType(typeof(Packet_SendInfo), ServerInfoReceived);
            _netManager.OnConnectionApproval = PlayerConnected;
            _netManager.OnDiscoveryRequest = RequestInfo;
            _netManager.Run();

            for (int i = -1; i < 10; i ++ )
                _peer.DiscoverLocalPeers(_peer.Port + i);

            Window.Title = _peer.Port + "";

            _players = new List<PlayerInfo>();
            _knownServers = new List<ServerInfo>();
            _myPlayer = new PlayerInfo("Player");
            _myServer = new ServerInfo("Host", "Hello");

            _myPlayerInstance = new PlayerInstance(Vector2.Zero);
            _playerInstances = new List<PlayerInstance>();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            TextureAtlas atlas = new TextureAtlas(Content.Load<Texture2D>("TDTerrain"), 8, 8);

            TileManager manager = new TileManager();
            manager.RegisterTile("grass", new TerrainTile(1, 0));
            manager.RegisterTile("water", new TerrainTile(2, 24));

            _terrain = new TileTerrain(GraphicsDevice, 64, 64, 4, manager, atlas, 1, 15);
            _terrain.BackDrop = Content.Load<Texture2D>("SampleRegion");
            _textureSplash = Content.Load<Texture2D>("Splash2");
            _camera = new Camera2D(GraphicsDevice);
                        
            _terrain.Set(2, 2, 1, 2);
            _terrain.Set(0, 0, 1, 2);
            _terrain.Set(1, 1, 1, 2);
            _terrain.Set(1, 2, 1, 2);
            _terrain.Set(2, 1, 1, 2);
            _terrain.Set(16, 16, 1, 2);

            BuildMenus();

            _camera.ScreenBounds = new Rectangle(0, 0, _terrain.WorldWidth, _terrain.WorldHeight);
            _camera.Focus = _myPlayerInstance;
        }

        protected void BuildMenus()
        {
            _mainMenuUI = new GUIPanel(GraphicsDevice, null);
            _mainMenuUI.Bounds = new Rectangle(0, 0, 120, 175);
            _mainMenuUI.X = GraphicsDevice.Viewport.Width / 2 - _mainMenuUI.Bounds.Width / 2;
            _mainMenuUI.Y = GraphicsDevice.Viewport.Height / 2 - _mainMenuUI.Bounds.Height / 2;
            _mainMenuUI.BackColor = Color.White;

            GUIButton goToServer = new GUIButton(GraphicsDevice, null, _mainMenuUI);
            goToServer.Bounds = new Rectangle(15, 15, 70, 20);
            goToServer.Text = "LAN";
            goToServer.OnMousePressed += LANButtonPressed;

            GUIButton quitButton = new GUIButton(GraphicsDevice, null, _mainMenuUI);
            quitButton.Bounds = new Rectangle(15, goToServer.Bounds.Bottom + 5, 70, 20);
            quitButton.Text = "Quit";
            quitButton.OnMousePressed += Exit;

            _serverListUI = new GUIPanel(GraphicsDevice, null);
            _serverListUI.Bounds = new Rectangle(GraphicsDevice.Viewport.Width / 2 - 50, GraphicsDevice.Viewport.Height / 2 - 150, 100, 300);

            _serverList = new GUIListView(GraphicsDevice, _serverListUI);
            _serverList.Bounds = new Rectangle(0, 0, _serverListUI.Width, _serverListUI.Height - 30);

            GUIButton serverBackButton = new GUIButton(GraphicsDevice, null, _serverListUI);
            serverBackButton.Bounds = new Rectangle(5, _serverList.Bounds.Bottom + 5, 40, 20);
            serverBackButton.Text = "< Back";
            serverBackButton.OnMousePressed += BackButtonPressed;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
                 
            switch(_gameState)
            {
                case GameState.Splash:
                    KeyboardState kState = Keyboard.GetState();

                    if (kState.GetPressedKeys().Length > 0)
                        _gameState = GameState.Menu;
                    break;
                case GameState.Menu:
                    _mainMenuUI.Update();
                    break;
                case GameState.ServerList:
                    _serverListUI.Update();
                    break;
            }
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            switch(_gameState)
            {
                case GameState.Splash:
                    _spriteBatch.Begin();
                    _spriteBatch.Draw(_textureSplash, GraphicsDevice.Viewport.Bounds, Color.White);
                    _spriteBatch.End();
                    break;
                case GameState.Menu:
                    _mainMenuUI.Draw();
                    break;
                case GameState.ServerList:
                    _serverListUI.Draw();
                    break;
                case GameState.InGame:
                    _terrain.Render(_camera);
                    break;
            }


            base.Draw(gameTime);
        }

        #region Input

        /// <summary>
        /// Called when a mouse button is pressed
        /// </summary>
        /// <param name="args">The current mouse arguments</param>
        protected void MousePressed(MouseEventArgs args)
        {
            switch (_gameState)
            {
                case GameState.Menu:
                    _mainMenuUI.MousePressed(args);
                    break;
                case GameState.ServerList:
                    _serverListUI.MousePressed(args);
                    break;
            }
        }

        /// <summary>
        /// Called when a mouse button is held down
        /// </summary>
        /// <param name="args">The current mouse arguments</param>
        protected override void MouseDown(MouseEventArgs args)
        {
            switch (_gameState)
            {
                case GameState.Menu:
                    _mainMenuUI.MouseDown(args);
                    break;
                case GameState.ServerList:
                    _serverListUI.MouseDown(args);
                    break;
            }
        }

        /// <summary>
        /// Called when a mouse button is released
        /// </summary>
        /// <param name="args">The current mouse arguments</param>
        protected void MouseReleased(MouseEventArgs args)
        {
            switch (_gameState)
            {
                case GameState.InGame:
                    break;
            }
        }

        /// <summary>
        /// Invoked when a mouse button state has changed
        /// </summary>
        /// <param name="state">A snapshot of mouse values</param>
        protected override void MouseEvent(MouseEventArgs state)
        {
            if (state.LeftButton == ButtonChangeState.Pressed ||
                state.RightButton == ButtonChangeState.Pressed ||
                state.RightButton == ButtonChangeState.Pressed)
                MousePressed(state);

            if (state.LeftButton == ButtonChangeState.Released ||
                state.RightButton == ButtonChangeState.Released ||
                state.RightButton == ButtonChangeState.Released)
                MouseReleased(state);

            base.MouseEvent(state);
        }

        #endregion

        protected void BackButtonPressed()
        {
            _gameState = GameState.Menu;
        }

        protected void LANButtonPressed()
        {
            _gameState = GameState.ServerList;
        }
        
        protected void RequestInfo(NetIncomingMessage message)
        {
            Packet_SendInfo outgoing = new Packet_SendInfo();
            outgoing.ServerInfo = _myServer;

            _netManager.SendMessage(outgoing, message.SenderEndPoint);
        }

        protected void PlayerConnected(NetIncomingMessage msg)
        {
            _players.Add(new PlayerInfo(msg.ReadString(), msg.SenderConnection));
        }

        protected void ServerInfoReceived(object sender, IPacket input)
        {
            Packet_SendInfo packet = (Packet_SendInfo)input;

            _serverList.AddItem(new ServerInfoListItem(packet.ServerInfo));
        }

        protected void PlayerRequestMove(object sender, IPacket input)
        {
            Packet_PlayerInput packet = (Packet_PlayerInput)input;
        }
    }

    enum GameState
    {
        Splash,
        Menu,
        ServerList,
        Lobby,
        InGame,
        Settings
    }
}
