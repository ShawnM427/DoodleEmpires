using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using DoodleEmpires.Engine.Utilities;

namespace DoodleEmpires.Engine.GUI
{
    /// <summary>
    /// Represents a control used for games
    /// </summary>
    public abstract class GUIElement : IGUI
    {
        protected GraphicsDevice _graphics;
        protected SpriteBatch _spriteBatch;
        protected BasicEffect _effect;
        protected RenderTarget2D _renderTarget;
        protected Rectangle _bounds;
        protected Rectangle _screenBounds;
        protected Color _colorMultiplier = Color.White;
        protected Color _backColor = Color.Transparent;
        protected Color _foreColor = Color.Black;
        protected GUIContainer _parent;

        /// <summary>
        /// The bounds relative to the parent container
        /// </summary>
        public virtual Rectangle Bounds
        {
            get { return _bounds; }
            set
            {
                if (_bounds.Width != value.Width || _bounds.Height != value.Height)
                {
                    _renderTarget = new RenderTarget2D(_graphics, _bounds.Width, _bounds.Height);
                    _effect.Projection = Matrix.CreateOrthographicOffCenter(0, value.Width, value.Height, 0,
                        1.0f, 1000.0f);
                    _effect.CurrentTechnique.Passes[0].Apply();
                    Invalidating = true;
                }

                _bounds = value;
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

        /// <summary>
        /// Gets or sets the back color for this control
        /// </summary>
        public virtual Color BackColor
        {
            get { return _backColor; }
            set
            {
                _backColor = value;
                Invalidating = true;
            }
        }
        /// <summary>
        /// Gets or sets the forecolor for this control
        /// </summary>
        public virtual Color ForeColor
        {
            get { return _foreColor; }
            set
            {
                _foreColor = value;
                Invalidating = true;
            }
        }

        public GUIElement(GraphicsDevice graphics)
            : this(graphics, null)
        {
        }

        public GUIElement(GraphicsDevice graphics, GUIContainer parent)
        {
            _graphics = graphics;
            _spriteBatch = new SpriteBatch(graphics);
            _parent = parent;

            _effect = new BasicEffect(_graphics);
            _effect.VertexColorEnabled = true;
            _effect.Projection = 
                Matrix.CreateOrthographicOffCenter(0, 800, 480, 0, 1.0f, 1000.0f);
            _effect.World = Matrix.Identity;

        }

        /// <summary>
        /// Begins the invalidation of this control
        /// </summary>
        protected override void BeginInvalidate()
        {
            _renderTarget = new RenderTarget2D(_graphics, _screenBounds.Width, _screenBounds.Height);
            _graphics.SetRenderTarget(_renderTarget);
            _graphics.Clear(Color.Transparent);
        }
        
        /// <summary>
        /// Ends the invalidation of the control
        /// </summary>
        protected override void EndInvalidate()
        {
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
        /// Draws this control to the screen
        /// </summary>
        public override void Draw()
        {
            if (Image != null)
            {
                _spriteBatch.Begin();
                _spriteBatch.Draw(Image, ScreenBounds, _colorMultiplier);
                _spriteBatch.End();
            }
        }

        /// <summary>
        /// Called when this control is clicked
        /// </summary>
        /// <param name="e">The mouse event arguments</param>
        protected virtual void MousePressed(MouseEventArgs e) { }
    }
}
