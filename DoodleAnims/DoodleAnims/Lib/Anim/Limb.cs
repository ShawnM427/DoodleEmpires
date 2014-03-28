using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;

namespace DoodleAnims.Lib.Anim
{
    public class Limb
    {
        const float MAX_MOUSE_RANGE = 100.0F;
        const double TO_DEG = (180.0 / Math.PI);

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

        Pen _drawPen;

        public float Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                _drawPen.Width = _scale;
            }
        }
        public float Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                _radRot = _rotation * (float)Math.PI / 180;
            }
        }
        public float Length
        {
            get { return _length; }
            set { _length = value; }
        }
        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value; _drawPen.Color = _color;
            }
        }
        public string Name
        {
            get;
            set;
        }

        public PointF EndPoint
        {
            get { return _endPoint; }
        }

        protected object _tag;

        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        public Limb(PointF orgin, Color color, int length = 5)
        {
            _scale = 1;
            _rotation = 0;
            _length = length;
            _color = color;
            _orgin = orgin;

            _drawPen = new Pen(_color, _scale);
        }

        public Limb(Limb parent, Color color, int length = 5)
        {
            _parent = parent;
            _scale = 1;
            _rotation = 0;
            _length = length;
            _color = color;

            parent._children.Add(this);

            _drawPen = new Pen(_color, _scale);
        }

        public void PointAt(PointF pos)
        {
            Rotation = (float)(Math.Atan2(pos.X, pos.Y) * 180.0 / Math.PI);
        }

        public void PointAt(Point pos)
        {
            Rotation = - (float)(Math.Atan2(pos.X - _orgin.X, pos.Y - _orgin.Y) * TO_DEG) + 90.0F;
        }

        PointF _CENTRE;
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
                    _CENTRE.X += (float)Math.Cos(_radRot) * _length;
                    _CENTRE.Y += (float)Math.Sin(_radRot) * _length;

                    graphics.TranslateTransform(_CENTRE.X, _CENTRE.Y);
                    graphics.RotateTransform(_rotation);
                    graphics.TranslateTransform(-_length / 2, -_length / 2);
                    graphics.DrawEllipse(_drawPen, 0, 0, _length, _length);
                    graphics.ResetTransform();
                    break;

                case LimbType.Textured:
                    _CENTRE = _orgin;
                    _CENTRE.X += (float)Math.Cos(_rotation) * _length;
                    _CENTRE.Y += (float)Math.Sin(_rotation) * _length;

                    graphics.TranslateTransform(_CENTRE.X, _CENTRE.Y);
                    graphics.RotateTransform(_rotation);
                    graphics.TranslateTransform(-_length / 2, -_length / 2);
                    graphics.DrawImage((Image)_tag, 0, 0, _length, _length);
                    graphics.ResetTransform();
                    break;
            }

            foreach (Limb l in _children)
            {
                l.Paint(graphics);
            }
        }

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
    }

    public enum LimbType
    {
        Line,
        Circle,
        Textured
    }
}
