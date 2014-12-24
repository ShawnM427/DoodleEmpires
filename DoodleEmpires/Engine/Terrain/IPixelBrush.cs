using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleEmpires.Engine.Terrain
{
    public abstract class IPixelBrush
    {
        private Color _eraseColor = Color.White;
        public Texture2D Texture;

        public virtual int Width
        {
            get;
            protected set;
        }
        public virtual int Height
        {
            get;
            protected set;
        }

        public virtual Vector2 HalfSize
        {
            get { return new Vector2(Width / 2.0f, Height / 2.0f); }
        }

        public virtual Color EraseColor
        {
            get { return _eraseColor; }
            set { _eraseColor = value; }
        }

        public virtual Color this[int x, int y]
        {
            get { return Sample(x, y); }
        }

        public abstract Color Sample(int x, int y);
    }
}
