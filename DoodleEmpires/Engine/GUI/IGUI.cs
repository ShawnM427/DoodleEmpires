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
    public abstract class IGUI
    {
        private bool _invalidating = true;

        protected GraphicsDevice _graphics;
        protected SpriteBatch _spriteBatch;
        protected BasicEffect _effect;
        protected RenderTarget2D _renderTarget;
        protected Rectangle _bounds;
        protected Rectangle _screenBounds;
        protected Color _colorMultiplier = Color.White;
        protected Color _backColor = Color.LightGray;
        protected GUIContainer _parent;
        protected bool _visible = true;
        protected bool _enabled = true;

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
                if (_bounds.Width != value.Width || _bounds.Height != value.Height)
                {

                    _effect.Projection = Matrix.CreateOrthographicOffCenter(0, value.Width, value.Height, 0,
                        1.0f, 1000.0f);
                    _effect.CurrentTechnique.Passes[0].Apply();

                    _cornerVerts[0].Position = new Vector3(0, 0, 0.5f);
                    _cornerVerts[1].Position = new Vector3(value.Width - 1, 0, 0.5f);
                    _cornerVerts[2].Position = new Vector3(value.Width - 1, value.Height - 1, 0.5f);
                    _cornerVerts[3].Position = new Vector3(0, value.Height - 1, 0.5f);
                    _cornerVerts[4].Position = new Vector3(0, 0, 0.5f);

                    Invalidating = true;
                }

                _bounds = value;
                Resized();
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

        public int X
        {
            get { return _bounds.X; }
            set
            {
                _bounds.X = value;
                Bounds = _bounds;
            }
        }
        public int Y
        {
            get { return _bounds.Y; }
            set
            {
                _bounds.Y = value;
                Bounds = _bounds;
            }
        }
        public int Width
        {
            get { return _bounds.Width; }
            set
            {
                _bounds.Width = value;
                Bounds = _bounds;
            }
        }
        public int Height
        {
            get { return _bounds.Height; }
            set
            {
                _bounds.Height = value;
                Bounds = _bounds;
            }
        }


        public IGUI(GraphicsDevice graphics, GUIContainer parent)
        {
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
        /// Called when this control is clicked, returns true if the mouse input was handled
        /// </summary>
        /// <param name="e">The mouse event arguments</param>
        /// <returns>True if the input was handled</returns>
        public virtual bool MousePressed(MouseEventArgs e) { return false; }

        /// <summary>
        /// Called when this component has been resized
        /// </summary>
        protected virtual void Resized()
        {

        }
    }
}
