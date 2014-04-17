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

    public static class GraphicsExtensions
    {
        public static void DrawRect(this GraphicsDevice g, Rectangle rect, Color color)
        {
            VertexPositionColor[] verts = new VertexPositionColor[]{
                new VertexPositionColor(new Vector3(rect.X, rect.Y, 0.5F), color),
                new VertexPositionColor(new Vector3(rect.Right - 1, rect.Y, 0.5F), color),
                new VertexPositionColor(new Vector3(rect.Right - 1, rect.Bottom - 1, 0.5F), color),
                new VertexPositionColor(new Vector3(rect.X, rect.Bottom - 1, 0.5F), color),
                new VertexPositionColor(new Vector3(rect.X, rect.Y, 0.5F), color)
            };

            g.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, verts, 0, 4);
        }

        public static void DrawRect(this GraphicsDevice g, float minX, float minY, float maxX, float maxY, Color color)
        {
            VertexPositionColor[] verts = new VertexPositionColor[]{
                new VertexPositionColor(new Vector3(minX, minY, 0.5F), color),
                new VertexPositionColor(new Vector3(maxX - 1, minY, 0.5F), color),
                new VertexPositionColor(new Vector3(maxX - 1, maxY - 1, 0.5F), color),
                new VertexPositionColor(new Vector3(minX, maxY - 1, 0.5F), color),
                new VertexPositionColor(new Vector3(minX, minY, 0.5F), color)
            };

            g.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, verts, 0, 4);
        }

        public static void DrawLine(this GraphicsDevice g, Vector2 point1, Vector2 point2, Color color)
        {
            VertexPositionColor[] verts = new VertexPositionColor[]{
                new VertexPositionColor(new Vector3(point1, 0.5F), color),
                new VertexPositionColor(new Vector3(point2, 0.5F), color)
            };

            g.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, verts, 0, 1);
        }

        public static void DrawLine(this GraphicsDevice g, float x, float y, float x2, float y2, Color color)
        {
            g.DrawLine(new Vector2(x, y), new Vector2(x2, y2), color);
        }
    }
}
