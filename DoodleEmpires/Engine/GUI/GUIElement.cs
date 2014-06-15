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
        /// <summary>
        /// The foreground color for graphical elements, such as text
        /// </summary>
        protected Color _foreColor = Color.Black;
        
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
        
        /// <summary>
        /// Creates a new GUI element
        /// </summary>
        /// <param name="graphics">The graphics device to bind to</param>
        /// <param name="parent">The parent container</param>
        protected GUIElement(GraphicsDevice graphics, GUIContainer parent) : base(graphics, parent)
        {
            _backColor = Color.Transparent;
        }
    }
}
