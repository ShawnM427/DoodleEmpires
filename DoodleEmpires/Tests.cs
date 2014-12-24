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
using DoodleEmpires.Engine.Net;
using Lidgren.Network;
using System.Net;
using System.Threading;
using System.Diagnostics;
using DoodleEmpires.Engine;
using DoodleEmpires.Engine.Terrain.Brushes;
using DoodleEmpires.Engine.Terrain.TerrainBuilders;
using DoodleEmpires.Content;
using System.Reflection;

namespace DoodleEmpires
{
    public class Tests : Game
    {
        GraphicsDeviceManager graphicsManager;
        SpriteBatch _spriteBatch;

        SpriteFont _font;

        PixelTerrain _region;
        IPixelBrush _sampleBrush;
        DynamicRectBrush _rectBrush;

        FPSCounter _fpsCounter;
        //SimpleTileMap _map;
        ICamera2D _camera;

        bool _isDragging = false;
        MouseState _prevMouseState;
        Point _beginRect;
        PointLight _light;
        Texture2D _pixelTex;
        private SpotLight _spotLight;
        private GUIPanel _mainContainer;
        private GUILabel _framerateLabel;
        private GUIButton _pauseButton;

        public Tests(string playerName)
            : base()
        {
            graphicsManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsFixedTimeStep = false;
            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 120.0);
            _fpsCounter = new FPSCounter(TargetElapsedTime);

            graphicsManager.IsFullScreen = true;
            graphicsManager.SynchronizeWithVerticalRetrace = false;
            IsMouseVisible = true;
            
            NetPeerConfiguration config = new NetPeerConfiguration("DETests");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            config.EnableMessageType(NetIncomingMessageType.StatusChanged);
            config.EnableMessageType(NetIncomingMessageType.Data);
            config.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            config.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
            config.PingInterval = 5.0f;
            config.ConnectionTimeout = 30F;
            config.EnableUPnP = true;
            config.Port = 12345;
            
            NetPeerConfiguration config2 = new NetPeerConfiguration("DETests");
            config2.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            config2.EnableMessageType(NetIncomingMessageType.StatusChanged);
            config2.EnableMessageType(NetIncomingMessageType.Data);
            config2.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            config2.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
            config2.PingInterval = 5.0f;
            config2.ConnectionTimeout = 30F;
            config2.EnableUPnP = true;
            config2.Port = 12346;

            NetClient peer = new NetClient(config);
            NetServer peer2 = new NetServer(config2);
            
            peer.Start();
            peer2.Start();

            peer.Connect(new IPEndPoint(NetUtility.Resolve("localhost"), 12346));
            
            Thread.Sleep(10);
        }

        protected void PlayerJoined(object sender, IPacket packet)
        {
            Packet_PlayerJoined inPacket = (Packet_PlayerJoined)packet;
            PlayerInfo info = inPacket.Info;

            Debug.WriteLine(info.UserName);
        }

        protected override void Initialize()
        {
            base.Initialize();
            // graphicsManager.PreferredBackBufferWidth = 2048;
            // graphicsManager.PreferredBackBufferHeight = 786;

            _prevMouseState = Mouse.GetState();
            
            //Packet_PlayerJoined packet = new Packet_PlayerJoined(new PlayerInfo("Dando Cuntrissian"));
            //_manager.SendMessage(packet);

            Global.Graphics = GraphicsDevice;
        }

        protected override void LoadContent()
        {
            _font = Content.Load<SpriteFont>("GUIFont");
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Effect e = new Effect(GraphicsDevice, EmbeddedResources.LightShader);

            _pixelTex = new Texture2D(GraphicsDevice, 1, 1);
            _pixelTex.SetData(new Color[] { Color.White });

            Random rand = new Random();

            _region = new PixelTerrain(2048, 1024, GraphicsDevice, new PerlinTerrainBuilder(rand.Next()), Content.Load<Texture2D>("dayNightGradient"), Content.Load<Texture2D>("stars"));
            _light = new PointLight(GraphicsDevice, new Vector2(150, 150), 500, Color.Red);
            _spotLight = new SpotLight(GraphicsDevice, new Vector2(400, 200), 600, Color.White, new Vector2(450, 250), 120);
            _region.AddLights(new ILight[] 
            { 
                _light, 
                _spotLight,
                new PointLight(GraphicsDevice, new Vector2(150, 300), 300, Color.Blue),
                new PointLight(GraphicsDevice, new Vector2(250, 250), 300, Color.Green)
            });
            
            ILight[] lights = new PointLight[0];
            for (int i = 0; i < lights.Length; i++)
                lights[i] = new PointLight(GraphicsDevice, new Vector2(rand.Next(800), rand.Next(400)), rand.Next(100), new Color(rand.Next(255), rand.Next(255), rand.Next(255), rand.Next(255)));
            _region.AddLights(lights);

                _sampleBrush = new TexturePixelBrush(Content.Load<Texture2D>("RoundBrush"));
            _rectBrush = new DynamicRectBrush(Rectangle.Empty);

            TileManager tileManager = new TileManager();
            tileManager.RegisterTile("grass", new Tile(0, 10));
            tileManager.RegisterTile("water", new Tile(1, 34));
            tileManager.RegisterTile("smallTree", new Tile(2, 19));

            //TextureAtlas atlas = new TextureAtlas(Content.Load<Texture2D>("TDTerrain"), 8, 8);

            //_map = new SimpleTileMap(GraphicsDevice, Content.Load<SpriteFont>("GUIFont"), tileManager, atlas, 64, 64, 4);
            //_map.GenTree(5, 5, 0);
            _camera = new Camera2D(GraphicsDevice);

            BuildUserInterface();
        }

