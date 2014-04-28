using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoodleEmpires.Engine.Utilities;
using DoodleEmpires.Engine.Terrain;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DoodleEmpires.Engine.Entities;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using Lidgren.Network;
using DoodleEmpires.Engine.GUI;
using System.IO;
using System.Windows.Forms;
using DoodleEmpires.Engine.Sound;
using Microsoft.Xna.Framework.Audio;
using System.ComponentModel;
using DoodleEmpires.Engine.Economy;

using MouseEventArgs = DoodleEmpires.Engine.Utilities.MouseEventArgs;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace DoodleEmpires.Engine.Net
{
    /// <summary>
    /// The final class for the Doodle Empires game client
    /// </summary>
    public class SPGame : AdvancedGame
    {
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
        protected GUILabel _fpsLabel;

        protected SpriteFont _guiFont;

        protected byte _editType = 1;

        protected Texture2D[] _blockTexs;

        protected bool _isDefininingZone = false;
        protected Vector2 _zoneStart = Vector2.Zero;

        protected GameState _gameState = GameState.InGame;

        /// <summary>
        /// Gets or sets a tile at the given chunk coords
        /// </summary>
        /// <param name="x">The x coordinate (chunk coords)</param>
        /// <param name="y">The y coordinate (chunk coords)</param>
        /// <returns>The voxel type at (x, y)</returns>
        public byte this[int x, int y]
        {
            get { return _map[x, y]; }
            set
            {
                _map[x, y] = value;
            }
        }
        
        public SPGame()
            : base()
        {
            graphicsManager = new GraphicsDeviceManager(this);
            graphicsManager.PreferMultiSampling = false;
            graphicsManager.SynchronizeWithVerticalRetrace = true;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = false;
        }
        
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _tileManager = GlobalTileManager.TileManager;

            _blockAtlas = new TextureAtlas(Content.Load<Texture2D>("Atlas"), 20, 20);

            _map = new SPMap(GraphicsDevice, _tileManager, _blockAtlas, 800, 400);
            _view = new Camera2D(GraphicsDevice);
            _view.ScreenBounds = new Rectangle(0, 0, _map.WorldWidth, _map.WorldHeight);

            _cameraController = new CameraControl(_view);
            _cameraController.Position = new Vector2(0, 200 * SPMap.TILE_HEIGHT);

            _view.Focus = _cameraController;
                        
            _rand = new Random();
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _paperTex = Content.Load<Texture2D>("Paper");

            _map.BackDrop = _paperTex;

            _debugFont = Content.Load<SpriteFont>("debugFont");
            _guiFont = Content.Load<SpriteFont>("GUIFont");
            _guiFont.FixFont();

            _soundEngine = new SoundEngine();

            foreach (string s in Directory.GetFiles("Content\\Sounds"))
            {
                string sName = Path.GetFileNameWithoutExtension(s);
                _soundEngine.AddSound(sName, Content.Load<SoundEffect>("Sounds\\" + sName));
            }

            _blockTexs = _blockAtlas.GetTextures(GraphicsDevice);
            
            _mainControl = new GUIPanel(GraphicsDevice, null);
            _mainControl.Bounds = new Rectangle(0, 0, 120, 165);

            _fpsLabel = new GUILabel(GraphicsDevice, _guiFont, _mainControl);
            _fpsLabel.Text = "";

            GUIButton saveButton = new GUIButton(GraphicsDevice, _guiFont, _mainControl);
            saveButton.Text = "Save";
            saveButton.Bounds = new Rectangle(5, 140, 40, 20);
            saveButton.OnMousePressed += new Action(saveButton_OnMousePressed);

            GUIButton loadButton = new GUIButton(GraphicsDevice, _guiFont, _mainControl);
            loadButton.Text = "Load";
            loadButton.Bounds = new Rectangle(50, 140, 40, 20);
            loadButton.OnMousePressed += new Action(loadButton_OnMousePressed);

            GUIGridView gridView = new GUIGridView(GraphicsDevice, _mainControl);
            gridView.Bounds = new Rectangle(0, 12, 121, 121);
            gridView.Font = _guiFont;
            gridView.HeaderText = "Block:";

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

        void loadButton_OnMousePressed()
        {
            OpenFileDialog loadDialog = new OpenFileDialog();
            loadDialog.Filter = "Doodle Empires Map|*.dem";
            loadDialog.AddExtension = false;

            DialogResult dResult = loadDialog.ShowDialog();

            if (dResult == DialogResult.OK || dResult == DialogResult.Yes)
                LoadGame(loadDialog.FileName.Replace(".dem",""));
            
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

        protected override void OnDeactivated(object sender, EventArgs args)
        {
            Mouse.GetState();
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
            base.Update(gameTime);
            
            #if PROFILING
            Window.Title = "" + FPSManager.AverageFramesPerSecond;

            if (Keyboard.GetState().IsKeyDown(Keys.PageUp))
                Thread.Sleep(1);

            if (Keyboard.GetState().IsKeyDown(Keys.PageDown))
                return;
            #endif

            switch (_gameState)
            {
                case GameState.InGame:
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                        Exit();

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

                    _prevKeyState = keyState;
                    break;
            }
        }

        protected override void MousePressed(MouseEventArgs args)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.LeftAlt) & args.LeftButton == ButtonState.Pressed)
            {
                _isDefininingZone = true;
                _zoneStart = _view.PointToWorld(args.Location);
            }
            else if (args.RightButton == ButtonState.Pressed)
            {
                Vector2 worldPos = _view.PointToWorld(args.Location);

                foreach (Zoning z in _map.Zones)
                {
                    if (z.Bounds.Contains(worldPos))
                    {
                        _map.DeleteZone(z);
                        break;
                    }
                }
            }

            base.MousePressed(args);
        }

        /// <summary>
        /// Called when a mouse button is down
        /// </summary>
        /// <param name="args">The mouse event arguments</param>
        protected override void MouseDown(MouseEventArgs args)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed || Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                _mouseWorldPos = _view.PointToWorld(args.Location);
                _mainControl.MousePressed(args);

                if (!_mainControl.ScreenBounds.Contains(args.Location) && !_isDefininingZone)
                {
                    if (args.LeftButton == ButtonState.Pressed)
                    {
                        _map.SetTileSafe((int)_mouseWorldPos.X / SPMap.TILE_WIDTH, (int)_mouseWorldPos.Y / SPMap.TILE_HEIGHT, _editType);
                    }
                    else if (args.RightButton == ButtonState.Pressed)
                    {
                        _map.SetTileSafe((int)_mouseWorldPos.X / SPMap.TILE_WIDTH, (int)_mouseWorldPos.Y / SPMap.TILE_HEIGHT, 0);
                    }
                }

                base.MousePressed(args);
            }
        }

        protected override void MouseReleased(MouseEventArgs args)
        {
            if (_isDefininingZone & args.LeftButton == ButtonState.Pressed)
            {
                Vector2 zoneEnd = _view.PointToWorld(args.Location);

                _map.DefineZone(_zoneStart, zoneEnd, new StockPileZone());

                _isDefininingZone = false;
            }

            base.MouseReleased(args);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.Clear(Color.White);
                        
            FPSManager.OnDraw(gameTime);

            switch (_gameState)
            {
                case GameState.InGame:
                    GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

                    _map.Render(_view);

                    //_fpsLabel.Text = " FPS: " + Math.Round(FPSManager.AverageFramesPerSecond, 1);
                    _mainControl.Draw();
                    break;
            }

            base.Draw(gameTime);
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
                _map = SPMap.ReadFromStream(fileStream, GraphicsDevice, _tileManager, _blockAtlas);
                fileStream.Close();
                fileStream.Dispose();

                _map.BackDrop = _paperTex;
            }
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
    }
}
