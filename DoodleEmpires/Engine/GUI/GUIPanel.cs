using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoodleEmpires.Engine.GUI
{
    /// <summary>
    /// A basic GUI panel
    /// </summary>
    public class GUIPanel : GUIContainer
    {
        /// <summary>
        /// Creates a new GUI panel
        /// </summary>
        /// <param name="graphics">The graphics device to bind to</param>
        /// <param name="parent">The parent container</param>
        public GUIPanel(GraphicsDevice graphics, GUIContainer parent) : base(graphics, parent) { }
    }
}
