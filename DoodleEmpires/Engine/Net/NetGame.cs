﻿/// TODO note:
/// 
/// There's currently a pretty severe bug on (probably) serverside
/// that's causing issues with changed tile states not properly synching when
/// saving and loading maps. Client synchronization is still good. 
/// May be an issue with the saving/loading of the delta map changes. 
/// May also be the setting of ID's if the change already exists.
/// May also look into keeping a non-static noise instance in server, and check
/// the requested ID's against the noise ID's, add delta change where needed
///  foo
///  Sidenote: Need to look more into the delta states. Currently, they're generated even
///  during TG. MUST fix to be only on TileSet, but the tree gen currently uses TileSet,
///  maybe add a CoreTileSet that doesn't update the delta changes?

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoodleEmpires.Engine.Utilities;
using DoodleEmpires.Engine.Terrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DoodleEmpires.Engine.Entities;
using Microsoft.Xna.Framework.Input;
using Lidgren.Network;
using System.IO;
using DoodleEmpires.Engine.GUI;
using DoodleEmpires.Engine.Economy;
using System.Windows.Forms;
using DoodleEmpires.Engine.Sound;
using Microsoft.Xna.Framework.Audio;

using MouseEventArgs = DoodleEmpires.Engine.Utilities.MouseEventArgs;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using System.Threading;
using System.Net;

namespace DoodleEmpires.Engine.Net
{
    public class NetGame : AdvancedGame
    {
        #region Networking Vars

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
        /// The underlying network connection
        /// </summary>
        //NetConnection _myConnection;
        /// <summary>
        /// The current port
        /// </summary>
        int? _port = 14239;
        string _ip;
        /// <summary>
        /// A list of all local servers, only populated if an IP and port were not specified
        /// </summary>
        List<ServerInfo> _availableServers = new List<ServerInfo>();

        /// <summary>
        /// A list of all other players currently connected
        /// </summary>
        List<PlayerInfo> _players = new List<PlayerInfo>();

        /// <summary>
        /// Represents the client-controlled player
        /// </summary>
        PlayerInfo _myPlayer;

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

        public List<ServerInfo> AvailableServers
        {
            get { return _availableServers; }
        }

        int _prevReqX = -1;
        int _prevReqY = -1;
        int _prevReqDelX = -1;
        int _prevReqDelY = -1;

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
        public event TerrainSetEvent OnTerrainSet;
        /// <summary>
        /// Called when a server connection was unsucessful
        /// </summary>
        public event ServerConnectionFailed OnConnectionFailed;

        #endregion

        System.Timers.Timer _serverRefreshTimer = new System.Timers.Timer(GlobalNetVars.SERVER_POLLING_RATE * 1000);

        #endregion

        #region Game Vars
        
        GraphicsDeviceManager graphicsManager;
        SpriteFont _debugFont;

        protected SPMap _map;
        protected TileManager _tileManager;
        protected TextureAtlas _blockAtlas;

        protected SoundEngine _soundEngine;

        protected Camera2D _view;
        protected CameraControl _cameraController;
        protected Texture2D _paperTex;

        protected KeyboardState _prevKeyState;
        protected Vector2 _moveVector;
        protected Vector2 _mouseWorldPos;

        protected Random _rand;

        protected GUIContainer _mainControl;
        protected GUIContainer _menuControl;
        protected GUIContainer _serverListControl;
        protected GUILabel _fpsLabel;
        GUIButton _saveButton;
        GUIButton _loadButton;
        GUIListView _serverList;
        GUIGridView _zoneView;

        protected SpriteFont _guiFont;

        protected byte _editType = 1;
        protected short _zoneTpye = 1;

        protected Texture2D[] _blockTexs;

        protected bool _isDefininingZone = false;
        protected Vector2 _zoneStart = Vector2.Zero;

        protected GameState _gameState = GameState.MainMenu;

        #endregion

