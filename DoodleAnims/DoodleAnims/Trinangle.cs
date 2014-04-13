using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace DoodleAnims
{
    /// <summary>
    /// A class for drawing triangles
    /// </summary>
    public abstract class Triangle
    {
        /// <summary>
        /// Represents which way the triangle is pointing
        /// </summary>
        public enum Orientation
        {
            /// <summary>
            /// The triangle points towards the top
            /// </summary>
            Top,
            /// <summary>
            /// The triangle points towards the right
            /// </summary>
            Right,
            /// <summary>
            /// The triangle points towards the bottom
            /// </summary>
            Bottom,
            /// <summary>
            /// The triangle points towards the left
            /// </summary>
            Left
        }

        /// <summary>
        /// Creates a new graphics path for a triangle
        /// </summary>
        /// <param name="bounds">The bounds of the triangle</param>
        /// <param name="orientation">The orientation of the triangle in the rectangle</param>
        /// <returns>A graphicsPath ready for drawing</returns>
        public static GraphicsPath Create(RectangleF bounds, Orientation orientation)
        {
            PointF top, left, right;

            switch (orientation)
            {
                case Orientation.Top:
                    top = new PointF(bounds.X + bounds.Width / 2, bounds.Y);
                    left = new PointF(bounds.X, bounds.Y + bounds.Height);
                    right = new PointF(bounds.X + bounds.Width, bounds.Y + bounds.Height);
                    break;
                case Orientation.Right:
                    top = new PointF(bounds.X + bounds.Width, bounds.Y + bounds.Height / 2);
                    left = new PointF(bounds.X, bounds.Y);
                    right = new PointF(bounds.X, bounds.Y + bounds.Height);
                    break;
                case Orientation.Bottom:
                    top = new PointF(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height);
                    left = new PointF(bounds.X, bounds.Y);
                    right = new PointF(bounds.X + bounds.Width, bounds.Y);
                    break;
                default:
                    top = new PointF(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height);
                    left = new PointF(bounds.X, bounds.Y);
                    right = new PointF(bounds.X + bounds.Width, bounds.Y);
                    break;
            }

            GraphicsPath path = new GraphicsPath();

            path.AddPolygon(new PointF[]{top, left, right});
            return path;
        }
    }
}
