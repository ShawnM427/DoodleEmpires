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
#endregion

namespace DoodleEmpires
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont _debugFont;

        Terrain _terrain;
        Camera2D _view;
        CameraControl _cameraController;
        Texture2D _paperTex;
        Rectangle _paperBounds;

        KeyboardState _kS;
        MouseState _mS;
        Vector2 _moveVector;
        Vector2 _mouseWorldPos;

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
            _terrain = new Terrain(8, 4.0F, 1600, GraphicsDevice, MapColorScheme.Default, Content.Load<Texture2D>("Paper"), Content.Load<Texture2D>("woodTex"));
            _cameraController = new CameraControl();
            _view = new Camera2D(GraphicsDevice);
            _view.Focus = _cameraController;

            _cameraController.Position = new Vector2(0, 0);

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _kS = Keyboard.GetState();
            _mS = Mouse.GetState();
            _moveVector = Vector2.Zero;
            _mouseWorldPos = _view.PointToWorld(new Vector2(_mS.X, _mS.Y));

            _moveVector.X += _kS.IsKeyDown(Keys.Right) ? 1 : _kS.IsKeyDown(Keys.Left) ? -1 : 0;
            _moveVector.Y += _kS.IsKeyDown(Keys.Up) ? -1 : _kS.IsKeyDown(Keys.Down) ? 1 : 0;
            
            _view.Scale *= _kS.IsKeyDown(Keys.OemPlus) ? 1.01F : _kS.IsKeyDown(Keys.OemMinus) ? 0.99F : 1;

            if (_mS.LeftButton == ButtonState.Pressed)
                _terrain.SetMaterial((int)Math.Round(_mouseWorldPos.X / 4.0F) + 1, Materials.Stone, true);

            if (_mS.RightButton == ButtonState.Pressed)
                _terrain.ChangeHeight((int)Math.Round(_mouseWorldPos.X / 4.0F) + 1, 1.0F);

            _cameraController.Position += _moveVector * 10;
            _cameraController.Update(gameTime);
            _view.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            _terrain.Render(_view);

            spriteBatch.Begin();
            spriteBatch.DrawString(_debugFont, "MousePos: " + _mouseWorldPos, Vector2.Zero, Color.Red);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
