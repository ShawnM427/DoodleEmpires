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
    /// Represents a control used for games
    /// </summary>
    public abstract class GUIContainer : IGUI
    {
        static readonly Color _disabledColor = Color.White * 0.4f;
        static readonly Color _enabledColor = Color.White;

        static Texture2D _pixelTex;

        /// <summary>
        /// A list of all controls that this container holds
        /// </summary>
        protected List<IGUI> _controls;
        /// <summary>
        /// The spacing to the left/right/top/bottom to shift elements
        /// </summary>
        protected int _margin = 5;

        /// <summary>
        /// Gets or sets the color of the border
        /// </summary>
        public Color BorderColor
        {
            get { return _cornerVerts[0].Color; }
            set
            {
                _cornerVerts[0].Color = _cornerVerts[1].Color = _cornerVerts[2].Color =
                    _cornerVerts[3].Color = _cornerVerts[4].Color = value;
            }
        }
        /// <summary>
        /// Gets or sets the background color for this control
        /// </summary>
        public Color BackColor
        {
            get { return _backColor; }
            set { _backColor = value; }
        }
        /// <summary>
        /// Gets or sets the margin
        /// </summary>
        public int Margin
        {
            get { return _margin; }
            set { _margin = value; }
        }
        /// <summary>
        /// Gets an array containing all the controls in this container
        /// </summary>
        public IGUI[] Controls
        {
            get { return _controls.ToArray(); }
        }

        /// <summary>
        /// Creates a new GUI container
        /// </summary>
        /// <param name="graphics">The graphics device to bind to</param>
        public GUIContainer(GraphicsDevice graphics) : this(graphics, null)
        {
        }

        /// <summary>
        /// Creates a new GUI container
        /// </summary>
        /// <param name="graphics">The graphics device to bind to</param>
        /// <param name="parent">The parent container</param>
        public GUIContainer(GraphicsDevice graphics, GUIContainer parent)
            : base(graphics, parent)
        {
            _controls = new List<IGUI>();

            if (_pixelTex == null)
            {
                _pixelTex = new Texture2D(graphics, 1, 1);
                _pixelTex.SetData<Color>(new Color[] { Color.White });
            }
        }

        /// <summary>
        /// Adds a new component to this GUI container
        /// </summary>
        /// <param name="component">The component to add</param>
        public void AddComponent(IGUI component)
        {
            _controls.Add(component);
        }

        /// <summary>
        /// Begins the invalidation of this control
        /// </summary>
        protected override bool BeginInvalidate()
        {
            if (base.BeginInvalidate())
            {
                _effect.CurrentTechnique.Passes[0].Apply();
                _graphics.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, _cornerVerts, 0, 4);

                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Ends the invalidation of the control
        /// </summary>
        protected override void EndInvalidate()
        {
            foreach (IGUI control in _controls)
            {
                if (control != null && control.Visible && control.Image != null)
                {
                    _spriteBatch.Draw(control.Image, control.Bounds, Color.White);

                    if (!control.Enabled)
                    {
                        _spriteBatch.Draw(_pixelTex, control.Bounds, _disabledColor);
                    }
                }
            }

            base.EndInvalidate();
        }

        /// <summary>
        /// Updates this control, MUST BE CALLED IN OVERRIDEN CLASSES
        /// </summary>
        public override void Update()
        {
            foreach (IGUI control in _controls)
                control.Update();

            base.Update();
        }

        /// <summary>
        /// Called when this control is clicked, returns true if the mouse input was handled
        /// </summary>
        /// <param name="e">The mouse event arguments</param>
        /// <returns>True if the input was handled</returns>
        public override void MousePressed(MouseEventArgs e)
        {
            base.MousePressed(e);

            if (_screenBounds.Contains(e.Position))
            {
                foreach (IGUI control in _controls)
                    if (control.Enabled)
                        if (control.ScreenBounds.Contains(e.Position))
                            control.MousePressed(e);
                        else
                            control.Focused = false;
            }
        }
    }
}
