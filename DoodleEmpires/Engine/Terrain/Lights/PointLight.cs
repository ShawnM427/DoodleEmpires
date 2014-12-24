using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleEmpires.Engine.Terrain
{
    public class PointLight : ILight
    {
        public float Range
        {
            get;
            set;
        }

        public PointLight(GraphicsDevice graphics, Vector2 position, float range, Color color) : base(graphics)
        {
            Range = range;
            Position = position;
            Color = color;

            int texSize = (int)Math.Ceiling(range * 2);
            Texture = new Texture2D(_graphics, texSize, texSize);
            TextureOrigin = new Vector2(texSize / 2.0f);

            PopulateTexture();
        }

        protected virtual void PopulateTexture()
        {
            Color[] pixels = new Color[Texture.Width * Texture.Height];

            int yy = 0;
            for (int y = 0; y < Texture.Height; y++)
            {
                yy = y * Texture.Width;
                for (int x = 0; x < Texture.Width; x++)
                {
                    pixels[x + yy] = Color.TransparentBlack;
                    float lerp = 1.0f - (Vector2.Distance(new Vector2(x, y), TextureOrigin) / Range);
                    pixels[x + yy] = Color.Lerp(Color.TransparentBlack, Color, lerp);
                }
            }

            Texture.SetData(pixels);
        }
    }
}
