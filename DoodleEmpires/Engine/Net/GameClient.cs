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

using MouseEventArgs = DoodleEmpires.Engine.Utilities.MouseEventArgs;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using DoodleEmpires.Engine.Render.Particle;
using DoodleEmpires.Engine.Sound;
using Microsoft.Xna.Framework.Audio;
using System.ComponentModel;

namespace DoodleEmpires.Engine.Net
{
    /// <summary>
    /// The final class for the Doodle Empires game client
    /// </summary>
    public sealed class GameClient : AdvancedGame
    {
        GraphicsDeviceManager graphicsManager;
        SpriteFont _debugFont;

        VoxelMap _voxelTerrain;
        TileManager _tileManager;
        TextureAtlas _blockAtlas;

        SoundEngine _soundEngine;

        Camera2D _view;
        CameraControl _cameraController;
        Texture2D _paperTex;

        KeyboardState _prevKeyState;
        Vector2 _moveVector;
        Vector2 _mouseWorldPos;

        Vector2 _mouseTextPos = new Vector2(5, 5);
        Vector2 _framerateTextPos = new Vector2(5, 25);
        Vector2 _updateTickPos = new Vector2(5, 45);
        Vector2 _drawTickPos = new Vector2(5, 65);
        Vector2 _otherTickPos = new Vector2(5, 85);
        
        Random _rand;

        GUIContainer _mainControl;
        GUILabel _fpsLabel;

        byte _editType = 1;

        Texture2D[] _blockTexs;

        /// <summary>
        /// Gets or sets a tile at the given chunk coords
        /// </summary>
        /// <param name="x">The x coordinate (chunk coords)</param>
        /// <param name="y">The y coordinate (chunk coords)</param>
        /// <returns>The voxel type at (x, y)</returns>
        public byte this[int x, int y]
        {
            get { return _voxelTerrain[x, y]; }
            set
            {
                _voxelTerrain[x, y] = value;

                NetOutgoingMessage message = NetManager.BuildMessage();
                message.Write((short)PacketTypes.MapSet);
                message.Write((short)x);
                message.Write((short)y);
                message.Write(value);
                NetManager.SendMessage(message);
            }
        }

        public GameClient()
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
            _tileManager = new TileManager();
            _tileManager.RegisterTile("Grass", 0, Color.Green, RenderType.Land, true);
            _tileManager.RegisterTile("Stone", 0, Color.Gray, RenderType.Land, true);
            _tileManager.RegisterTile("Concrete", 0, Color.LightGray, RenderType.Land, true);
            _tileManager.RegisterTile("Wood", 20, Color.Brown, RenderType.Land, false);
            _tileManager.RegisterTile("Leaves", new Leaves(0));
            _tileManager.RegisterTile("Cobble", 60, Color.Gray, RenderType.Land, true);
            _tileManager.RegisterTile("Wooden Spikes", new WoodSpike(0));
            _tileManager.RegisterTile("Ladder", new Ladder(0));
            _tileManager.RegisterTile("Door", new Door(0));

            _tileManager.RegisterConnect("Grass", "Stone");
            _tileManager.RegisterConnect("Grass", "Concrete");
            _tileManager.RegisterConnect("Stone", "Concrete");
            _tileManager.RegisterConnect("Wood", "Grass");
            _tileManager.RegisterConnect("Wood", "Leaves");

            _tileManager.RegisterOneWayConnect("Ladder", "Door");
            _tileManager.RegisterOneWayConnect("Ladder", "Wood");
            _tileManager.RegisterOneWayConnect("Ladder", "Concrete");
            _tileManager.RegisterOneWayConnect("Ladder", "Stone");

            _blockAtlas = new TextureAtlas(Content.Load<Texture2D>("Atlas"), 20, 20);

            _voxelTerrain = new VoxelMap(GraphicsDevice, _tileManager, _blockAtlas, 800, 400);
            _view = new Camera2D(GraphicsDevice);
            _view.ScreenBounds = new Rectangle(0, 0, _voxelTerrain.WorldWidth, _voxelTerrain.WorldHeight);

            _cameraController = new CameraControl(_view);
            _cameraController.Position = new Vector2(0, 200 * VoxelMap.TILE_HEIGHT);

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

            _voxelTerrain.BackDrop = _paperTex;

            _debugFont = Content.Load<SpriteFont>("debugFont");
            SpriteFont _guiFont = Content.Load<SpriteFont>("GUIFont");
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

            _cameraController.Update(gameTime);
            _view.Update(gameTime);

            #if PROFILING
            Window.Title = "" + FPSManager.AverageFramesPerSecond;

            if (Keyboard.GetState().IsKeyDown(Keys.PageUp))
                Thread.Sleep(1);

            if (Keyboard.GetState().IsKeyDown(Keys.PageDown))
                return;
            #endif

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState keyState = Keyboard.GetState();
            _moveVector = Vector2.Zero;

            _moveVector.X += keyState.IsKeyDown(Keys.Right) ? 1 : keyState.IsKeyDown(Keys.Left) ? -1 : 0;
            _moveVector.Y += keyState.IsKeyDown(Keys.Up) ? -1 : keyState.IsKeyDown(Keys.Down) ? 1 : 0;

            _view.Scale *= keyState.IsKeyDown(Keys.OemPlus) ? 1.01F : keyState.IsKeyDown(Keys.OemMinus) ? 0.99F : 1;

            _cameraController.Position += _moveVector * 10;

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

                if (!_mainControl.ScreenBounds.Contains(args.Location))
                {
                    if (args.LeftButton == ButtonState.Pressed)
                    {
                        _voxelTerrain.SetTileSafe((int)_mouseWorldPos.X / VoxelMap.TILE_WIDTH, (int)_mouseWorldPos.Y / VoxelMap.TILE_HEIGHT, _editType);
                    }
                    else if (args.RightButton == ButtonState.Pressed)
                    {
                        _voxelTerrain.SetTileSafe((int)_mouseWorldPos.X / VoxelMap.TILE_WIDTH, (int)_mouseWorldPos.Y / VoxelMap.TILE_HEIGHT, 0);
                    }
                }

                base.MousePressed(args);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.Clear(Color.White);
                        
            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            FPSManager.OnDraw(gameTime);
            
            _voxelTerrain.Render(_view);

            //_fpsLabel.Text = " FPS: " + Math.Round(FPSManager.AverageFramesPerSecond, 1);
            _mainControl.Draw();


            base.Draw(gameTime);
        }

        /// <summary>
        /// Saves this game to a given file
        /// </summary>
        /// <param name="fName">The relative file name to save to</param>
        private void SaveGame(string fName)
        {
            Stream fileStream = File.OpenWrite(fName.Replace(".dem", "") + ".dem");
            _voxelTerrain.SaveToStream(fileStream);
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
                _voxelTerrain = VoxelMap.ReadFromStream(fileStream, GraphicsDevice, _tileManager, _blockAtlas);
                fileStream.Close();
                fileStream.Dispose();
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

        public void HandleNetMassage(NetIncomingMessage message)
        {
            PacketTypes MID = (PacketTypes)message.ReadInt16();

            switch (MID)
            {
                case PacketTypes.MapSet:
                    this[message.ReadInt16(), message.ReadInt16()] = message.ReadByte();
                    break;
            }
        }
    }
}
