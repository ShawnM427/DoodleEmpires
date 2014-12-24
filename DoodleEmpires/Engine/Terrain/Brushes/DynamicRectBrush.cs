using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleEmpires.Engine.Terrain.Brushes
{
    public class DynamicRectBrush : IPixelBrush
    {
        Color _innerColor;
        Color _outerColor;
        Rectangle _bounds;

        public Rectangle Bounds
        {
            get { return _bounds; }
            set { _bounds = value; Width = value.Width; Height = value.Height; UpdateTexture(); }
        }

        public DynamicRectBrush(Rectangle bounds) : this(bounds, Color.White, Color.Black) 
        { 
        }

        public DynamicRectBrush(Rectangle bounds, Color innerColor, Color outerColor)
        {
            Width = bounds.Width;
            Height = bounds.Height;

            _innerColor = innerColor;
            _outerColor = outerColor;

            UpdateTexture();
        }

        protected void UpdateTexture()
        {
            if (Width > 0 & Height > 0)
            {
                Color[] colors = new Color[Width * Height];
                for (int x = 0; x < Width; x++)
                {
                    colors[x] = _outerColor;
                    colors[x + Height ^ 2] = _outerColor;
                }
                for (int y = 0; y < Height; y++)
                {
                    colors[0 + y * Height] = _outerColor;
                    colors[Width + y * Height] = _outerColor;
                }
                Texture = new Texture2D(Global.Graphics, Width, Height);
                Texture.SetData(colors);
            }
        }

        public override Color Sample(int x, int y)
        {
            return (x % (Width - 1) == 0 || y % (Height - 1) == 0) ? _outerColor : _innerColor;
        }
    }
}
