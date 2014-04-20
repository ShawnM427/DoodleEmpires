using System;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Utilities
{
    public struct RectangleF : IEquatable<RectangleF>
    {
        #region Private Fields
        private static RectangleF _emptyRectangle = new RectangleF();
        #endregion Private Fields
        
        #region Public Fields
        public float X;
        public float Y;
        public float Width;
        public float Height;
        #endregion Public Fields
        
        #region Public Properties
        public static RectangleF Empty
        {
            get { return _emptyRectangle; }
        }
        public float Left
        {
            get { return X; }
        }
        public float Right
        {
            get { return (X + Width); }
        }
        public float Top
        {
            get { return Y; }
        }
        public float Bottom
        {
            get { return (Y + Height); }
        }
        #endregion Public Properties
        
        #region Constructors
        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        #endregion Constructors
        
        #region Public Methods
        public static bool operator ==(RectangleF a, RectangleF b)
        {
            return ((a.X == b.X) && (a.Y == b.Y) && (a.Width == b.Width) && (a.Height == b.Height));
        }

        public bool Contains(float x, float y)
        {
            return ((((this.X <= x) && (x < (this.X + this.Width))) && (this.Y <= y)) && (y < (this.Y + this.Height)));
        }

        public bool Contains(Vector2 value)
        {
            return ((((this.X <= value.X) && (value.X < (this.X + this.Width))) && (this.Y <= value.Y)) && (value.Y < (this.Y + this.Height)));
        }

        public bool Contains(Point value)
        {
            return ((((this.X <= value.X) && (value.X < (this.X + this.Width))) && (this.Y <= value.Y)) && (value.Y < (this.Y + this.Height)));
        }

        public bool Contains(RectangleF value)
        {
            return ((((this.X <= value.X) && ((value.X + value.Width) <= (this.X + this.Width))) && (this.Y <= value.Y)) && ((value.Y + value.Height) <= (this.Y + this.Height)));
        }

        public static bool operator !=(RectangleF a, RectangleF b)
        {
            return !(a == b);
        }

        public void Offset(Point offset)
        {
            X += offset.X;
            Y += offset.Y;
        }

        public void Offset(int offsetX, int offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        public Vector2 Center
        {
            get
            {
                return new Vector2((this.X + this.Width) / 2, (this.Y + this.Height) / 2);
            }
        }
        
        public void Inflate(int horizontalValue, int verticalValue)
        {
            X -= horizontalValue;
            Y -= verticalValue;
            Width += horizontalValue * 2;
            Height += verticalValue * 2;
        }

        public bool IsEmpty
        {
            get
            {
                return ((((this.Width == 0) && (this.Height == 0)) && (this.X == 0)) && (this.Y == 0));
            }
        }

        public bool Equals(RectangleF other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return (obj is RectangleF) ? this == ((RectangleF)obj) : false;
        }

        public override string ToString()
        {
            return string.Format("{{X:{0} Y:{1} Width:{2} Height:{3}}}", X, Y, Width, Height);
        }

        public override int GetHashCode()
        {
            return (int)(Math.Pow(this.X ,Math.Pow(this.Y ,Math.Pow(this.Width, this.Height))));
        }

        public bool Intersects(RectangleF r2)
        {
            return !(r2.Left > Right
                     || r2.Right < Left
                     || r2.Top > Bottom
                     || r2.Bottom < Top
                    );

        }
        
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