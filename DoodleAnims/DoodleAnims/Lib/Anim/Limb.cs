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

        Skeleton _parentSkeleton;

        TreeNode _treeNode;

        int _id;

        PointF _orgin;
        PointF _startPos;
        PointF _endDrawPos;
        PointF _endPoint;
        float _offsetX = 0;
        float _offsetY = 0;
        float _rotation = 0;
        float _radRot = 0;
        float _length = 1;

        float _xScale = 1;
        float _yScale = 1;
        float _imageAngle = 0;
        
        float _scale = 1;
        Color _color;

        object _tag;

        string _name;

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
            get { return _name; }
            set { _name = value; if (_treeNode != null) _treeNode.Text = _name; }
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
        /// Gets the orgin for this limb
        /// </summary>
        public PointF Orgin
        {
            get { return _orgin; }
            set
            {
                if (_parent == null)
                    _orgin = value;
            }
        }

        /// <summary>
        /// Gets the root limb for this limb
        /// </summary>
        public Limb RootLimb
        {
            get
            {
                if (_parent == null)
                    return _parent;
                else
                    return _parent.RootLimb;
            }
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
        /// <summary>
        /// Gets a treenode representing this limb
        /// </summary>
        public TreeNode TreeNode
        {
            get { return _treeNode; }
        }

        /// <summary>
        /// Gets the ID of this limb
        /// </summary>
        public int ID
        {
            get { return _id; }
        }
        #endregion

        /// <summary>
        /// Creates a new <b>root</b> limb
        /// </summary>
        /// <param name="skeleton">The owning skeleton, used to declare the root limb</param>
        /// <param name="color">The color of this limb</param>
        /// <param name="length">The length of this limb</param>
        public Limb(Skeleton skeleton, Color color, float length = 5)
        {
            _parentSkeleton = skeleton;
            _scale = 1;
            _rotation = 0;
            _length = length;
            _color = color;
            _orgin = skeleton.Orgin;
            Name = "Orgin";

            _drawPen = new Pen(_color, _scale);
            _id = skeleton.GetID();
            _parentSkeleton.AddLimbRef(this);

            _treeNode = new TreeNode(Name);
            _treeNode.Tag = this;
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
            _parentSkeleton = _parent._parentSkeleton;
            _scale = 1;
            _rotation = 0;
            _length = length;
            _color = color;


            parent._children.Add(this);

            _drawPen = new Pen(_color, _scale);
            _id = _parentSkeleton.GetID();
            _parentSkeleton.AddLimbRef(this);
            Name = _parentSkeleton.GetName();

            _treeNode = new TreeNode(Name);
            _treeNode.Tag = this;

            _parent._treeNode.Nodes.Add(_treeNode);
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
            bool opp = _children.Remove(limb);

            if (opp)
            {
                _parentSkeleton.RemoveLimbRef(limb);
                _treeNode.Nodes.Remove(limb.TreeNode);
            }

            return opp;
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

            _startPos = _orgin;
            _endDrawPos = _orgin;

            float _lengthX = (float)(Math.Cos(_radRot) * OffsetX) - (float)(Math.Sin(-_radRot) * OffsetY);
            float _lengthY = (float)(Math.Sin(_radRot) * OffsetX) - (float)(Math.Cos(-_radRot) * OffsetY);

            _endPoint.X = _orgin.X + (float)(Math.Cos(_radRot) * _length);
            _endPoint.Y = _orgin.Y + (float)(Math.Sin(_radRot) * _length);

            _endDrawPos.X += (float)(Math.Cos(_radRot) * _length) + _lengthX;
            _endDrawPos.Y += (float)(Math.Sin(_radRot) * _length) + _lengthY;

            _startPos.X += _lengthX;
            _startPos.Y += _lengthY;

            switch (_limbType)
            {
                case LimbType.Line:
                    graphics.DrawLine(_drawPen, _startPos, _endDrawPos);
                    break;
                case LimbType.Circle:
                    _CENTRE = _startPos;
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
                        _CENTRE = _startPos;
                        _CENTRE.X += (float)Math.Cos(_radRot) * _length / 2;
                        _CENTRE.Y += (float)Math.Sin(_radRot) * _length / 2;

                        graphics.TranslateTransform(_CENTRE.X, _CENTRE.Y);
                        graphics.RotateTransform(_rotation + 90);
                        graphics.RotateTransform(_imageAngle);
                        graphics.ScaleTransform(XScale, YScale);
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
            if (
                (e.X - _endDrawPos.X) * (e.X - _endDrawPos.X) + (e.Y - _endDrawPos.Y) * (e.Y - _endDrawPos.Y) < MAX_MOUSE_RANGE |
                (e.X - _endPoint.X) * (e.X - _endPoint.X) + (e.Y - _endPoint.Y) * (e.Y - _endPoint.Y) < MAX_MOUSE_RANGE)
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
                w.Write(_children.Count);
            }
            else
            {
                w.Write(Rotation);
                w.Write(Length);
                w.Write(Scale);

                w.Write(OffsetX);
                w.Write(OffsetY);

                w.Write(XScale);
                w.Write(YScale);
                w.Write(ImageAngle);

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
            }

            foreach (Limb l in _children)
            {
                l.Save(w);
            }
        }

        /// <summary>
        /// Loads a skeleton from a memory stream
        /// </summary>
        /// <param name="rootSkeleton">The parent skeleton to load limbs into</param>
        /// <param name="parent">The parent limb, or null to start from scratch</param>
        /// <param name="r">The binaryReader to use</param>
        public static Limb LoadV2(Skeleton rootSkeleton, Limb parent, BinaryReader r)
        {
            string name = r.ReadString();
            bool hasParent = r.ReadBoolean();
            Limb l;
            int children;

            if (hasParent)
            {
                float rot = r.ReadSingle();
                float length = r.ReadSingle();
                float scale = r.ReadSingle();

                float offsetX = r.ReadSingle();
                float offsetY = r.ReadSingle();

                float xScale = r.ReadSingle();
                float yScale = r.ReadSingle();
                float imageAngle = r.ReadSingle();

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

                children = r.ReadInt32();

                l = new Limb(parent, Color.FromArgb(A, R, G, B), length);

                l._tag = tag;
                l.Name = name;
                l.Rotation = rot;
                l.Scale = scale;
                l.OffsetX = offsetX;
                l.OffsetY = offsetY;
                l.XScale = xScale;
                l.YScale = yScale;
                l.ImageAngle = imageAngle;
                l.LimbType = limbType;
            }            
            else
            {
                l = new Limb(rootSkeleton, Color.Transparent, 0);
                children = r.ReadInt32();
            }

            for (int i = 0; i < children; i++)
            {
                LoadV2(rootSkeleton, l, r);
            }

            return l;
        }

        /// <summary>
        /// Loads a skeleton from a memory stream
        /// </summary>
        /// <param name="rootSkeleton">The parent skeleton to load limbs into</param>
        /// <param name="parent">The parent limb, or null to start from scratch</param>
        /// <param name="r">The binaryReader to use</param>
        public static Limb LoadV1(Skeleton rootSkeleton, Limb parent, BinaryReader r)
        {
            string name = r.ReadString();
            bool hasParent = r.ReadBoolean();
            Limb l;
            int children;

            if (hasParent)
            {
                float rot = r.ReadSingle();
                float length = r.ReadSingle();
                float scale = r.ReadSingle();

                float offsetX = r.ReadSingle();
                float offsetY = r.ReadSingle();

                float xScale = r.ReadBoolean() ? -1 : 1;
                float yScale = r.ReadBoolean() ? -1 : 1;
                float imageAngle = r.ReadSingle();

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

                children = r.ReadInt32();

                l = new Limb(parent, Color.FromArgb(A, R, G, B), length);

                l._tag = tag;
                l.Name = name;
                l.Rotation = rot;
                l.Scale = scale;
                l.OffsetX = offsetX;
                l.OffsetY = offsetY;
                l.XScale = xScale;
                l.YScale = yScale;
                l.ImageAngle = imageAngle;
                l.LimbType = limbType;
            }
            else
            {
                l = new Limb(rootSkeleton, Color.Transparent, 0);
                children = r.ReadInt32();
            }

            for (int i = 0; i < children; i++)
            {
                LoadV1(rootSkeleton, l, r);
            }

            return l;
        }
    }

    /// <summary>
    /// Represents the type of a limb
    /// </summary>
    public enum LimbType : byte
    {
        /// <summary>
        /// A simple line
        /// </summary>
        Line = 0,
        /// <summary>
        /// A circle
        /// </summary>
        Circle = 1,
        /// <summary>
        /// A textured segment
        /// </summary>
        Textured = 2
    }
}
