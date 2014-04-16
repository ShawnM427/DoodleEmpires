using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DoodleEmpires.Engine.GUI
{
    public class GUIPanel : GameControl
    {
        public GUIPanel(GraphicsDevice graphics, GameControl parent) : base(graphics, parent) { }

        protected override void Invalidate()
        {
        }
    }
}
