using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DoodleEmpires.Engine.Utilities;
using Microsoft.Xna.Framework.Input;

namespace DoodleEmpires.Engine.GUI
{
    /// <summary>
    /// Represents a control used for games
    /// </summary>
    public abstract class GameControl
    {
        protected GraphicsDevice _graphics;
        protected SpriteBatch _spriteBatch;
        protected RenderTarget2D _renderTarget;
        protected Rectangle _bounds;
        protected Rectangle _screenBounds;
        protected Color _backColor = Color.LightGray;
        protected List<GameControl> _controls;
        protected Color _colorMultiplier = Color.White;
        protected GameControl _parent;

        /// <summary>
        /// The bounds relative to the parent container
        /// </summary>
        public virtual Rectangle Bounds
        {
            get { return _bounds; }
            set
            {
                _bounds = value;
                _renderTarget = new RenderTarget2D(_graphics, _bounds.Width, _bounds.Height);
                _screenBounds = _bounds;

                if (_parent != null)
                    _screenBounds.Offset(_parent.Bounds.X, _parent.Bounds.Y);
            }
        }
        /// <summary>
        /// The bounds relative to the screen
        /// </summary>
        public virtual Rectangle ScreenBounds
        {
            get { return _screenBounds; }
        }
        /// <summary>
        /// Gets the image representation of this control
        /// </summary>
        public Texture2D Image
        {
            get { return _renderTarget; }
        }
        /// <summary>
        /// Gets or sets whether this control should invalidate on the next update
        /// </summary>
        public bool Invalidating
        {
            get;
            set;
        }

        public GameControl(GraphicsDevice graphics)
        {
            _graphics = graphics;
            _spriteBatch = new SpriteBatch(graphics);
            _controls = new List<GameControl>();
        }

        public GameControl(GraphicsDevice graphics, GameControl parent)
        {
            _graphics = graphics;
            _spriteBatch = new SpriteBatch(graphics);
            _controls = new List<GameControl>();
            _parent = parent;
        }

        /// <summary>
        /// Begins the invalidation of this control
        /// </summary>
        private void BeginInvalidate()
        {
            _graphics.SetRenderTarget(_renderTarget);
            _graphics.Clear(_backColor);
        }

        /// <summary>
        /// Renders this control
        /// </summary>
        protected abstract void Invalidate();

        /// <summary>
        /// Ends the invalidation of the control
        /// </summary>
        private void EndInvalidate()
        {
            foreach (GameControl control in _controls)
            {
                _spriteBatch.Draw(control.Image, control.Bounds, _colorMultiplier);
            }

            _graphics.SetRenderTarget(null);
        }

        /// <summary>
        /// Updates this control, MUST BE CALLED IN OVERRIDEN CLASSES
        /// </summary>
        public virtual void Update()
        {
            if (Invalidating)
            {
                BeginInvalidate();
                Invalidate();
                EndInvalidate();

                Invalidating = false;
            }

            MouseState mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed || mouseState.RightButton == ButtonState.Pressed &&
                _screenBounds.Contains(mouseState.Position))
            {
                MousePressed(new MouseEventArgs(mouseState, 0));
            }
        }

        /// <summary>
        /// Called when this control is clicked
        /// </summary>
        /// <param name="e">The mouse event arguments</param>
        protected virtual void MousePressed(MouseEventArgs e) { }
    }
}
