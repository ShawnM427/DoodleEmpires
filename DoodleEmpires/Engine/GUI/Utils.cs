using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.GUI
{
    public static class Utils
    {
        public static void FixFont(this SpriteFont font)
        {
            font.LineSpacing = (int)(font.MeasureString(" ").Y / 1.5f);
        }

        public static string Wrap(this string s, SpriteFont font, float width)
        {
            string[] words = s.Split(' ');

            StringBuilder sb = new StringBuilder();

            float lineWidth = 0.0f;
            float spaceWidth = font.MeasureString(" ").X;

            foreach (string word in words)
            {
                string checker = word.Replace("\n","");
                Vector2 size = font.MeasureString(checker);

                if (lineWidth + size.X < width)
                {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    sb.Append("\n" + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }

            return sb.ToString();
        }
    }
}
