#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using DoodleEmpires.Engine.Terrain;
using DoodleEmpires.Engine.Render;
using DoodleEmpires.Engine.Entities;
using DoodleEmpires.Engine.Utilities;
using System.Diagnostics;
#endregion

namespace DoodleEmpires
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphicsManager;
        SpriteBatch spriteBatch;
        SpriteFont _debugFont;

        Terrain _terrain;
        VoxelTerrain _voxelTerrain;
        Camera2D _view;
        CameraControl _cameraController;
        Texture2D _paperTex;
        Rectangle _paperBounds;

        KeyboardState _kS;
        MouseState _mS;
        Vector2 _moveVector;
        Vector2 _mouseWorldPos;

        Vector2 _mouseTextPos = new Vector2(5, 5);
        Vector2 _framerateTextPos = new Vector2(5, 25);
        Vector2 _updateTickPos = new Vector2(5, 45);
        Vector2 _drawTickPos = new Vector2(5, 65);
        Vector2 _otherTickPos = new Vector2(5, 85);

        Stopwatch _watch = new Stopwatch();
        double _updateTick = 0;
        double _drawTick = 0;
        double _updateMS;
        double _drawMS;
        double _otherMS;
        double _accuracy;

        public Game1()
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
            TileManager _tileManager = new TileManager();
            _tileManager.RegisterTile(0, RenderType.Land, true);
            _tileManager.RegisterTile(1, RenderType.Land, true);
            _tileManager.RegisterConnect(1, 1);

            _terrain = new Terrain(8, 4.0F, 1600, GraphicsDevice, MapColorScheme.Sketch, Content.Load<Texture2D>("Paper"), Content.Load<Texture2D>("woodTex"));
            _voxelTerrain = new VoxelTerrain(GraphicsDevice, _tileManager, new TextureAtlas(Content.Load<Texture2D>("Atlas"), 16, 256), 800, 400);
            _cameraController = new CameraControl();
            _view = new Camera2D(GraphicsDevice);
            _view.Focus = _cameraController;


            _cameraController.Position = new Vector2(0, 200 * VoxelTerrain.TILE_HEIGHT);

            _accuracy = Stopwatch.Frequency;
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _paperTex = Content.Load<Texture2D>("Paper");
            _paperBounds = new Rectangle(0, 0, (int)_terrain.TotalWidth, 1000);

            _debugFont = Content.Load<SpriteFont>("debugFont");
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
            _watch.Stop();
            _otherMS = Math.Round((_watch.ElapsedTicks / _accuracy) * 1000.0, 4);
            _watch.Restart();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _kS = Keyboard.GetState();
            _mS = Mouse.GetState();
            _moveVector = Vector2.Zero;
            _mouseWorldPos = _view.PointToWorld(new Vector2(_mS.X, _mS.Y));

            _moveVector.X += _kS.IsKeyDown(Keys.Right) ? 1 : _kS.IsKeyDown(Keys.Left) ? -1 : 0;
            _moveVector.Y += _kS.IsKeyDown(Keys.Up) ? -1 : _kS.IsKeyDown(Keys.Down) ? 1 : 0;

            if (_mS.LeftButton == ButtonState.Pressed)
            {
                Vector2 point = _view.PointToWorld(_mS.X, _mS.Y);
                _voxelTerrain.SetTileSafe((int)point.X / VoxelTerrain.TILE_WIDTH, (int)point.Y / VoxelTerrain.TILE_HEIGHT, 1);
            }
            if (_mS.RightButton == ButtonState.Pressed)
            {
                Vector2 point = _view.PointToWorld(_mS.X, _mS.Y);
                _voxelTerrain.SetTileSafe((int)point.X / VoxelTerrain.TILE_WIDTH, (int)point.Y / VoxelTerrain.TILE_HEIGHT, 0);
            }
            
            _view.Scale *= _kS.IsKeyDown(Keys.OemPlus) ? 1.01F : _kS.IsKeyDown(Keys.OemMinus) ? 0.99F : 1;

            if (_mS.LeftButton == ButtonState.Pressed)
                _terrain.SetMaterial((int)Math.Round(_mouseWorldPos.X / 4.0F) + 1, Materials.Stone, true);

            if (_mS.RightButton == ButtonState.Pressed)
                _terrain.ChangeHeight((int)Math.Round(_mouseWorldPos.X / 4.0F) + 1, 1.0F);

            _cameraController.Position += _moveVector * 10;
            _cameraController.Update(gameTime);
            _view.Update(gameTime);
            
            //base.Update(gameTime);

            _watch.Stop();
            _updateTick = _watch.ElapsedTicks;
            _updateMS = Math.Round((_updateTick / _accuracy) * 1000.0, 4);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            _watch.Restart();
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            FPSManager.OnDraw(gameTime);

            //_terrain.Render(_view);
            _voxelTerrain.Render(_view);

            spriteBatch.Begin();
            spriteBatch.DrawString(_debugFont, "MousePos: " + _mouseWorldPos, _mouseTextPos, Color.Red);
            spriteBatch.DrawString(_debugFont, "Framerate: " + FPSManager.AverageFramesPerSecond, _framerateTextPos, Color.Red);
            spriteBatch.DrawString(_debugFont, "Update MS: " + _updateMS, _updateTickPos, Color.Red);
            spriteBatch.DrawString(_debugFont, "Draw MS: " + _drawMS, _drawTickPos, Color.Red);
            spriteBatch.DrawString(_debugFont, "Other MS: " + _otherMS, _otherTickPos, Color.Red);
            spriteBatch.End();

            //base.Draw(gameTime);

            _watch.Stop();
            _drawTick = _watch.ElapsedTicks;
            _drawMS = Math.Round((_drawTick / _accuracy) * 1000.0, 4);
            
            _watch.Restart();
        }
    }
}
