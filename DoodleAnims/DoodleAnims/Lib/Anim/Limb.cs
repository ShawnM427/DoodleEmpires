using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace DoodleAnims.Lib.Anim
{
    /// <summary>
    /// Represents a limb for a body
    /// </summary>
    public class Limb
    {
        const float MAX_MOUSE_RANGE = 100.0F;
        const double TO_DEG = (180.0 / Math.PI);

        #region Private Vars
        Limb _parent;
        List<Limb> _children = new List<Limb>();
        LimbType _limbType = LimbType.Line;

        PointF _orgin;
        PointF _endPoint;
        float _rotation = 0;
        float _radRot = 0;
        float _length = 1;
        
        float _scale = 1;
        Color _color;

        protected object _tag;

        Pen _drawPen;
        #endregion

        #region Public Fields
        /// <summary>
        /// Gets or sets the scale for this limb
        /// </summary>
        public float Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                _drawPen.Width = _scale;
            }
        }
        /// <summary>
        /// Gets or sets the rotation for this limb
        /// </summary>
        public float Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                _radRot = _rotation * (float)Math.PI / 180;
            }
        }
        /// <summary>
        /// Gets or sets the length for this limb
        /// </summary>
        public float Length
        {
            get { return _length; }
            set { _length = value; }
        }
        /// <summary>
        /// Gets or sets the color for this limb
        /// </summary>
        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value; _drawPen.Color = _color;
            }
        }
        /// <summary>
        /// Gets or sets the name for this limb
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the limb type for this limb
        /// </summary>
        public LimbType LimbType
        {
            get { return _limbType; }
            set { _limbType = value; }
        }

        /// <summary>
        /// Gets this limbs parent, or null
        /// </summary>
        public Limb Parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// Gets the endpoint for this limb
        /// </summary>
        public PointF EndPoint
        {
            get { return _endPoint; }
        }

        /// <summary>
        /// Gets or sets the tag associated with this object. 
        /// Used for textured limbs
        /// </summary>
        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }
        #endregion

        /// <summary>
        /// Creates a new limb
        /// </summary>
        /// <param name="orgin">The orgin point for this limb</param>
        /// <param name="color">The color of this limb</param>
        /// <param name="length">The length of this limb</param>
        public Limb(PointF orgin, Color color, float length = 5)
        {
            _scale = 1;
            _rotation = 0;
            _length = length;
            _color = color;
            _orgin = orgin;
            Name = "";

            _drawPen = new Pen(_color, _scale);
        }

        /// <summary>
        /// Creates a new limb
        /// </summary>
        /// <param name="parent">The parent limb</param>
        /// <param name="color">The color of this limb</param>
        /// <param name="length">The length of this limb</param>
        public Limb(Limb parent, Color color, float length = 5)
        {
            _parent = parent;
            _scale = 1;
            _rotation = 0;
            _length = length;
            _color = color;

            Name = "";

            parent._children.Add(this);

            _drawPen = new Pen(_color, _scale);
        }

        /// <summary>
        /// Points this limb towards a given point
        /// </summary>
        /// <param name="pos">The point to point this limb at</param>
        public void PointAt(PointF pos)
        {
            Rotation = (float)(Math.Atan2(pos.X, pos.Y) * 180.0 / Math.PI);
        }

        /// <summary>
        /// Points this limb towards a given point
        /// </summary>
        /// <param name="pos">The point to point this limb at</param>
        public void PointAt(Point pos)
        {
            Rotation = - (float)(Math.Atan2(pos.X - _orgin.X, pos.Y - _orgin.Y) * TO_DEG) + 90.0F;
        }

        /// <summary>
        /// Removes a limb from this limb's children limbs
        /// </summary>
        /// <param name="limb">The limb to remove</param>
        /// <returns>True if sucessfull</returns>
        public bool RemoveChild(Limb limb)
        {
            return _children.Remove(limb);
        }

        PointF _CENTRE;
        /// <summary>
        /// Renders this limb and all children limbs to the screen
        /// </summary>
        /// <param name="graphics">The Graphics device to use</param>
        public void Paint(Graphics graphics)
        {
            if (_parent != null)
                _orgin = _parent._endPoint;

            _endPoint.X = _orgin.X + (float)Math.Cos(_radRot) * _length;
            _endPoint.Y = _orgin.Y + (float)Math.Sin(_radRot) * _length;

            switch (_limbType)
            {
                case LimbType.Line:
                    graphics.DrawLine(_drawPen, _orgin, _endPoint);
                    break;
                case LimbType.Circle:
                    _CENTRE = _orgin;
                    _CENTRE.X += (float)Math.Cos(_radRot) * _length / 2;
                    _CENTRE.Y += (float)Math.Sin(_radRot) * _length / 2;

                    graphics.TranslateTransform(_CENTRE.X, _CENTRE.Y);
                    graphics.RotateTransform(_rotation);
                    graphics.TranslateTransform(-_length / 2, -_length / 2);
                    graphics.DrawEllipse(_drawPen, 0, 0, _length, _length);
                    graphics.ResetTransform();
                    break;

                case LimbType.Textured:
                    if (_tag != null)
                    {
                        _CENTRE = _orgin;
                        _CENTRE.X += (float)Math.Cos(_radRot) * _length / 2;
                        _CENTRE.Y += (float)Math.Sin(_radRot) * _length / 2;

                        graphics.TranslateTransform(_CENTRE.X, _CENTRE.Y);
                        graphics.RotateTransform(_rotation + 90);
                        graphics.TranslateTransform(-_length / 2, -_length / 2);
                        graphics.DrawImage((Image)_tag, 0, 0, _length, _length);
                        graphics.ResetTransform();
                    }
                    break;
            }

            foreach (Limb l in _children)
            {
                l.Paint(graphics);
            }
        }

        /// <summary>
        /// Gets the limb within range of the mouse, or null if none is in range
        /// </summary>
        /// <param name="e">The MouseEventArgs to check</param>
        /// <returns>Closest limb to the mouse, or null</returns>
        public Limb Selected(MouseEventArgs e)
        {
            if ((e.X - _endPoint.X) * (e.X - _endPoint.X) + (e.Y - _endPoint.Y) * (e.Y - _endPoint.Y) < MAX_MOUSE_RANGE)
                return this;

            foreach (Limb l in _children)
            {
                if (l.Selected(e) != null)
                    return l.Selected(e);
            }

            return null;
        }

        /// <summary>
        /// Saves a skeleton to a memory stream
        /// </summary>
        /// <param name="w">The binary writer to use</param>
        public void Save(BinaryWriter w)
        {
            w.Write(Name);
            w.Write(_parent != null);
            if (_parent == null)
            {
                w.Write(_orgin.X);
                w.Write(_orgin.Y);
            }
            w.Write(Rotation);
            w.Write(Length);
            w.Write(Scale);
            w.Write(Color.R);
            w.Write(Color.G);
            w.Write(Color.B);
            w.Write(Color.A);
            w.Write((byte)LimbType);
            w.Write(Tag != null);
            if (Tag != null && LimbType == LimbType.Textured)
            {
                w.WriteImage((Bitmap)_tag);
            }
            w.Write(_children.Count);

            foreach (Limb l in _children)
            {
                l.Save(w);
            }
        }

        /// <summary>
        /// Loads a skeleton from a memory stream
        /// </summary>
        /// <param name="parent">The parent limb, or null to start from scratch</param>
        /// <param name="r">The binaryReader to use</param>
        public static Limb Load(Limb parent, BinaryReader r)
        {
            string name = r.ReadString();
            bool hasParent = r.ReadBoolean();
            PointF orgin = PointF.Empty;
            if (!hasParent)
            {
                orgin.X = r.ReadSingle();
                orgin.Y = r.ReadSingle();
            }

            float rot = r.ReadSingle();
            float length = r.ReadSingle();
            float scale = r.ReadSingle();

            byte R = r.ReadByte();
            byte G = r.ReadByte();
            byte B = r.ReadByte();
            byte A = r.ReadByte();

            LimbType limbType = (LimbType)r.ReadByte();
            bool hasTag = r.ReadBoolean();
            object tag = null;
            if (hasTag && limbType == LimbType.Textured)
            {
                tag = r.ReadImage();
            }
            int children = r.ReadInt32();

            Limb l;
            if (hasParent)
            {
                l = new Limb(parent, Color.FromArgb(A, R, G, B), length);
                l._tag = tag;
                l.Name = name;
                l.Rotation = rot;
                l.Scale = scale;
                l.LimbType = limbType;
            }
            else
            {
                l = new Limb(orgin, Color.FromArgb(A, R, G, B), length);
                l._tag = tag;
                l.Name = name;
                l.Rotation = rot;
                l.Scale = scale;
                l.LimbType = limbType;
            }

            for (int i = 0; i < children; i++)
            {
                Load(l, r);
            }

            return l;
        }
    }

    /// <summary>
    /// Represents the type of a limb
    /// </summary>
    public enum LimbType : byte
    {
        Line = 0,
        Circle = 1,
        Textured = 2
    }
}