        bool _singlePlayer = true;
        protected bool SinglePlayer
        {
            get { return _singlePlayer; }
            set
            {
                _singlePlayer = value;

                if (_singlePlayer)
                {
                    if (_map == null || !_map.SinglePlayerMap)
                    {
                        _map = new SPMap(GraphicsDevice, _guiFont, _tileManager, _blockAtlas, 400, 800);
                        _map.BackDrop = _paperTex;

                        _view = new Camera2D(GraphicsDevice);
                        _view.ScreenBounds = new Rectangle(0, 0, _map.WorldWidth, _map.WorldHeight);

                        _cameraController = new CameraControl(_view);
                        _cameraController.Position = new Vector2(0, 200 * SPMap.TILE_HEIGHT);

                        _view.Focus = _cameraController;

                    }

                    _loadButton.Visible = true;
                    _loadButton.Enabled = true;

                    _saveButton.Visible = true;
                    _saveButton.Enabled = true;

                    _zoneView.Y = _loadButton.Bounds.Bottom + 5;
                }
                else
                {
                    _serverRefreshTimer.Start();

                    _port = _port.HasValue ? _port.Value : GlobalNetVars.DEFAULT_PORT;

                    NetPeerConfiguration config = new NetPeerConfiguration("DoodleEmpires");
                    config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
                    config.EnableMessageType(NetIncomingMessageType.StatusChanged);
                    config.EnableMessageType(NetIncomingMessageType.Data);
                    config.ConnectionTimeout = 10F;
                    //config.LocalAddress = IPAddress.Parse("");
                    //config.Port = _port.HasValue ? _port.Value : GlobalNetVars.DEFAULT_PORT;

                    _client = new NetClient(config);
                    _client.Start();

                    OnJoinedServer += _OnJoinedServer;

                    PollForServers();
                    
                    _zoneView.Y = _loadButton.Y;
                }
            }
        }

        /// <summary>
        /// Creates a new instance of a networked game
        /// </summary>
        public NetGame(string userName = "unknown")
            : base()
        {
            graphicsManager = new GraphicsDeviceManager(this);
            graphicsManager.PreferMultiSampling = false;
            graphicsManager.SynchronizeWithVerticalRetrace = false;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = false;

            _myPlayer = new PlayerInfo(userName);

            OnFoundServer += new FoundServerEvent(NetGame_OnFoundServer);
        }
        
        /// <summary>
        /// Initializes the game
        /// </summary>
        protected override void Initialize()
        {
            _tileManager = GlobalTileManager.TileManager;

            _blockAtlas = new TextureAtlas(Content.Load<Texture2D>("Atlas"), 20, 20);
            
            _guiFont = Content.Load<SpriteFont>("GUIFont");
            _guiFont.FixFont();

            _serverRefreshTimer.Elapsed += new System.Timers.ElapsedEventHandler(_serverRefreshTimer_Elapsed);
                        
            if (_singlePlayer)
            {
                _map = new SPMap(GraphicsDevice, _guiFont, _tileManager, _blockAtlas, 400, 800);

                _view = new Camera2D(GraphicsDevice);
                _view.ScreenBounds = new Rectangle(0, 0, _map.WorldWidth, _map.WorldHeight);

                _cameraController = new CameraControl(_view);
                _cameraController.Position = new Vector2(0, 200 * SPMap.TILE_HEIGHT);

                _view.Focus = _cameraController;
            }

            _rand = new Random();
            
            base.Initialize();
        }

        void _serverRefreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            PollForServers();
        }

        /// <summary>
        /// Checks for local servers. Later, this will poll the central server for servers
        /// </summary>
        private void PollForServers()
        {
            if (_ip == null)
            {
                for (int i = GlobalNetVars.MIN_PORT; i <= GlobalNetVars.MAX_PORT; i++)
                    _client.DiscoverLocalPeers(i);
            }
            else
                _client.Connect(_ip, _port.Value);

            _client.DiscoverKnownPeer("192.0.247.228", 14245);
        }

