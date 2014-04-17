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
    public abstract class GUIContainer : IGUI
    {
        protected List<IGUI> _controls;
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
        public override Rectangle Bounds
        {
            get { return base.Bounds; }
            set
            {
                if (_bounds.Width != value.Width || _bounds.Height != value.Height)
                {
                    _cornerVerts[0].Position = new Vector3(0, 0, 0.5f);
                    _cornerVerts[1].Position = new Vector3(value.Width - 1, 0, 0.5f);
                    _cornerVerts[2].Position = new Vector3(value.Width - 1, value.Height - 1, 0.5f);
                    _cornerVerts[3].Position = new Vector3(0, value.Height - 1, 0.5f);
                    _cornerVerts[4].Position = new Vector3(0, 0, 0.5f);
                }

                base.Bounds = value;
            }
        }
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

        public GUIContainer(GraphicsDevice graphics) : this(graphics, null)
        {
        }

        public GUIContainer(GraphicsDevice graphics, GUIContainer parent)
            : base(graphics, parent)
        {
            _controls = new List<IGUI>();
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
                if (control != null)
                    _spriteBatch.Draw(control.Image, control.Bounds, Color.White);
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
        public override bool MousePressed(MouseEventArgs e)
        {
            foreach (IGUI control in _controls)
                if (control.MousePressed(e))
                    return true;

            return false;
        }
    }
}
