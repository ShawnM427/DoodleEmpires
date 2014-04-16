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
        Camera2D _view;
        CameraControl _cameraController;
        Texture2D _paperTex;

        KeyboardState _kS;
        Vector2 _moveVector;
        Vector2 _mouseWorldPos;

        Vector2 _mouseTextPos = new Vector2(5, 5);
        Vector2 _framerateTextPos = new Vector2(5, 25);
        Vector2 _updateTickPos = new Vector2(5, 45);
        Vector2 _drawTickPos = new Vector2(5, 65);
        Vector2 _otherTickPos = new Vector2(5, 85);
        
        Random _rand;

        GUIContainer _mainControl;

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
            TileManager _tileManager = new TileManager();
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

            _voxelTerrain = new VoxelTerrain(GraphicsDevice, _tileManager, new TextureAtlas(Content.Load<Texture2D>("Atlas"), 20, 20), 800, 400);
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
            
            _mainControl = new GUIPanel(GraphicsDevice, null);
            _mainControl.Bounds = new Rectangle(20, 60, 60, 120);
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

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _kS = Keyboard.GetState();
            _moveVector = Vector2.Zero;

            _moveVector.X += _kS.IsKeyDown(Keys.Right) ? 1 : _kS.IsKeyDown(Keys.Left) ? -1 : 0;
            _moveVector.Y += _kS.IsKeyDown(Keys.Up) ? -1 : _kS.IsKeyDown(Keys.Down) ? 1 : 0;

            _view.Scale *= _kS.IsKeyDown(Keys.OemPlus) ? 1.01F : _kS.IsKeyDown(Keys.OemMinus) ? 0.99F : 1;
            
            _cameraController.Position += _moveVector * 10;
            _cameraController.Update(gameTime);
            _view.Update(gameTime);

            _mainControl.Update();
        }

        /// <summary>
        /// Called when a mouse button is pressed
        /// </summary>
        /// <param name="args">The mouse event arguments</param>
        protected override void MousePressed(MouseEventArgs args)
        {
            _mouseWorldPos = _view.PointToWorld(args.Location);

            if (args.LeftButton == ButtonState.Pressed & Keyboard.GetState().IsKeyDown(Keys.LeftControl))
            {
                _voxelTerrain.SetTileSafe((int)_mouseWorldPos.X / VoxelTerrain.TILE_WIDTH, (int)_mouseWorldPos.Y / VoxelTerrain.TILE_HEIGHT, 5);
            }
            else if (args.LeftButton == ButtonState.Pressed & Keyboard.GetState().IsKeyDown(Keys.LeftAlt))
            {
                _voxelTerrain.GenTree((int)_mouseWorldPos.X / VoxelTerrain.TILE_WIDTH, (int)_mouseWorldPos.Y / VoxelTerrain.TILE_HEIGHT);
            }
            else if (args.LeftButton == ButtonState.Pressed)
            {
                _voxelTerrain.SetTileSafe((int)_mouseWorldPos.X / VoxelTerrain.TILE_WIDTH, (int)_mouseWorldPos.Y / VoxelTerrain.TILE_HEIGHT, 4);
            }
            if (args.RightButton == ButtonState.Pressed & Keyboard.GetState().IsKeyDown(Keys.LeftAlt))
            {
                _voxelTerrain.SetMeta((int)_mouseWorldPos.X / VoxelTerrain.TILE_WIDTH, (int)_mouseWorldPos.Y / VoxelTerrain.TILE_HEIGHT, 1);
            }
            else if (args.RightButton == ButtonState.Pressed)
            {
                _voxelTerrain.SetTileSafe((int)_mouseWorldPos.X / VoxelTerrain.TILE_WIDTH, (int)_mouseWorldPos.Y / VoxelTerrain.TILE_HEIGHT, 0);
            }

            base.MousePressed(args);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            FPSManager.OnDraw(gameTime);

            //_terrain.Render(_view);
            _voxelTerrain.Render(_view);

            SpriteBatch.Begin();
            SpriteBatch.DrawString(_debugFont, "MousePos: " + _mouseWorldPos, _mouseTextPos, Color.Red);
            SpriteBatch.DrawString(_debugFont, "Framerate: " + FPSManager.AverageFramesPerSecond, _framerateTextPos, Color.Red);
            SpriteBatch.End();

            _mainControl.Draw();

            base.Draw(gameTime);
        }
    }
}