        /// <summary>
        /// Loads the content for this game
        /// </summary>
        protected override void LoadContent()
        {
            _paperTex = Content.Load<Texture2D>("Paper");

            _debugFont = Content.Load<SpriteFont>("debugFont");

            _soundEngine = new SoundEngine();

            foreach (string s in Directory.GetFiles("Content\\Sounds"))
            {
                string sName = Path.GetFileNameWithoutExtension(s);
                _soundEngine.AddSound(sName, Content.Load<SoundEffect>("Sounds\\" + sName));
            }

            _blockTexs = _blockAtlas.GetTextures(GraphicsDevice);

            _mainControl = new GUIPanel(GraphicsDevice, null);
            _mainControl.Bounds = new Rectangle(0, 0, 120, 285);

            _menuControl = new GUIPanel(GraphicsDevice, null);
            _menuControl.Bounds = new Rectangle(0, 0, 120, 165);
            _menuControl.X = GraphicsDevice.Viewport.Width / 2 - _menuControl.Bounds.Width / 2;
            _menuControl.Y = GraphicsDevice.Viewport.Height / 2 - _menuControl.Bounds.Height / 2;

            _serverListControl = new GUIPanel(GraphicsDevice, null);
            _serverListControl.Bounds = new Rectangle(0, 0, 120, 165);
            _serverListControl.X = GraphicsDevice.Viewport.Width / 2 - _serverListControl.Bounds.Width / 2;
            _serverListControl.Y = GraphicsDevice.Viewport.Height / 2 - _serverListControl.Bounds.Height / 2;

            _serverList = new GUIListView(GraphicsDevice, _serverListControl);
            _serverList.Bounds = new Rectangle(0, 0, 120, 140);
            _serverList.Font = _guiFont;
            _serverList.HeaderText = "Host: ";

            GUIButton serverListBack = new GUIButton(GraphicsDevice, _guiFont, _serverListControl);
            serverListBack.Text = "Back";
            serverListBack.Bounds = new Rectangle(40, _serverList.Bounds.Bottom + 5, 40, 15);
            serverListBack.OnMousePressed += ExitToMenu;
            
            GUIButton singlePlayerButton = new GUIButton(GraphicsDevice, _guiFont, _menuControl);
            singlePlayerButton.Bounds = new Rectangle(20, 20, 80, 20);
            singlePlayerButton.Text = "Singleplayer";
            singlePlayerButton.OnMousePressed += singlePlayerButton_OnPressed;

            GUIButton LANButton = new GUIButton(GraphicsDevice, _guiFont, _menuControl);
            LANButton.Bounds = new Rectangle(20, singlePlayerButton.Bounds.Bottom + 5, 80, 20);
            LANButton.Text = "LAN";
            LANButton.OnMousePressed += LANButton_OnPressed;

            singlePlayerButton.Invalidating = true;

            _fpsLabel = new GUILabel(GraphicsDevice, _guiFont, _mainControl);
            _fpsLabel.Text = "";

            _saveButton = new GUIButton(GraphicsDevice, _guiFont, _mainControl);
            _saveButton.Text = "Save";
            _saveButton.Bounds = new Rectangle(5, 140, 40, 20);
            _saveButton.OnMousePressed += new Action(saveButton_OnMousePressed);
            _saveButton.Visible = false;

            _loadButton = new GUIButton(GraphicsDevice, _guiFont, _mainControl);
            _loadButton.Text = "Load";
            _loadButton.Bounds = new Rectangle(50, 140, 40, 20);
            _loadButton.OnMousePressed += new Action(loadButton_OnMousePressed);
            _loadButton.Visible = false;

            GUIGridView gridView = new GUIGridView(GraphicsDevice, _mainControl);
            gridView.Bounds = new Rectangle(0, 12, 121, 121);
            gridView.Font = _guiFont;
            gridView.HeaderText = "Block:";

            _zoneView = new GUIGridView(GraphicsDevice, _mainControl);
            _zoneView.Bounds = new Rectangle(0, gridView.Bounds.Bottom + 5, 121, 120);
            _zoneView.Font = _guiFont;
            _zoneView.HeaderText = "Zone:";

            foreach (ZoneInfo zone in GlobalZoneManager.Manager.Items)
            {
                _zoneView.AddItem(new GridViewItem()
                {
                    Texture = _blockTexs[0],
                    MousePressed = OnZoneChanged,
                    Tag = zone.ZoneID,
                    Text = zone.Name,
                    ColorModifier = zone.Color
                });
            }

            foreach (Tile t in _tileManager.Tiles)
            {
                if (t.Type != 0)
                {
                    gridView.AddItem(new GridViewItem()
                    {
                        Texture = _blockTexs[t.TextureID],
                        MousePressed = OnItemChanged,
                        Tag = t.Type,
                        Text = _tileManager.NameOf(t.Type),
                        ColorModifier = t.Color
                    });
                }
            }
        }

