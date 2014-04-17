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

namespace DoodleEmpires.Engine.Net
{
    /// <summary>
    /// The final class for the Doodle Empires game client
    /// </summary>
    public sealed class GameClient : AdvancedGame
    {
        GraphicsDeviceManager graphicsManager;
        SpriteFont _debugFont;

        VoxelTerrain _voxelTerrain;
        TileManager _tileManager;
        TextureAtlas _blockAtlas;
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
            _tileManager.RegisterTile(new Leaves(0), "Leaves");
            _tileManager.RegisterTile("Cobble", 60, Color.Gray, RenderType.Land, true);

            _tileManager.RegisterConnect("Grass", "Stone");
            _tileManager.RegisterConnect("Grass", "Concrete");
            _tileManager.RegisterConnect("Grass", "Cobble");
            _tileManager.RegisterConnect("Leaves", "Wood");

            _blockAtlas = new TextureAtlas(Content.Load<Texture2D>("Atlas"), 20, 20);

            _voxelTerrain = new VoxelTerrain(GraphicsDevice, _tileManager, _blockAtlas, 800, 400);
            _cameraController = new CameraControl();
            _view = new Camera2D(GraphicsDevice);
            _view.Focus = _cameraController;

            _cameraController.Position = new Vector2(0, 200 * VoxelTerrain.TILE_HEIGHT);
            
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

            _debugFont = Content.Load<SpriteFont>("debugFont");
            SpriteFont _guiFont = Content.Load<SpriteFont>("GUIFont");
            _guiFont.FixFont();

            _blockTexs = _blockAtlas.GetTextures(GraphicsDevice);
            
            _mainControl = new GUIPanel(GraphicsDevice, null);
            _mainControl.Bounds = new Rectangle(0, 0, 120, 140);

            _fpsLabel = new GUILabel(GraphicsDevice, _guiFont, _mainControl);
            _fpsLabel.Text = "";

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

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState keyState = Keyboard.GetState();
            _moveVector = Vector2.Zero;

            _moveVector.X += keyState.IsKeyDown(Keys.Right) ? 1 : keyState.IsKeyDown(Keys.Left) ? -1 : 0;
            _moveVector.Y += keyState.IsKeyDown(Keys.Up) ? -1 : keyState.IsKeyDown(Keys.Down) ? 1 : 0;

            _view.Scale *= keyState.IsKeyDown(Keys.OemPlus) ? 1.01F : keyState.IsKeyDown(Keys.OemMinus) ? 0.99F : 1;

            if ((keyState.IsKeyDown(Keys.LeftControl) & keyState.IsKeyDown(Keys.S))
                && (_prevKeyState.IsKeyUp(Keys.LeftControl) | _prevKeyState.IsKeyUp(Keys.S)))
            {
                Stream fileStream = File.OpenWrite("tempSave.dem");
                _voxelTerrain.SaveToStream(fileStream);
                fileStream.Close();
                fileStream.Dispose();
            }

            if ((keyState.IsKeyDown(Keys.LeftControl) & keyState.IsKeyDown(Keys.L))
                && (_prevKeyState.IsKeyUp(Keys.LeftControl) | _prevKeyState.IsKeyUp(Keys.L)))
            {
                if (File.Exists("tempSave.dem"))
                {
                    Stream fileStream = File.OpenRead("tempSave.dem");
                    _voxelTerrain = VoxelTerrain.ReadFromStream(fileStream, GraphicsDevice, _tileManager, _blockAtlas);
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }

            _cameraController.Position += _moveVector * 10;
            _cameraController.Update(gameTime);
            _view.Update(gameTime);

            _mainControl.Update();

            _prevKeyState = keyState;
        }

        /// <summary>
        /// Called when a mouse button is down
        /// </summary>
        /// <param name="args">The mouse event arguments</param>
        protected override void MouseDown(MouseEventArgs args)
        {
            _mouseWorldPos = _view.PointToWorld(args.Location);
            _mainControl.MousePressed(args);

            if (!_mainControl.ScreenBounds.Contains(args.Location))
            {
                if (args.LeftButton == ButtonState.Pressed)
                {
                    _voxelTerrain.SetTileSafe((int)_mouseWorldPos.X / VoxelTerrain.TILE_WIDTH, (int)_mouseWorldPos.Y / VoxelTerrain.TILE_HEIGHT, _editType);
                }
                else if (args.RightButton == ButtonState.Pressed)
                {
                    _voxelTerrain.SetTileSafe((int)_mouseWorldPos.X / VoxelTerrain.TILE_WIDTH, (int)_mouseWorldPos.Y / VoxelTerrain.TILE_HEIGHT, 0);
                }
            }

            base.MousePressed(args);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            FPSManager.OnDraw(gameTime);
            
            _voxelTerrain.Render(_view);
            
            _fpsLabel.Text = " FPS: " + Math.Round(FPSManager.AverageFramesPerSecond, 1);
            _mainControl.Draw();


            base.Draw(gameTime);
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
