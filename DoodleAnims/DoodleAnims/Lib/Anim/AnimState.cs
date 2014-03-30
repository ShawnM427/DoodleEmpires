using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DoodleAnims.Lib.Anim
{
    /// <summary>
    /// Represents a single limb's state
    /// </summary>
    public class AnimState
    {
        int _id;
        float _rotation;
        float _scale;
        Color _color;
        float _offsetX;
        float _offsetY;
        float _xScale;
        float _yScale;
        float _imageAngle;

        /// <summary>
        /// Gets the ID of the limb that this anim state is associated with
        /// </summary>
        public int ID
        {
            get { return _id; }
        }
        /// <summary>
        /// Gets or sets the new scale for the limb
        /// </summary>
        public float Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
            }
        }
        /// <summary>
        /// Gets or sets the rotation for the limb
        /// </summary>
        public float Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
            }
        }
        /// <summary>
        /// Gets or sets the color for the limb
        /// </summary>
        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
            }
        }

        /// <summary>
        /// Gets or sets the offset x value
        /// </summary>
        public float OffsetX
        {
            get { return _offsetX; }
            set { _offsetX = value; }
        }
        /// <summary>
        /// Gets or sets the offset y value
        /// </summary>
        public float OffsetY
        {
            get { return _offsetY; }
            set { _offsetY = value; }
        }
        /// <summary>
        /// Gets or sets the offset value
        /// </summary>
        public PointF Offset
        {
            get { return new PointF(_offsetX, _offsetY); }
            set
            {
                _offsetX = value.X;
                _offsetY = value.Y;
            }
        }

        /// <summary>
        /// Gets or sets whether this limb flips it's image horizontally
        /// </summary>
        public float XScale
        {
            get { return _xScale; }
            set { _xScale = value; }
        }
        /// <summary>
        /// Gets or sets whether this limb flips it's image vertically
        /// </summary>
        public float YScale
        {
            get { return _yScale; }
            set { _yScale = value; }
        }
        /// <summary>
        /// Gets or sets the angle for this limb's image
        /// </summary>
        public float ImageAngle
        {
            get { return _imageAngle; }
            set { _imageAngle = value; }
        }

        /// <summary>
        /// Creates a new blank anim state bound to a specific ID
        /// </summary>
        /// <param name="id">The ID of the limb to bind to</param>
        public AnimState(int id)
        {
            _id = id;
            _rotation = 0;

            _scale = 1;
            _color = Color.Black;
            _offsetX = 0;
            _offsetY = 0;
            _xScale = 1;
            _yScale = 1;
            _imageAngle = 0;
        }

        /// <summary>
        /// Creates a new animation state from a limb's settings
        /// </summary>
        /// <param name="baseLimb">The limb to grab the settings from</param>
        public AnimState(Limb baseLimb)
        {
            _id = baseLimb.ID;
            _rotation = baseLimb.Rotation;

            _scale = baseLimb.Scale;
            _color = baseLimb.Color;
            _offsetX = baseLimb.OffsetX;
            _offsetY = baseLimb.OffsetY;
            _xScale = baseLimb.XScale;
            _yScale = baseLimb.YScale;
            _imageAngle = baseLimb.ImageAngle;
        }

        /// <summary>
        /// Performs linear interpolation over two anim states
        /// </summary>
        /// <param name="min">The start anim state</param>
        /// <param name="max">The end anim state</param>
        /// <param name="percent">The percentage from in to max, between 0 and 1</param>
        /// <returns>An animation state lerped between the min and max</returns>
        public static AnimState Lerp(AnimState min, AnimState max, float percent)
        {
            if (min.ID == max.ID)
            {
                AnimState val = new AnimState(min.ID);

                val.Scale = Math2.Lerp(min.Scale, max.Scale, percent);
                val.Rotation = Math2.Lerp(min.Rotation, max.Rotation, percent);
                val.Color = Math2.Lerp(min.Color, max.Color, percent);
                val.OffsetX = Math2.Lerp(min.OffsetX, max.OffsetX, percent);
                val.OffsetY = Math2.Lerp(min.OffsetY, max.OffsetY, percent);
                val.XScale = Math2.Lerp(min.XScale, max.XScale, percent);
                val.YScale = Math2.Lerp(min.YScale, max.YScale, percent);
                val.ImageAngle = Math2.Lerp(min.ImageAngle, max.ImageAngle, percent);

                return val;
            }
            else
                throw new InvalidOperationException("The ID must be the same when lerping anim states!");
        }
    }
}
