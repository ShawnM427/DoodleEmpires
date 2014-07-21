using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Reflection;

namespace DoodleEmpires.Engine.Utilities
{
    #pragma warning disable 169, 67

    /// <summary>
    /// Represents a slighty more advanced base game class
    /// </summary>
    public abstract class AdvancedGame : Game
    {
        /// <summary>
        /// Gets or sets the spritebatch for drawing
        /// </summary>
        protected SpriteBatch SpriteBatch;
        
        private MouseState _mouseState;
        private MouseState _prevMouseState;

        /// <summary>
        /// Creates a new advanced game
        /// </summary>
        public AdvancedGame()
            : base()
        {
            IsMouseVisible = true;
        }

        /// <summary>
        /// Initializes this game
        /// </summary>
        protected override void Initialize()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            base.Initialize();
        }

        /// <summary>
        /// Updates this game
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        protected override void Update(GameTime gameTime)
        {
            _mouseState = Mouse.GetState();

            if (_mouseState.LeftButton != _prevMouseState.LeftButton ||
                _mouseState.MiddleButton != _prevMouseState.MiddleButton ||
                _mouseState.RightButton != _prevMouseState.RightButton)
                MouseEvent(new MouseEventArgs(_mouseState.X, _mouseState.Y, 
                    (_mouseState.LeftButton == ButtonState.Released && _prevMouseState.LeftButton == ButtonState.Pressed) ?
                    ButtonChangeState.Released : 
                    (_mouseState.LeftButton == ButtonState.Pressed && _prevMouseState.LeftButton == ButtonState.Released) ?
                    ButtonChangeState.Pressed : ButtonChangeState.None,

                    (_mouseState.MiddleButton == ButtonState.Released && _prevMouseState.MiddleButton == ButtonState.Pressed) ?
                    ButtonChangeState.Released : 
                    (_mouseState.MiddleButton == ButtonState.Pressed && _prevMouseState.MiddleButton == ButtonState.Released) ?
                    ButtonChangeState.Pressed : ButtonChangeState.None,

                    (_mouseState.RightButton == ButtonState.Released && _prevMouseState.RightButton == ButtonState.Pressed) ?
                    ButtonChangeState.Released : 
                    (_mouseState.RightButton == ButtonState.Pressed && _prevMouseState.RightButton == ButtonState.Released) ?
                    ButtonChangeState.Pressed : ButtonChangeState.None));

            if (_mouseState.LeftButton == ButtonState.Pressed && _prevMouseState.LeftButton == ButtonState.Pressed ||
                _mouseState.RightButton == ButtonState.Pressed && _prevMouseState.RightButton == ButtonState.Pressed ||
                _prevMouseState.MiddleButton == ButtonState.Pressed && _prevMouseState.MiddleButton == ButtonState.Pressed)
            {
                MouseDown(new MouseEventArgs(_mouseState.X, _mouseState.Y,
                    (_mouseState.LeftButton == ButtonState.Released && _prevMouseState.LeftButton == ButtonState.Pressed) ?
                    ButtonChangeState.Released :
                    (_mouseState.LeftButton == ButtonState.Pressed) ?
                    ButtonChangeState.Pressed : ButtonChangeState.None,

                    (_mouseState.MiddleButton == ButtonState.Released && _prevMouseState.MiddleButton == ButtonState.Pressed) ?
                    ButtonChangeState.Released :
                    (_mouseState.MiddleButton == ButtonState.Pressed) ?
                    ButtonChangeState.Pressed : ButtonChangeState.None,

                    (_mouseState.RightButton == ButtonState.Released && _prevMouseState.RightButton == ButtonState.Pressed) ?
                    ButtonChangeState.Released :
                    (_mouseState.RightButton == ButtonState.Pressed) ?
                    ButtonChangeState.Pressed : ButtonChangeState.None));
            }

           _prevMouseState = Mouse.GetState();

            base.Update(gameTime);
        }
        
        /// <summary>
        /// Draws the game
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        protected override void Draw(GameTime gameTime)
        {
            FPSManager.OnDraw(gameTime);
            base.Draw(gameTime);
        }

        /// <summary>
        /// Called when the state of the mouse has changed
        /// </summary>
        /// <param name="state"></param>
        protected virtual void MouseEvent(MouseEventArgs state)
        {
        }

        protected virtual void MouseDown(MouseEventArgs state)
        {

        }
    }

    public struct MouseEventArgs
    {
        public int X;
        public int Y;
        public ButtonChangeState LeftButton;
        public ButtonChangeState MiddleButton;
        public ButtonChangeState RightButton;
        public Vector2 Position
        {
            get { return new Vector2(X, Y); }
        }

        public MouseEventArgs(int x, int y, ButtonChangeState left, ButtonChangeState middle, ButtonChangeState right)
        {
            X = x;
            Y = y;
            LeftButton = left;
            MiddleButton = middle;
            RightButton = right;
        }
    }

    public enum ButtonChangeState
    {
        None = 0,
        Pressed = 1,
        Released = 2
    }
}
