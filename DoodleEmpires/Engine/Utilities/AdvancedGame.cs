using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace DoodleEmpires.Engine.Utilities
{
    public abstract class AdvancedGame : Game
    {
        private MouseState _prevMouseState = Mouse.GetState();
        private MouseState _currentMouseState;
        private MouseEventArgs _mouseArgs;

        /// <summary>
        /// Gets or sets the spritebatch for drawing
        /// </summary>
        protected SpriteBatch SpriteBatch;

        protected event MouseEvent MouseClickedEvent;
        protected event MouseEvent MouseReleasedEvent;
        protected event MouseEvent MouseDownEvent;

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
            _currentMouseState = Mouse.GetState();
            _mouseArgs = 
                new MouseEventArgs(_currentMouseState, _currentMouseState.ScrollWheelValue - _prevMouseState.ScrollWheelValue);

            if (_currentMouseState.LeftButton == ButtonState.Pressed & _prevMouseState.LeftButton == ButtonState.Released ||
                _currentMouseState.RightButton == ButtonState.Pressed & _prevMouseState.RightButton == ButtonState.Released)
            {
                MousePressed(_mouseArgs);
                if (MouseClickedEvent != null)
                    MouseClickedEvent(_mouseArgs);
            }

            if (_currentMouseState.LeftButton == ButtonState.Released & _prevMouseState.LeftButton == ButtonState.Pressed ||
                _currentMouseState.RightButton == ButtonState.Released & _prevMouseState.RightButton == ButtonState.Pressed)
            {
                MouseReleased(_mouseArgs);
                if (MouseReleasedEvent != null)
                    MouseReleasedEvent(_mouseArgs);
            }

            if (_currentMouseState.LeftButton == ButtonState.Pressed | _currentMouseState.RightButton == ButtonState.Pressed)
            {
                MouseDown(_mouseArgs);
                if (MouseDownEvent != null)
                    MouseDownEvent(_mouseArgs);
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
        /// Called when the mouse has been pressed
        /// </summary>
        /// <param name="args">The mouse event arguments</param>
        protected virtual void MousePressed(MouseEventArgs args)
        {
        }

        /// <summary>
        /// Called when the mouse has been released
        /// </summary>
        /// <param name="args">The mouse event arguments</param>
        protected virtual void MouseReleased(MouseEventArgs args)
        {
        }

        /// <summary>
        /// Called when a mouse button is down
        /// </summary>
        /// <param name="args">The mouse event arguments</param>
        protected virtual void MouseDown(MouseEventArgs args)
        {
        }
    }

    public delegate void MouseEvent(MouseEventArgs args);

    /// <summary>
    /// Represents the arguments for the mouse
    /// </summary>
    public struct MouseEventArgs
    {
        Vector2 _location;
        ButtonState _leftButton;
        ButtonState _middleButton;
        ButtonState _rightButton;
        int _scrollChange;

        /// <summary>
        /// Gets the location of the mouse
        /// </summary>
        public Vector2 Location
        {
            get { return _location; }
        }
        /// <summary>
        /// Gets the change in the scroll wheel value
        /// </summary>
        public int ScrollChange
        {
            get { return _scrollChange; }
        }
        /// <summary>
        /// Gets the button state for the left button
        /// </summary>
        public ButtonState LeftButton
        {
            get { return _leftButton; }
        }
        /// <summary>
        /// Gets the button state for the middle button
        /// </summary>
        public ButtonState MiddleButton
        {
            get { return _middleButton; }
        }
        /// <summary>
        /// Gets the button state for the right button
        /// </summary>
        public ButtonState RightButton
        {
            get { return _rightButton; }
        }

        /// <summary>
        /// Creates a new mouse event argument
        /// </summary>
        /// <param name="mouseState">The current mouse state</param>
        /// <param name="scrollChange">The change in the scroll wheel value</param>
        public MouseEventArgs(MouseState mouseState, int scrollChange)
        {
            _location = new Vector2(mouseState.X, mouseState.Y);

            _leftButton = mouseState.LeftButton;
            _middleButton = mouseState.MiddleButton;
            _rightButton = mouseState.RightButton;

            _scrollChange = scrollChange;
        }
    }
}
