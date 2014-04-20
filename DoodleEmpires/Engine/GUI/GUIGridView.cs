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
    public class GUIGridView : GUIElement
    {
        List<GridViewItem> _items = new List<GridViewItem>();
        protected Rectangle _itemSize = new Rectangle(0, 0,0,0);
        protected Rectangle[,] _itemBounds = new Rectangle[4, 4];
        protected Rectangle _internalItemBounds = new Rectangle(0, 0, 0, 0);
        protected int _headerSize= 8;
        protected string _headerText = "";
        protected string _headerDrawnText = "";

        protected int _xItems = 4;
        protected int _yItems = 4;

        protected int _xOffset = 0;

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

                _itemSize.Width = _internalItemBounds.Width / _xItems;
                _itemSize.Height = _internalItemBounds.Height / _yItems;

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

                _itemSize.Width = _internalItemBounds.Width / _xItems;
                _itemSize.Height = _internalItemBounds.Height / _yItems;

                BuildItemBounds();

                base.Bounds = value;
            }
        }

        public GUIGridView(GraphicsDevice graphics, GUIContainer parent) :
            base(graphics, parent)
        {
            BuildItemBounds();
        }

        private void BuildItemBounds()
        {
            _itemBounds = new Rectangle[_xItems, _yItems];

            for (int x = 0; x < _xItems; x++)
            {
                for (int y = 0; y < _yItems; y++)
                {
                    _itemBounds[x, y] = new Rectangle(x * _itemSize.Width,
                        y * _itemSize.Height + _headerSize, _itemSize.Width + 1, _itemSize.Height + 1);
                }
            }
        }

        public virtual void AddItem(GridViewItem item)
        {
            _items.Add(item);
        }
        
        public virtual bool RemoveItem(GridViewItem item)
        {
            return _items.Remove(item);
        }

        protected override void Invalidate()
        {
            if (_selectedIndex == -1)
            {
                _selectedIndex = 0;
                _headerDrawnText = _headerText + " " + _items[_selectedIndex].Text;
                _items[_selectedIndex].Selected = true;
            }

            if (_font != null)
                _spriteBatch.DrawString(_font, _headerDrawnText, Vector2.Zero, _foreColor);

            _spriteBatch.End();
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            for (int x = 0; x < _xItems; x++)
            {
                for (int y = 0; y < _yItems; y++)
                {
                    int ID = x * _yItems + y;

                    if (ID < _items.Count)
                        _spriteBatch.Draw(_items[ID].Texture, _itemBounds[x, y], _items[ID].Selected ?
                            _items[ID].ColorModifier : _items[ID].ColorModifier * 0.9f);
                }
            }
        }

        protected override void EndInvalidate()
        {
            _spriteBatch.End();

            _graphics.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, _cornerVerts, 0, 4);
            for (int x = 0; x < _xItems; x++)
            {
                for (int y = 0; y < _yItems; y++)
                {
                    _graphics.DrawRect(_itemBounds[x, y], Color.Black);
                }

            }
            base.EndInvalidate();
        }

        public override bool MousePressed(MouseEventArgs e)
        {
            Vector2 sMousePos = e.Location - new Vector2(_screenBounds.X, _screenBounds.Y);

            //if (_internalItemBounds.Contains(sMousePos))
            {
                for (int x = 0; x < _xItems; x++)
                {
                    for (int y = 0; y < _yItems; y++)
                    {
                        if (_itemBounds[x, y].Contains(sMousePos))
                        {
                            int ID = x * _yItems + y;

                            if (ID < _items.Count && _items[ID].MousePressed != null)
                            {
                                _items[_selectedIndex].Selected = false;
                                _items[ID].Selected = true;
                                _selectedIndex = ID;
                                _headerDrawnText = _headerText + " " + _items[ID].Text;
                                _items[ID].MousePressed.Invoke(this, _items[ID]);
                                Invalidating = true;
                            }

                            return true;
                        }
                    }
                }

                return true;
            }
            return false;
        }
    }

    public class GridViewItem : EventArgs
    {
        Texture2D _texture;
        string _text;
        EventHandler<GridViewItem> _mouseClicked;
        object _tag;
        Color _colorModifier = Color.White;
        bool _selected = false;

        public Texture2D Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        public EventHandler<GridViewItem> MousePressed
        {
            get{return _mouseClicked;}
            set{_mouseClicked = value;}
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
