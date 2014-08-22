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

        /// <summary>
        /// Called when the mouse is held down
        /// </summary>
        /// <param name="state"></param>
        protected virtual void MouseDown(MouseEventArgs state)
        {

        }
    }

    /// <summary>
    /// Provides a snapshot of mouse values
    /// </summary>
    public struct MouseEventArgs
    {
        /// <summary>
        /// The x-coordinate of the mouse relative to the top-left corner of the window
        /// </summary>
        public int X;
        /// <summary>
        /// The y-coordinate of the mouse relative to the top-left corner of the window
        /// </summary>
        public int Y;
        /// <summary>
        /// The state of the left mouse button relative to it's previous state
        /// </summary>
        public ButtonChangeState LeftButton;
        /// <summary>
        /// The state of the middle mouse button relative to it's previous state
        /// </summary>
        public ButtonChangeState MiddleButton;
        /// <summary>
        /// The state of the right button relative to it's previous state
        /// </summary>
        public ButtonChangeState RightButton;
        /// <summary>
        /// The position of the mouse relative to the top left corner of the window
        /// </summary>
        public Vector2 Position
        {
            get { return new Vector2(X, Y); }
        }

        /// <summary>
        /// Creates a new mouse event argument
        /// </summary>
        /// <param name="x">The x position of the mouse</param>
        /// <param name="y">The y position of the mouse</param>
        /// <param name="left">The change in state of the left mouse button</param>
        /// <param name="middle">The change in state of the middle mouse button</param>
        /// <param name="right">The change in state of the right mouse button</param>
        public MouseEventArgs(int x, int y, ButtonChangeState left, ButtonChangeState middle, ButtonChangeState right)
        {
            X = x;
            Y = y;
            LeftButton = left;
            MiddleButton = middle;
            RightButton = right;
        }
    }

    /// <summary>
    /// Represents how the state of a button has changed since the previos check
    /// </summary>
    public enum ButtonChangeState
    {
        /// <summary>
        /// No change of state has occured
        /// </summary>
        None = 0,
        /// <summary>
        /// The button has been pressed since the last check
        /// </summary>
        Pressed = 1,
        /// <summary>
        /// The button has been released since the last check
        /// </summary>
        Released = 2
    }
}
