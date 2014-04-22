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
    public abstract class AdvancedGame : Game
    {
        /// <summary>
        /// Gets or sets the spritebatch for drawing
        /// </summary>
        protected SpriteBatch SpriteBatch;

        protected event MouseEvent MouseClickedEvent;
        protected event MouseEvent MouseReleasedEvent;
        protected event MouseEvent MouseDownEvent;

        protected bool _mouseDown;

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

            Type t = typeof(OpenTKGameWindow);
            PropertyInfo pInf = t.GetProperty("Window", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            OpenTK.GameWindow _window = (OpenTK.GameWindow)pInf.GetValue((OpenTKGameWindow)Window, new object[0]);

            _window.Mouse.ButtonDown += new EventHandler<OpenTK.Input.MouseButtonEventArgs>(Mouse_ButtonDown);
            _window.Mouse.ButtonUp += new EventHandler<OpenTK.Input.MouseButtonEventArgs>(Mouse_ButtonUp);

            base.Initialize();
        }

        void Mouse_ButtonUp(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            _mouseDown = false;

            MouseReleased(
                new MouseEventArgs(new MouseState(e.X, e.Y, 0,
                e.Button == OpenTK.Input.MouseButton.Left ? ButtonState.Pressed : ButtonState.Released,
                e.Button == OpenTK.Input.MouseButton.Middle ? ButtonState.Pressed : ButtonState.Released,
                e.Button == OpenTK.Input.MouseButton.Right ? ButtonState.Pressed : ButtonState.Released,
                e.Button == OpenTK.Input.MouseButton.Button1 ? ButtonState.Pressed : ButtonState.Released,
                e.Button == OpenTK.Input.MouseButton.Button2 ? ButtonState.Pressed : ButtonState.Released), 0));
        }

        void Mouse_ButtonDown(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            _mouseDown = true;

            MousePressed(
                new MouseEventArgs(new MouseState(e.X, e.Y, 0,
                e.Button == OpenTK.Input.MouseButton.Left ? ButtonState.Pressed : ButtonState.Released,
                e.Button == OpenTK.Input.MouseButton.Middle ? ButtonState.Pressed : ButtonState.Released,
                e.Button == OpenTK.Input.MouseButton.Right ? ButtonState.Pressed : ButtonState.Released,
                e.Button == OpenTK.Input.MouseButton.Button1 ? ButtonState.Pressed : ButtonState.Released,
                e.Button == OpenTK.Input.MouseButton.Button2 ? ButtonState.Pressed : ButtonState.Released), 0));
        }

        /// <summary>
        /// Updates this game
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        protected override void Update(GameTime gameTime)
        {
            if (!IsActive)
                _mouseDown = false;

            if (_mouseDown)
                MouseDown(new MouseEventArgs(Mouse.GetState(), 0));

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
    public class MouseEventArgs : EventArgs
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
            set { _location = value; }
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
