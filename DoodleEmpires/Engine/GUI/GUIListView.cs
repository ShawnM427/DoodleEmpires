using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using DoodleEmpires.Engine.Utilities;

namespace DoodleEmpires.Engine.GUI
{
    public class GUIListView : GUIElement
    {
        List<ListViewItem> _items = new List<ListViewItem>();
        ListViewItem _selectedItem;
        protected Rectangle _itemSize = new Rectangle(0, 0,0,0);
        protected Rectangle[] _itemBounds = new Rectangle[4];
        protected Rectangle _internalItemBounds = new Rectangle(0, 0, 0, 0);
        protected int _headerSize= 8;
        protected string _headerText = "";
        protected string _headerDrawnText = "";

        protected int _itemCount = 4;

        protected int _scroll = 0;

        protected int _selectedIndex = -1;

        SpriteFont _font;
        public SpriteFont Font
        {
            get { return _font; }
            set
            {
                _font = value;
                
                if (_font != null)
                    HeaderSize = (int)_font.MeasureString(" ").Y;
            }
        }
        /// <summary>
        /// Gets or sets the header text
        /// </summary>
        public string HeaderText
        {
            get { return _headerText; }
            set
            {
                _headerText = value;
            }
        }
        /// <summary>
        /// Gets or sets the size of the header
        /// </summary>
        public int HeaderSize
        {
            get { return _headerSize; }
            set
            {
                _headerSize = value;

                _internalItemBounds = _bounds;
                _internalItemBounds.Y += _headerSize;
                _internalItemBounds.Height = _bounds.Height - _headerSize;

                _itemSize.Width = _internalItemBounds.Width;
                _itemSize.Height = _internalItemBounds.Height / _itemCount;

                BuildItemBounds();
            }
        }

        public override Rectangle Bounds
        {
            get
            {
                return base.Bounds;
            }
            set
            {
                _internalItemBounds = value;
                _internalItemBounds.Y += _headerSize;
                _internalItemBounds.Height = value.Height - _headerSize;

                _itemSize.Width = _internalItemBounds.Width;
                _itemSize.Height = _internalItemBounds.Height / _itemCount;

                BuildItemBounds();

                base.Bounds = value;
            }
        }

        public GUIListView(GraphicsDevice graphics, GUIContainer parent) :
            base(graphics, parent)
        {
            BuildItemBounds();
        }

        private void BuildItemBounds()
        {
            _itemBounds = new Rectangle[_itemCount];

            for (int x = 0; x < _itemCount; x++)
            {
                    _itemBounds[x] = new Rectangle(0,
                        x * _itemSize.Height + _headerSize, _itemSize.Width + 1, _itemSize.Height + 1);
            }
        }

        public virtual void AddItem(ListViewItem item)
        {
            _items.Add(item);
            Invalidating = true;
        }

        public virtual bool RemoveItem(ListViewItem item)
        {
            Invalidating = true;
            return _items.Remove(item);
        }

        protected override void Invalidate()
        {
            if (_selectedIndex == -1 && _items.Count > 0)
            {
                _selectedIndex = 0;
                _headerDrawnText = _headerText + " " + _items[_selectedIndex].Text;
                _items[_selectedIndex].Selected = true;
            }

            if (_font != null)
            {
                _spriteBatch.DrawString(_font, _headerDrawnText, Vector2.Zero, _foreColor);

                for (int x = 0; x < _items.Count; x++)
                {
                    Vector2 tPos = - _font.MeasureString(_items[x].Text) / 2;
                    tPos.X += _itemBounds[x].Center.X ;
                    tPos.Y += _itemBounds[x].Center.Y;

                    _spriteBatch.DrawString(_font, _items[x].Text, tPos, _items[x].ColorModifier);
                }
            }
        }

        protected override void EndInvalidate()
        {
            _spriteBatch.End();

            _effect.CurrentTechnique.Passes[0].Apply();
            _graphics.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, _cornerVerts, 0, 4);
            for (int x = 0; x < _itemCount; x++)
            {
                _graphics.DrawRect(_itemBounds[x], Color.Black);
            }
            base.EndInvalidate();
        }

        public override bool MousePressed(MouseEventArgs e)
        {
            Vector2 sMousePos = e.Location - new Vector2(_screenBounds.X, _screenBounds.Y);

            for (int x = 0; x < _items.Count; x++)
            {
                if (_itemBounds[x].Contains(sMousePos))
                {
                    if (_selectedIndex >= 0)
                        _items[_selectedIndex].Selected = false;

                    _items[x].Selected = true;
                    _selectedIndex = x;
                    _headerDrawnText = _headerText + " " + _items[x].Text;
                    _items[x].MousePressed.Raise(this, _items[x]);
                    Invalidating = true;

                    return true;
                }
            }

            return false;
        }
    }

    public class ListViewItem : EventArgs
    {
        Texture2D _texture;
        string _text;
        object _tag;
        Color _colorModifier = Color.White;
        EventHandler<ListViewItem> _mousePressed;
        bool _selected = false;

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        public EventHandler<ListViewItem> MousePressed
        {
            get { return _mousePressed; }
            set { _mousePressed += value; }
        }
        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }
        public Color ColorModifier
        {
            get { return _colorModifier; }
            set { _colorModifier = value; }
        }
        public bool Selected
        {
            get { return _selected; }
            set { _selected = value; }
        }
    }
}
