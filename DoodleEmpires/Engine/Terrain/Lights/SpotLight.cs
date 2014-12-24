using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleEmpires.Engine.Terrain
{
    public class SpotLight : ILight
    {        
        public float Range
        {
            get;
            set;
        }
        public float ConeAngle
        {
            get;
            set;
        }
        
        public SpotLight(GraphicsDevice graphics, Vector2 position, float range, Color color, Vector2 lookAt, float coneAngle = 45)
            : base(graphics)
        {
            if (coneAngle >= 180 || coneAngle <= 0)
                throw new ArgumentOutOfRangeException("coneAngle", "Must be betweeen 0 and 180, exlusive!");

            Range = range;
            Position = position;
            Color = color;
            ConeAngle = coneAngle;

            float halfAngle = MathHelper.ToRadians(coneAngle / 2);
            float oppAngle = MathHelper.ToRadians(90 - coneAngle / 2);

            int texWidth = (int)Math.Ceiling(range);
            int texHeight = 2 * (int)Math.Ceiling((texWidth * Math.Sin(halfAngle)) / Math.Sin(oppAngle));
            Texture = new Texture2D(_graphics, texWidth, texHeight);
            TextureOrigin = new Vector2(texWidth / 2.0f, texHeight / 2.0f);

            Rotation = (float)Math.Atan2(lookAt.Y - position.Y, lookAt.X - position.X);
            TextureOrigin = new Vector2(0, Texture.Height / 2);

            PopulateTexture();
        }

        protected virtual void PopulateTexture()
        {
            Color[] pixels = new Color[Texture.Width * Texture.Height];

            float halfHeight = Texture.Height / 2.0f;
            int yy = 0;
            float offSet = 0;

            for (int y = 0; y < Texture.Height; y++)
            {
                yy = y * Texture.Width;
                for (int x = 0; x < Texture.Width; x++)
                {
                    offSet = (x / Range) * halfHeight;
                    pixels[x + yy] = Color.TransparentBlack;

                    if (y < halfHeight + offSet & y > halfHeight - offSet)
                    {
                        float lerp = 1.0f - (x / Range);
                        pixels[x + yy] = Color.Lerp(Color.TransparentBlack, Color, lerp);
                    }
                }
            }

            Texture.SetData(pixels);
        }
    }
}
