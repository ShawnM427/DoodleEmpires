using System;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Utilities
{
    /// <summary>
    /// Represents a floating-point rectangle
    /// </summary>
    public struct RectangleF : IEquatable<RectangleF>
    {
        #region Private Fields
        private static RectangleF _emptyRectangle = new RectangleF();
        #endregion Private Fields
        
        #region Public Fields

        /// <summary>
        /// The left side of this rectangle
        /// </summary>
        public float X;
        /// <summary>
        /// The top side of this rectangle
        /// </summary>
        public float Y;
        /// <summary>
        /// The width of this rectangle
        /// </summary>
        public float Width;
        /// <summary>
        /// The height of this rectangle
        /// </summary>
        public float Height;

        #endregion Public Fields
        
        #region Public Properties

        /// <summary>
        /// Gets a blank rectangle with all properties set to zero
        /// </summary>
        public static RectangleF Empty
        {
            get { return _emptyRectangle; }
        }
        /// <summary>
        /// Gets the left bound of this rectangle
        /// </summary>
        public float Left
        {
            get { return X; }
        }
        /// <summary>
        /// Gets the right bound of this rectangle
        /// </summary>
        public float Right
        {
            get { return (X + Width); }
        }
        /// <summary>
        /// Gets the top bound of this rectangle
        /// </summary>
        public float Top
        {
            get { return Y; }
        }
        /// <summary>
        /// Gets the bottom bound of this rectangle
        /// </summary>
        public float Bottom
        {
            get { return (Y + Height); }
        }

        #endregion Public Properties
        
        #region Constructors

        /// <summary>
        /// Creates a new floating point rectangle
        /// </summary>
        /// <param name="x">The x-coord of the rectangle</param>
        /// <param name="y">The y-coord of the rectangle</param>
        /// <param name="width">The width of the rectangle</param>
        /// <param name="height">The height of the rectangle</param>
        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        #endregion Constructors
        
        #region Public Methods

        /// <summary>
        /// Checks equality betweeen 2 rectangles
        /// </summary>
        /// <param name="a">The first rectangle to check</param>
        /// <param name="b">The rectangle to check against</param>
        /// <returns>True if the rectangles are equal</returns>
        public static bool operator ==(RectangleF a, RectangleF b)
        {
            return ((a.X == b.X) && (a.Y == b.Y) && (a.Width == b.Width) && (a.Height == b.Height));
        }

        /// <summary>
        /// Checks inequality betweeen 2 rectangles
        /// </summary>
        /// <param name="a">The first rectangle to check</param>
        /// <param name="b">The rectangle to check against</param>
        /// <returns>True if the rectangles are not equal</returns>
        public static bool operator !=(RectangleF a, RectangleF b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Checks if this rectangle contains a given point
        /// </summary>
        /// <param name="x">The x-coord to check</param>
        /// <param name="y">The y-coord to check</param>
        /// <returns>True if this rectangle contains the point</returns>
        public bool Contains(float x, float y)
        {
            return ((((this.X <= x) && (x < (this.X + this.Width))) && (this.Y <= y)) && (y < (this.Y + this.Height)));
        }

        /// <summary>
        /// Checks if this rectangle contains a given point
        /// </summary>
        /// <param name="value">The point to check</param>
        /// <returns>True if this rectangle contains the point</returns>
        public bool Contains(Vector2 value)
        {
            return ((((this.X <= value.X) && (value.X < (this.X + this.Width))) && (this.Y <= value.Y)) && (value.Y < (this.Y + this.Height)));
        }

        /// <summary>
        /// Checks if this rectangle contains a given point
        /// </summary>
        /// <param name="value">The point to check</param>
        /// <returns>True if this rectangle contains the point</returns>
        public bool Contains(Point value)
        {
            return ((((this.X <= value.X) && (value.X < (this.X + this.Width))) && (this.Y <= value.Y)) && (value.Y < (this.Y + this.Height)));
        }

        /// <summary>
        /// Checks if this rectangle fully contains another rectangle
        /// </summary>
        /// <param name="value">The rectangle to check</param>
        /// <returns>True if this rectangle fully contains the other rectangle</returns>
        public bool Contains(RectangleF value)
        {
            return ((((this.X <= value.X) && ((value.X + value.Width) <= (this.X + this.Width))) && (this.Y <= value.Y)) && ((value.Y + value.Height) <= (this.Y + this.Height)));
        }

        /// <summary>
        /// Shifts this rectangle by a given amount
        /// </summary>
        /// <param name="offset">The point to shift by</param>
        public void Offset(Point offset)
        {
            X += offset.X;
            Y += offset.Y;
        }

        /// <summary>
        /// Shifts this rectangle by a given amount
        /// </summary>
        /// <param name="offsetX">The x-offset</param>
        /// <param name="offsetY">The y-offset</param>
        public void Offset(int offsetX, int offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        /// <summary>
        /// Shifts this rectangle by a given amount
        /// </summary>
        /// <param name="offset">The point to shift by</param>
        public void Offset(Vector2 offset)
        {
            X += offset.X;
            Y += offset.Y;
        }

        /// <summary>
        /// Shifts this rectangle by a given amount
        /// </summary>
        /// <param name="offsetX">The x-offset</param>
        /// <param name="offsetY">The y-offset</param>
        public void Offset(float offsetX, float offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        /// <summary>
        /// Gets the centre of this rectangle
        /// </summary>
        public Vector2 Center
        {
            get
            {
                return new Vector2((this.X + this.Width) / 2, (this.Y + this.Height) / 2);
            }
        }
        
        /// <summary>
        /// Inflates this rectangle by a given amount. This value will be apllied on
        /// both sides of the centre point
        /// </summary>
        /// <param name="horizontalValue">The horizontal value to inflate by</param>
        /// <param name="verticalValue">The vertical value to inflate by</param>
        public void Inflate(float horizontalValue, float verticalValue)
        {
            X -= horizontalValue;
            Y -= verticalValue;
            Width += horizontalValue * 2;
            Height += verticalValue * 2;
        }

        /// <summary>
        /// Gets whether this rectangle has a width and height of 0
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return (this.Width == 0) & (this.Height == 0);
            }
        }

        /// <summary>
        /// Checks if this rectangle is equal to another
        /// </summary>
        /// <param name="other">The rectangle to check against</param>
        /// <returns>True if both rectangles has the same values</returns>
        public bool Equals(RectangleF other)
        {
            return this == other;
        }

        /// <summary>
        /// Checks if this rectangle is equal to another object
        /// </summary>
        /// <param name="obj">The object to check against</param>
        /// <returns>True if this rectangle is equal to the object</returns>
        public override bool Equals(object obj)
        {
            return (obj is RectangleF) ? this == ((RectangleF)obj) : false;
        }

        /// <summary>
        /// Creates a string representation of this rectangle
        /// </summary>
        /// <returns>A string representation of this rectangle</returns>
        public override string ToString()
        {
            return string.Format("{{X:{0} Y:{1} Width:{2} Height:{3}}}", X, Y, Width, Height);
        }

        /// <summary>
        /// Gets an integer hash code representing this rectangle
        /// </summary>
        /// <returns>A unique hash code</returns>
        public override int GetHashCode()
        {
            return (int)(Math.Pow(this.X ,Math.Pow(this.Y ,Math.Pow(this.Width, this.Height))));
        }

        /// <summary>
        /// Checks if this rectangle intersects another rectangle
        /// </summary>
        /// <param name="r2">The rectangle to check intersection against</param>
        /// <returns>True if the two triangles intersect</returns>
        public bool Intersects(RectangleF r2)
        {
            return !(r2.Left > Right
                     || r2.Right < Left
                     || r2.Top > Bottom
                     || r2.Bottom < Top
                    );

        }

        /// <summary>
        /// Checks if this rectangle intersects another rectangle
        /// </summary>
        /// <param name="value">The rectangle to check intersection against</param>
        /// <param name="result">The bool to set true or false</param>
        /// <returns>True if the two triangles intersect</returns>
        public void Intersects(ref RectangleF value, out bool result)
        {
            result = !(value.Left > Right
                     || value.Right < Left
                     || value.Top > Bottom
                     || value.Bottom < Top
                    );

        }

        #endregion Public Methods
    }
}