        #region Standard Game

        /// <summary>
        /// Updates this game
        /// </summary>
        /// <param name="gameTime">The current time stamp</param>
        protected override void Update(GameTime gameTime)
        {
            #if PROFILING
            Window.Title = "" + FPSManager.AverageFramesPerSecond;

            if (Keyboard.GetState().IsKeyDown(Keys.PageUp))
                Thread.Sleep(1);

            if (Keyboard.GetState().IsKeyDown(Keys.PageDown))
                return;
            #endif

            switch (_gameState)
            {
                case GameState.MainMenu:
                    _menuControl.Update();
                    break;
                case GameState.ServerList:
                    _serverListControl.Update();
                    break;
                case GameState.InGame:
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                        ExitToMenu();

                    _cameraController.Update(gameTime);
                    _view.Update(gameTime);

                    KeyboardState keyState = Keyboard.GetState();

                    _mainControl.Update();

                    _soundEngine.ListenerPosition = _cameraController.Position;

                    if (keyState.IsKeyDown(Keys.Space) && _prevKeyState.IsKeyUp(Keys.Space))
                    {
                        _soundEngine.PlaySound("rifle", _view.PointToWorld(new Vector2(Mouse.GetState().X, Mouse.GetState().Y)));

                        if (_rand.NextDouble() < 0.6)
                        {
                            if (_rand.NextDouble() < 0.5)
                                _soundEngine.PlaySound("shell_01", _view.PointToWorld(new Vector2(Mouse.GetState().X, Mouse.GetState().Y)));
                            else

                                _soundEngine.PlaySound("shell_02", _view.PointToWorld(new Vector2(Mouse.GetState().X, Mouse.GetState().Y)));
                        }
                    }

                    if (keyState.IsKeyDown(Keys.F3) && _prevKeyState.IsKeyUp(Keys.F3))
                    {
                        _map.Debugging = !_map.Debugging;
                    }

                    _prevKeyState = keyState;
                    break;
            }

            if (!_singlePlayer)
                UpdateNetworking();

            base.Update(gameTime);
        }

