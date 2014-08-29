using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace MonoUI
{
    /// <summary>
    /// The base class for all GUI elements and containers
    /// </summary>
    public abstract class IGUI : IDisposable
    {
        private bool _invalidating = true;

        /// <summary>
        /// Gets or sets whether this object is in focus or not
        /// </summary>
        protected bool _isFocused = false;
        /// <summary>
        /// The graphics device used to draw within this component
        /// </summary>
        protected GraphicsDevice _graphics;
        /// <summary>
        /// The spritebatch used to draw within this component
        /// </summary>
        protected SpriteBatch _spriteBatch;
        /// <summary>
        /// The effect used to draw within this component
        /// </summary>
        protected BasicEffect _effect;
        /// <summary>
        /// The internal render target to render to
        /// </summary>
        protected RenderTarget2D _renderTarget;
        /// <summary>
        /// This component's bounds
        /// </summary>
        protected Rectangle _bounds;
        /// <summary>
        /// This component's bounds on the screen
        /// </summary>
        protected Rectangle _screenBounds;
        /// <summary>
        /// The Color multiplier to used when drawing this component
        /// </summary>
        protected Color _colorMultiplier = Color.White;
        /// <summary>
        /// The background color of this component
        /// </summary>
        protected Color _backColor = Color.LightGray;
        /// <summary>
        /// This components parent container
        /// </summary>
        protected GUIContainer _parent;
        /// <summary>
        /// True if this component is visible
        /// </summary>
        protected bool _visible = true;
        /// <summary>
        /// True if this component is enabled
        /// </summary>
        protected bool _enabled = true;

        /// <summary>
        /// The corner vertices of this GUI peice
        /// </summary>
        protected VertexPositionColor[] _cornerVerts = new VertexPositionColor[5]
        {
            new VertexPositionColor(new Vector3(0, 0, 0.5f), Color.Black),
            new VertexPositionColor(new Vector3( 32, 0, 0.5f), Color.Black),
            new VertexPositionColor(new Vector3(32, 32, 0.5f), Color.Black),
            new VertexPositionColor(new Vector3(0, 32, 0.5f), Color.Black),
            new VertexPositionColor(new Vector3(0, 0, 0.5f), Color.Black)
        };

        /// <summary>
        /// The bounds relative to the parent container
        /// </summary>
        public virtual Rectangle Bounds
        {
            get { return _bounds; }
            set
            {
                _effect.Projection = Matrix.CreateOrthographicOffCenter(0, value.Width, value.Height, 0, 1.0f, 1000.0f);

                if (_bounds.Width != value.Width || _bounds.Height != value.Height)
                {
                    _cornerVerts[0].Position = new Vector3(0, 0, 0.5f);
                    _cornerVerts[1].Position = new Vector3(value.Width - 1, 0, 0.5f);
                    _cornerVerts[2].Position = new Vector3(value.Width - 1, value.Height - 1, 0.5f);
                    _cornerVerts[3].Position = new Vector3(0, value.Height - 1, 0.5f);
                    _cornerVerts[4].Position = new Vector3(0, 0, 0.5f);

                    Invalidating = true;
                }

                _bounds = value;
                _screenBounds = _bounds;

                if (_renderTarget != null)
                {
                    _renderTarget.Dispose();
                    _renderTarget = new RenderTarget2D(_graphics, _screenBounds.Width, _screenBounds.Height);
                }
                
                if (_parent != null)
                {
                    _screenBounds.X += _parent.ScreenBounds.X;
                    _screenBounds.Y += _parent.ScreenBounds.Y;
                }

                Resized();
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
            get { return _invalidating; }
            set
            {
                _invalidating = value;

                if (_parent != null)
                    _parent.Invalidating = true;
            }
        }
        /// <summary>
        /// Gets or sets whether this control is visible
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                _enabled = !_visible ? false : _enabled;
                Invalidating = true;
            }
        }
        /// <summary>
        /// Gets or sets whether this control is visible
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                Invalidating = true;
            }
        }
        /// <summary>
        /// Gets or sets whether this control has focus
        /// </summary>
        public bool Focused
        {
            get { return _isFocused; }
            set
            {
                _isFocused = value;
                Invalidating = true;
            }
        }

        /// <summary>
        /// Gets or sets the left bound of the client rectangle
        /// </summary>
        public int X
        {
            get { return _bounds.X; }
            set
            {
                _bounds.X = value;
                Bounds = _bounds;
            }
        }
        /// <summary>
        /// Gets or sets the top bound of the client rectangle
        /// </summary>
        public int Y
        {
            get { return _bounds.Y; }
            set
            {
                _bounds.Y = value;
                Bounds = _bounds;
            }
        }
        /// <summary>
        /// Gets or sets the width of the client rectangle
        /// </summary>
        public int Width
        {
            get { return _bounds.Width; }
            set
            {
                _bounds.Width = value;
                Bounds = _bounds;
            }
        }
        /// <summary>
        /// Gets or sets the height of the client rectangle
        /// </summary>
        public int Height
        {
            get { return _bounds.Height; }
            set
            {
                _bounds.Height = value;
                Bounds = _bounds;
            }
        }
        
        /// <summary>
        /// Creates a new GUI component
        /// </summary>
        /// <param name="graphics">The GraphicsDevice to bind to</param>
        /// <param name="parent">The parent container</param>
        protected IGUI(GraphicsDevice graphics, GUIContainer parent)
        {
            Invalidating = true;
            _graphics = graphics;
            _spriteBatch = new SpriteBatch(graphics);
            _parent = parent;

            _effect = new BasicEffect(_graphics);
            _effect.VertexColorEnabled = true;
            _effect.Projection =
                Matrix.CreateOrthographicOffCenter(0, 800, 480, 0, 1.0f, 1000.0f);
            _effect.World = Matrix.Identity;

            if (_parent != null)
                _parent.AddComponent(this);
        }

        /// <summary>
        /// Begins the invalidation of this control
        /// </summary>
        protected virtual bool BeginInvalidate()
        {
            if (_screenBounds.Width > 0 && _screenBounds.Height > 0 && _visible)
            {
                if (_renderTarget == null)
                    _renderTarget = new RenderTarget2D(_graphics, _screenBounds.Width, _screenBounds.Height);

                _graphics.SetRenderTarget(_renderTarget);
                _graphics.Clear(_backColor);

                _effect.CurrentTechnique.Passes[0].Apply();
                _spriteBatch.Begin();
                return true;
            }
            else
            {
                Invalidating = false;
                return false;
            }
        }

        /// <summary>
        /// Renders this control
        /// </summary>
        protected virtual void Invalidate() { }

        /// <summary>
        /// Ends the invalidation of the control
        /// </summary>
        protected virtual void EndInvalidate()
        {
            _spriteBatch.End();
            
            if (_parent != null)
                _parent.Invalidating = true;
        }

        /// <summary>
        /// Draws this control to the screen
        /// </summary>
        public virtual void Draw()
        {
            if (Image != null)
            {
                _spriteBatch.Begin();
                _spriteBatch.Draw(Image, ScreenBounds, _colorMultiplier);
                _spriteBatch.End();
            }
        }

        /// <summary>
        /// Updates this control, MUST BE CALLED IN OVERRIDEN CLASSES
        /// </summary>
        public virtual void Update()
        {
            if (Invalidating && BeginInvalidate())
            {
                Invalidate();
                EndInvalidate();

                Invalidating = false;
            }
        }
        
        /// <summary>
        /// Called when this control is clicked with the mouse
        /// </summary>
        /// <param name="e">The mouse event arguments</param>
        public virtual void MousePressed(MouseEventArgs e) 
        {
            Focused = true;
        }

        /// <summary>
        /// Called when the left mouse button is held down over this control
        /// </summary>
        /// <param name="e">The mouse event arguments</param>
        public virtual void MouseDown(MouseEventArgs e)
        {
        }

        /// <summary>
        /// Called when this component has been resized
        /// </summary>
        protected virtual void Resized()
        {
            
        }

        /// <summary>
        /// Disposes of this object and free's it's resources
        /// </summary>
        public void Dispose()
        {
            _renderTarget.Dispose();
            _spriteBatch.Dispose();
            _effect.Dispose();
        }
    }
}