        private void BuildUserInterface()
        {
            _mainContainer = new GUIPanel(GraphicsDevice, null);
            _mainContainer.Bounds = new Rectangle(0, 0, 200, 100);
            _mainContainer.BackColor = Color.Black * 0.75f;

            _framerateLabel = new GUILabel(GraphicsDevice, null, _mainContainer);
            _framerateLabel.Text = "FPS: ";
            _framerateLabel.Alignment = TextAlignment.Centred;
            _framerateLabel.Location = new Point(_mainContainer.Width / 2, _framerateLabel.Height / 2 + _mainContainer.Margin);
            _framerateLabel.ForeColor = Color.White;

            _pauseButton = new GUIButton(GraphicsDevice, null, _mainContainer);
            _pauseButton.Text = "Pause";
            _pauseButton.Alignment = TextAlignment.Centred;
            _pauseButton.Width = 60;
            _pauseButton.Height = 15;
            _pauseButton.X = _mainContainer.Width / 2 - _pauseButton.Bounds.Width / 2;
            _pauseButton.Y = _framerateLabel.Bounds.Bottom + 5;
            _pauseButton.ForeColor = Color.White;
            _pauseButton.OnMousePressed += _pauseButton_OnMousePressed;
        }

        void _pauseButton_OnMousePressed()
        {
            Global.GameSpeed = Global.GameSpeed == 0 ? 1.0f : 0.0f;
            _pauseButton.Text = Global.GameSpeed == 0 ? "Resume" : "Pause";
        }

        protected override void OnDeactivated(object sender, EventArgs args)
        {
            _isDragging = false;
        }

        protected override void Update(GameTime gameTime)
        {
            MouseState state = Mouse.GetState();

            MouseEventArgs e = new MouseEventArgs(state, _prevMouseState);
            _mainContainer.Update();

            if (e.IsImportant)
            {
                if (e.LeftButton == ButtonChangeState.Pressed)
                    _mainContainer.MousePressed(e);
            }

            if (state.LeftButton == ButtonState.Pressed)
                _region.ApplyBrush(_sampleBrush, state.Position.ToVector2(), 0);

            _spotLight.Rotation += (float)gameTime.ElapsedGameTime.TotalSeconds * 45.0f * Global.GameSpeed;

            if (e.RightButton == ButtonChangeState.Released)
            {
                _isDragging = !_isDragging;

                if (!_isDragging)
                {
                    _rectBrush.Bounds = new Rectangle(Math.Min(_beginRect.X, state.X), Math.Min(_beginRect.Y, state.Y), Math.Abs(_beginRect.X - state.X), Math.Abs(_beginRect.Y - state.Y));
                    _region.ApplyBrush(_rectBrush, new Vector2(_rectBrush.Bounds.X + _rectBrush.HalfSize.X, _rectBrush.Bounds.Y + _rectBrush.HalfSize.Y));
                }
                else
                {
                    _beginRect = state.Position;
                }
            }
            KeyboardState keyState = Keyboard.GetState();

            //float lerp = (float)(Math.Sin(gameTime.TotalGameTime.TotalSeconds) + 1.0f) / 2.0f;
            //_region.AmbientColor = Color.Lerp(Color.FromNonPremultiplied(24, 24, 48, 255), Color.FromNonPremultiplied(160, 160, 200, 255), lerp);

            Window.Title = "Time: " + gameTime.ElapsedGameTime.TotalSeconds;

            _region.Update(gameTime);

            if (keyState.IsKeyDown(Keys.Left))
                _light.Position -= Vector2.UnitX;
            if (keyState.IsKeyDown(Keys.Right))
                _light.Position += Vector2.UnitX;

            if (keyState.IsKeyDown(Keys.Escape))
                Exit();

            _prevMouseState = state;
        }

        protected override void Draw(GameTime gameTime)
        {
            _region.Render();

            _fpsCounter.AddFrameToBuffer(gameTime.ElapsedGameTime);

            _framerateLabel.Text = string.Format("FPS: {0}", _fpsCounter.FramesPerSecond);
            _mainContainer.Draw();

            base.Draw(gameTime);
        }

        private class TextureTerrainBuilder : TerrainBuilder
        {
            Color[] dat;

            public TextureTerrainBuilder(Texture2D texture) : base(texture) 
            {
                dat = new Color[texture.Width * texture.Height];
                texture.GetData(dat);
            }

            public override Color GetColorAtPosition(int x, int y)
            {
                x = x.Wrap(0, ((Texture2D)_seed).Width);
                y = y.Wrap(0, ((Texture2D)_seed).Height);
                return dat[x + y * ((Texture2D)_seed).Width];
            }
        }
        }
}
