using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace WinFormsTools
{
    /// <summary>
    /// Used to draw rounded rectangles
    /// </summary>
    public abstract class RoundedRectangle
    {
        /// <summary>
        /// The represents corners to round off
        /// </summary>
        public enum RectangleCorners
        {
            /// <summary>
            /// Round no corners
            /// </summary>
            None = 0, 
            /// <summary>
            /// Round the top-left corner
            /// </summary>
            TopLeft = 1,
            /// <summary>
            /// Round the top-right corner
            /// </summary>
            TopRight = 2,
            /// <summary>
            /// Round the bottom-left corner
            /// </summary>
            BottomLeft = 4,
            /// <summary>
            /// Round the bottom-right corner
            /// </summary>
            BottomRight = 8,
            /// <summary>
            /// Round off all corners
            /// </summary>
            All = TopLeft | TopRight | BottomLeft | BottomRight,
            /// <summary>
            /// Round off all corners on the left side
            /// </summary>
            Left = TopLeft | BottomLeft,
            /// <summary>
            /// Round off all corners on the right side
            /// </summary>
            Right = TopRight | BottomRight,
            /// <summary>
            /// Round off all corners on the top
            /// </summary>
            Top = TopLeft | TopRight,
            /// <summary>
            /// Round off all corners on the bottom
            /// </summary>
            Bottom = BottomLeft | BottomRight
        }

        /// <summary>
        /// Creates a new graphics path for a rounded rectangle
        /// </summary>
        /// <param name="x">The top-left x coordinate</param>
        /// <param name="y">The top-left y coordinate</param>
        /// <param name="width">The width of the rectangle</param>
        /// <param name="height">The height of the rectangle</param>
        /// <param name="radius">The radius of the corners</param>
        /// <param name="corners">The corners to be rounded</param>
        /// <returns>A graphics path ready for rendering</returns>
        public static GraphicsPath Create(float x, float y, float width, float height,
                                          float radius, RectangleCorners corners)
        {
            width = width - 1;
            height = height - 1;

            float xw = x + width;
            float yh = y + height;
            float xwr = xw - radius;
            float yhr = yh - radius;
            float xr = x + radius;
            float yr = y + radius;
            float r2 = radius * 2;
            float xwr2 = xw - r2;
            float yhr2 = yh - r2;

            GraphicsPath p = new GraphicsPath();
            p.StartFigure();

            //Top Left Corner
            if ((RectangleCorners.TopLeft & corners) == RectangleCorners.TopLeft)
            {
                p.AddArc(x, y, r2, r2, 180, 90);
            }
            else
            {
                p.AddLine(x, yr, x, y);
                p.AddLine(x, y, xr, y);
            }

            //Top Edge
            p.AddLine(xr, y, xwr, y);

            //Top Right Corner
            if ((RectangleCorners.TopRight & corners) == RectangleCorners.TopRight)
            {
                p.AddArc(xwr2, y, r2, r2, 270, 90);
            }
            else
            {
                p.AddLine(xwr, y, xw, y);
                p.AddLine(xw, y, xw, yr);
            }

            //Right Edge
            p.AddLine(xw, yr, xw, yhr);

            //Bottom Right Corner
            if ((RectangleCorners.BottomRight & corners) == RectangleCorners.BottomRight)
            {
                p.AddArc(xwr2, yhr2, r2, r2, 0, 90);
            }
            else
            {
                p.AddLine(xw, yhr, xw, yh);
                p.AddLine(xw, yh, xwr, yh);
            }

            //Bottom Edge
            p.AddLine(xwr, yh, xr, yh);

            //Bottom Left Corner
            if ((RectangleCorners.BottomLeft & corners) == RectangleCorners.BottomLeft)
            {
                p.AddArc(x, yhr2, r2, r2, 90, 90);
            }
            else
            {
                p.AddLine(xr, yh, x, yh);
                p.AddLine(x, yh, x, yhr);
            }

            //Left Edge
            p.AddLine(x, yhr, x, yr);

            p.CloseFigure();
            return p;
        }

        /// <summary>
        /// Creates a new graphics path for a rounded rectangle
        /// </summary>
        /// <param name="rect">The rectangle to create a path for</param>
        /// <param name="radius">The radius of the corners</param>
        /// <param name="corners">The corners to be rounded</param>
        /// <returns>A graphics path ready for rendering</returns>
        public static GraphicsPath Create(Rectangle rect, float radius, RectangleCorners corners)
        { return Create(rect.X, rect.Y, rect.Width, rect.Height, radius, corners); }

        /// <summary>
        /// Creates a new graphics path for a rounded rectangle
        /// </summary>
        /// <param name="rect">The rectangle to create a path for</param>
        /// <param name="radius">The radius of the corners</param>
        /// <param name="corners">The corners to be rounded</param>
        /// <returns>A graphics path ready for rendering</returns>
        public static GraphicsPath Create(RectangleF rect, float radius, RectangleCorners corners)
        { return Create(rect.X, rect.Y, rect.Width, rect.Height, radius, corners); }

        /// <summary>
        /// Creates a new graphics path for a rounded rectangle
        /// </summary>
        /// <param name="x">The top-left x coordinate</param>
        /// <param name="y">The top-left y coordinate</param>
        /// <param name="width">The width of the rectangle</param>
        /// <param name="height">The height of the rectangle</param>
        /// <param name="radius">The radius of the corners</param>
        /// <returns>A graphics path ready for rendering</returns>
        public static GraphicsPath Create(float x, float y, float width, float height, float radius)
        { return Create(x, y, width, height, radius, RectangleCorners.All); }

        /// <summary>
        /// Creates a new graphics path for a rounded rectangle
        /// </summary>
        /// <param name="rect">The rectangle to create a path for</param>
        /// <param name="radius">The radius of the corners</param>
        /// <returns>A graphics path ready for rendering</returns>
        public static GraphicsPath Create(Rectangle rect, float radius)
        { return Create(rect.X, rect.Y, rect.Width, rect.Height, radius); }

        /// <summary>
        /// Creates a new graphics path for a rounded rectangle
        /// </summary>
        /// <param name="rect">The rectangle to create a path for</param>
        /// <param name="radius">The radius of the corners</param>
        /// <returns>A graphics path ready for rendering</returns>
        public static GraphicsPath Create(RectangleF rect, float radius)
        { return Create(rect.X, rect.Y, rect.Width, rect.Height, radius); }

        /// <summary>
        /// Creates a new graphics path for a rounded rectangle
        /// </summary>
        /// <param name="x">The top-left x coordinate</param>
        /// <param name="y">The top-left y coordinate</param>
        /// <param name="width">The width of the rectangle</param>
        /// <param name="height">The height of the rectangle</param>
        /// <returns>A graphics path ready for rendering</returns>
        public static GraphicsPath Create(float x, float y, float width, float height)
        { return Create(x, y, width, height, 5); }

        /// <summary>
        /// Creates a new graphics path for a rounded rectangle
        /// </summary>
        /// <param name="rect">The rectangle to create a path for</param>
        /// <returns>A graphics path ready for rendering</returns>
        public static GraphicsPath Create(Rectangle rect)
        { return Create(rect.X, rect.Y, rect.Width, rect.Height); }

        /// <summary>
        /// Creates a new graphics path for a rounded rectangle
        /// </summary>
        /// <param name="rect">The rectangle to create a path for</param>
        /// <returns>A graphics path ready for rendering</returns>
        public static GraphicsPath Create(RectangleF rect)
        { return Create(rect.X, rect.Y, rect.Width, rect.Height); }
    }
}
