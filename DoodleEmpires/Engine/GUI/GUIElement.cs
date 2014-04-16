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
        
        public GUIElement(GraphicsDevice graphics, GUIContainer parent) : base(graphics, parent)
        {
            _backColor = Color.Transparent;
        }
    }
}