        /// <summary>
        /// Updates this network handler, should be threaded
        /// </summary>
        public void UpdateNetworking()
        {
            // read messages
            NetIncomingMessage msg;
            while ((msg = _client.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryResponse:

                        ServerInfo info = ServerInfo.ReadFromPacket(msg);
                        info.EndPoint = msg.SenderEndpoint;

                        if (!_availableServers.Contains(info))
                        {
                            _availableServers.Add(info);

                            if (OnFoundServer != null)
                                OnFoundServer(info);
                        }

                        break;

                    case NetIncomingMessageType.DiscoveryRequest:
                        NetOutgoingMessage outMsg = _client.CreateMessage();
                        outMsg.Write("Hello");
                        _client.SendDiscoveryResponse(outMsg, msg.SenderEndpoint);
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                        if (status == NetConnectionStatus.Connected)
                        {
                            Console.WriteLine("Connected to server, joining game");
                            RequestJoin();
                        }
                        else if (status == NetConnectionStatus.Disconnected)
                        {
                            System.Diagnostics.Debug.WriteLine("Lost Connection \"{0}\"", msg.ReadString());
                            _gameState = GameState.MainMenu;
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

                            case NetPacketType.ConnectionFailed: //the connection attempt has failed
                                HandleConnectionFailed(msg);
                                break;

                            case NetPacketType.ZoneAdded:
                                HandleZoneAdded(msg);
                                break;

                            case NetPacketType.ZoneRemoved:
                                HandleZoneDel(msg);
                                break;

                            case NetPacketType.MapChanged:
                                HandleMapChanged(msg);
                                break;

                            default:
                                Console.WriteLine("Unknown packet type {0} received!", packetType);
                                _client.Disconnect("You sent shitty data!");
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
        /// Draws this game
        /// </summary>
        /// <param name="gameTime">The current time stamp</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(null);

            FPSManager.OnDraw(gameTime);

            switch (_gameState)
            {
                case GameState.ServerList:
                    DrawServerList();
                    break;
                case GameState.MainMenu:
                    DrawMenu();
                    break;
                case GameState.InGame:
                    DrawMainGame();
                    break;
            }
        }

        /// <summary>
        /// Draws the main game
        /// </summary>
        protected virtual void DrawMainGame()
        {
            GraphicsDevice.Clear(Color.White);
            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

            _map.Render(_view);

            _mainControl.Draw();
        }

        /// <summary>
        /// Draws the game's main menu
        /// </summary>
        protected virtual void DrawMenu()
        {
            GraphicsDevice.Clear(Color.White);

            _menuControl.Draw();
        }

        /// <summary>
        /// Draws the server list
        /// </summary>
        protected virtual void DrawServerList()
        {
            GraphicsDevice.Clear(Color.White);

            _serverListControl.Draw();
        }

        /// <summary>
        /// Exits this game to the main menu
        /// </summary>
        protected virtual void ExitToMenu()
        {
            _gameState = GameState.MainMenu;
            _serverRefreshTimer.Stop();

            if (_client != null)
            {
                _client.Disconnect("Disconnecting");
            }
        }

        /// <summary>
        /// Called when a mouse button is pressed
        /// </summary>
        /// <param name="args">The current mouse arguments</param>
        protected override void MousePressed(MouseEventArgs args)
        {
            switch (_gameState)
            {
                case GameState.InGame:
                    if (Keyboard.GetState().IsKeyDown(Keys.LeftAlt) & args.LeftButton == ButtonState.Pressed)
                    {
                        _isDefininingZone = true;
                        _zoneStart = _view.PointToWorld(args.Location);
                    }
                    else if (args.RightButton == ButtonState.Pressed)
                    {
                        Vector2 worldPos = _view.PointToWorld(args.Location);

                        if (_singlePlayer)
                        {
                            foreach (Zoning z in _map.Zones)
                            {
                                if (z.Bounds.Contains(worldPos))
                                {
                                    _map.DeleteZone(z);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            RequestDelZone((int)worldPos.X, (int)worldPos.Y);
                        }
                    }
                    break;
            }

            base.MousePressed(args);
        }

        /// <summary>
        /// Called when a mouse button is held down
        /// </summary>
        /// <param name="args">The current mouse arguments</param>
        protected override void MouseDown(MouseEventArgs args)
        {
            switch (_gameState)
            {
                case GameState.MainMenu:
                    _menuControl.MousePressed(args);
                    break;
                case GameState.ServerList:
                    _serverListControl.MousePressed(args);
                    break;
                case GameState.InGame:
                    if (!_mainControl.ScreenBounds.Contains(args.Location))
                    {
                        #region handle editing
                        if (args.LeftButton == ButtonState.Pressed && !_isDefininingZone)
                        {
                            Vector2 pos = _view.PointToWorld(args.Location);

                            int pX = (int)pos.X / SPMap.TILE_WIDTH;
                            int pY = (int)pos.Y / SPMap.TILE_HEIGHT;

                            if (pX != _prevReqX || pY != _prevReqY)
                            {
                                if (!_singlePlayer)
                                    RequestBlockChange(pX, pY, _editType);
                                else
                                    _map.SetTileSafe(pX, pY, _editType);
                            }

                            _prevReqX = pX;
                            _prevReqY = pY;
                        }
                        else if (args.RightButton == ButtonState.Pressed)
                        {
                            Vector2 pos = _view.PointToWorld(args.Location);

                            int pX = (int)pos.X / SPMap.TILE_WIDTH;
                            int pY = (int)pos.Y / SPMap.TILE_HEIGHT;

                            if (pX != _prevReqDelX || pY != _prevReqDelY)
                                if (!_singlePlayer)
                                    RequestBlockChange(pX, pY, 0);
                                else
                                    _map.SetTileSafe(pX, pY, 0);

                            _prevReqDelX = pX;
                            _prevReqDelY = pY;
                        }
                        #endregion
                    }
                    else
                    {
                        _mainControl.MousePressed(args);
                    }
                    break;
            }

            base.MouseDown(args);
        }

        /// <summary>
        /// Called when a mouse button is released
        /// </summary>
        /// <param name="args">The current mouse arguments</param>
        protected override void MouseReleased(MouseEventArgs args)
        {
            switch (_gameState)
            {
                case GameState.InGame:
                    _prevReqX = -1;
                    _prevReqY = -1;
                    _prevReqDelX = -1;
                    _prevReqDelY = -1;

                    if (_isDefininingZone & args.LeftButton == ButtonState.Pressed)
                    {
                        Vector2 zoneEnd = _view.PointToWorld(args.Location);

                        int x = (int)Math.Min(_zoneStart.X, zoneEnd.X);
                        int y = (int)Math.Min(_zoneStart.Y, zoneEnd.Y);
                        int width = (int)Math.Abs(zoneEnd.X - _zoneStart.X);
                        int height = (int)Math.Abs(zoneEnd.Y - _zoneStart.Y);

                        Rectangle bounds =
                            new Rectangle(x, y, width, height);
                        
                        if (_singlePlayer)
                        {
                            _map.DefineZone(new Zoning(bounds, GlobalZoneManager.Manager.Get(_zoneTpye)));
                        }
                        else
                        {
                            RequestNewZone(new Zoning(bounds, GlobalZoneManager.Manager.Get(_zoneTpye)));
                        }

                        _isDefininingZone = false;
                    }
                    break;
            }

            base.MouseReleased(args);
        }

        /// <summary>
        /// Called when the single player button in the main menu is pressed
        /// </summary>
        void singlePlayerButton_OnPressed()
        {
            SinglePlayer = true;
            _gameState = GameState.InGame;
        }

        /// <summary>
        /// Called when the LAN button in the main menu is pressed
        /// </summary>
        void LANButton_OnPressed()
        {
            SinglePlayer = false;
            _gameState = GameState.ServerList;
        }

        void NetGame_OnFoundServer(ServerInfo serverInfo)
        {
            GUI.ListViewItem item = new GUI.ListViewItem();
            item.Tag = serverInfo;
            item.Text = serverInfo.Name;
            item.MousePressed += new EventHandler<GUI.ListViewItem>(OnServerInfoMousePressed);
            item.ColorModifier = Color.Black;

            _serverList.AddItem(item);
        }

        /// <summary>
        /// Called when the a server is selected from the server list
        /// </summary>
        /// <param name="sender">The object that raised the event, should be the server list control</param>
        /// <param name="e">The selected list view item</param>
        private void OnServerInfoMousePressed(object sender, GUI.ListViewItem e)
        {
            ServerInfo sInfo = (ServerInfo)e.Tag;

            _gameState = GameState.Lobby;
            _serverRefreshTimer.Stop();
            ConnectToServer(sInfo);
            return;

        }

        /// <summary>
        /// Called when an item in the block list is selected
        /// </summary>
        /// <param name="sender">The object to raise the event</param>
        /// <param name="item">The newly selected item</param>
        private void OnItemChanged(object sender, GridViewItem item)
        {
            _editType = (byte)item.Tag;
        }

        /// <summary>
        /// Called when an item in the zone list is selected
        /// </summary>
        /// <param name="sender">The object to raise the event</param>
        /// <param name="item">The newly selected item</param>
        private void OnZoneChanged(object send, GridViewItem item)
        {
            _zoneTpye = (short)item.Tag;
        }

        #endregion

        #region SP Side

        void loadButton_OnMousePressed()
        {
            OpenFileDialog loadDialog = new OpenFileDialog();
            loadDialog.Filter = "Doodle Empires Map|*.dem";
            loadDialog.AddExtension = false;

            DialogResult dResult = loadDialog.ShowDialog();

            if (dResult == DialogResult.OK || dResult == DialogResult.Yes)
                LoadGame(loadDialog.FileName.Replace(".dem", ""));

            _mouseDown = false;
        }

        void saveButton_OnMousePressed()
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Doodle Empires Map|*.dem";
            saveDialog.AddExtension = false;

            DialogResult dResult = saveDialog.ShowDialog();

            if (dResult == DialogResult.OK || dResult == DialogResult.Yes)
                SaveGame(saveDialog.FileName);

            _mouseDown = false;
        }
        
        /// <summary>
        /// Saves this game to a given file
        /// </summary>
        /// <param name="fName">The relative file name to save to</param>
        private void SaveGame(string fName)
        {
            Stream fileStream = File.OpenWrite(fName.Replace(".dem", "") + ".dem");
            _map.SaveToStream(fileStream);
            fileStream.Close();
            fileStream.Dispose();
        }

        /// <summary>
        /// Loads this game from a given file
        /// </summary>
        /// <param name="fName">The relative file name to load from</param>
        private void LoadGame(string fName)
        {
            if (File.Exists(fName + ".dem"))
            {
                Stream fileStream = File.OpenRead(fName + ".dem");
                _map = SPMap.ReadFromStream(fileStream, GraphicsDevice, _guiFont, _tileManager, _blockAtlas);
                fileStream.Close();
                fileStream.Dispose();

                _map.BackDrop = _paperTex;
            }
        }

        #endregion

        /// <summary>
        /// Invoked when we have joined a server
        /// </summary>
        /// <param name="info">The info for the server we are connecting to</param>
        void _OnJoinedServer(ServerInfo info)
        {
            _gameState = GameState.InGame;
        }

        #region Networking

        #region Outgoing

        /// <summary>
        /// Connects the internal client to a specified game server
        /// </summary>
        /// <param name="server">The server to connect to</param>
        public void ConnectToServer(ServerInfo server)
        {
            _client.Connect(server.EndPoint);
        }

        /// <summary>
        /// Called when this client should request to join the server
        /// </summary>
        public void RequestJoin()
        {
            NetOutgoingMessage m = _client.CreateMessage();

            m.Write((byte)NetPacketType.RequestJoin);
            _myPlayer.WriteToPacket(m);

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

            m.Write((byte)NetPacketType.RequestBlockChanged, 8);

            m.Write((short)x);
            m.Write((short)y);
            m.Write(newID);

            _client.SendMessage(m, NetDeliveryMethod.ReliableUnordered);

#if DEBUG
            AccountedUpload += m.LengthBytes;
#endif
        }

        /// <summary>
        /// Called when this client is requesting a block update
        /// </summary>
        /// <param name="zone">The zone to add</param>
        public void RequestNewZone(Zoning zone)
        {
            NetOutgoingMessage msg = _client.CreateMessage();

            msg.Write((byte)NetPacketType.ReqZoneadded, 8);

            zone.WriteToPacket(msg);

            _client.SendMessage(msg, NetDeliveryMethod.ReliableUnordered);

#if DEBUG
            AccountedUpload += msg.LengthBytes;
#endif
        }

        /// <summary>
        /// Called when this client is requesting a zone to be deleted
        /// </summary>
        /// <param name="x">The x coord to delete at</param>
        /// <param name="y">The y coord to delete at</param>
        public void RequestDelZone(int x, int y)
        {
            NetOutgoingMessage msg = _client.CreateMessage();

            msg.Write((byte)NetPacketType.ReqZoneRemoved, 8);

            msg.Write((short)x);
            msg.Write((short)y);

            _client.SendMessage(msg, NetDeliveryMethod.ReliableUnordered);

#if DEBUG
            AccountedUpload += msg.LengthBytes;
#endif
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
        /// Called when the server has changed maps
        /// </summary>
        /// <param name="m">The message to handle</param>
        private void HandleMapChanged(NetIncomingMessage m)
        {
            _map = null;
            _map = SPMap.ReadFromMessage(m, GraphicsDevice, _guiFont, _tileManager, _blockAtlas);
            _map.BackDrop = _paperTex;
        }

        /// <summary>
        /// Called when the client receives a connection failed message
        /// </summary>
        /// <param name="m">The message to handle</param>
        private void HandleConnectionFailed(NetIncomingMessage m)
        {
            string reason = m.ReadString();
            _client.Disconnect(string.Format("Could not connect due to \"{0}\"", reason));

            if (OnConnectionFailed != null)
                OnConnectionFailed.Invoke(reason);

            Console.WriteLine(string.Format("Connection failed due to \"{0}\"", reason));
        }

        /// <summary>
        /// Called when the server accepts a clients join attempt
        /// </summary>
        /// <param name="m"></param>
        private void HandleJoin(NetIncomingMessage m)
        {
            ServerInfo serverInfo = ServerInfo.ReadFromPacket(m);

            _myPlayer.PlayerIndex = m.ReadByte();

            _map = SPMap.ReadFromMessage(m, GraphicsDevice, _guiFont, _tileManager, _blockAtlas);
            _map.BackDrop = _paperTex;

            byte playerCount = m.ReadByte();

            for (int i = 0; i < playerCount; i++)
            {
                PlayerInfo pInfo = PlayerInfo.ReadFromPacket(m);

                _players.Add(pInfo);
            }

#if DEBUG
            AccountedUpload += m.LengthBytes;
#endif

            
            _view = new Camera2D(GraphicsDevice);
            _view.ScreenBounds = new Rectangle(0, 0, _map.WorldWidth, _map.WorldHeight);

            _cameraController = new CameraControl(_view);
            _cameraController.Position = new Vector2(0, 200 * SPMap.TILE_HEIGHT);

            _view.Focus = _cameraController;

            if (OnJoinedServer != null)
                OnJoinedServer.Invoke(serverInfo);

            Console.WriteLine("Joined a game with {0} players as '{1}'", playerCount, _myPlayer.UserName);
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

            _map[x, y] = newID;

            if (OnTerrainSet != null)
                OnTerrainSet.Invoke(x, y, newID);

#if DEBUG
            AccountedDownload += m.LengthBytes;
#endif
        }

        /// <summary>
        /// Called when the server says a zone has been added
        /// </summary>
        /// <param name="m">The message to parse</param>
        private void HandleZoneAdded(NetIncomingMessage m)
        {
            Zoning zone = Zoning.ReadFromPacket(m);

            _map.AddPrebuiltZone(zone);

            #if DEBUG
            AccountedDownload += m.LengthBytes;
            #endif
        }

        /// <summary>
        /// Called when the server says a zone has been removed
        /// </summary>
        /// <param name="m">The message to parse</param>
        private void HandleZoneDel(NetIncomingMessage m)
        {
            int x = m.ReadInt16();
            int y = m.ReadInt16();

            _map.DeleteZone(x, y);

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

            _players.Add(pInfo);

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

            PlayerInfo player = _players.Find(X => X.PlayerIndex == pInfo.PlayerIndex);

            if (player != null)
            {
                _players.Remove(player);
                Console.WriteLine("{0} has left the game.", player.UserName);

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

        #endregion
    }
}
