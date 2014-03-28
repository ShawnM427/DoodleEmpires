using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DoodleEmpires.Engine.Economy;

namespace DoodleEmpires.Engine.Entities
{
    public abstract class Building
    {
        protected Rectangle _bounds;
        VertexPositionColorTexture[] _verts = new VertexPositionColorTexture[4];
        int[] _indices = new int[6];

        public abstract Resources Cost { get; }

        public Building(Rectangle bounds)
        {
            _bounds = bounds;
        }

        protected void RenderThis()
        {
            
        }
    }
}
