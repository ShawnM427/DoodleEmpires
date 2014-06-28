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
    /// <summary>
    /// A GUI element that displays a scrollable list of items
    /// </summary>
    public class GUIListView : GUIElement
    {
        List<ListViewItem> _items = new List<ListViewItem>();
        ListViewItem _selectedItem;

        /// <summary>
        /// A rectangle representing the size of one item
        /// </summary>
        protected Rectangle _itemSize = new Rectangle(0, 0,0,0);
        /// <summary>
        /// An array containing all the item bounds
        /// </summary>
        protected Rectangle[] _itemBounds = new Rectangle[4];
        /// <summary>
        /// The internal item bounds
        /// </summary>
        protected Rectangle _internalItemBounds = new Rectangle(0, 0, 0, 0);
        /// <summary>
        /// The height of the header
        /// </summary>
        protected int _headerSize= 8;
        /// <summary>
        /// The string to draw in the header box
        /// </summary>
        protected string _headerText = "";
        /// <summary>
        /// The actual string drawn in the header box
        /// </summary>
        protected string _headerDrawnText = "";

        /// <summary>
        /// The number of items in this list view
        /// </summary>
        protected int _itemCount = 4;

        /// <summary>
        /// The amount that this panel is scrolled by
        /// </summary>
        protected int _scroll = 0;

        /// <summary>
        /// The currently selected item index
        /// </summary>
        protected int _selectedIndex = -1;

        SpriteFont _font;
        /// <summary>
        /// Gets or sets the font for this control to use
        /// </summary>
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

        /// <summary>
        /// Gets or sets the client bounds for this control
        /// </summary>
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

        /// <summary>
        /// Creates a new GUI list view
        /// </summary>
        /// <param name="graphics">The graphics device to bind to</param>
        /// <param name="parent">The parent control</param>
        public GUIListView(GraphicsDevice graphics, GUIContainer parent) :
            base(graphics, parent)
        {
            BuildItemBounds();
        }

        /// <summary>
        /// Build the item bounds for this list
        /// </summary>
        private void BuildItemBounds()
        {
            _itemBounds = new Rectangle[_itemCount];

            for (int x = 0; x < _itemCount; x++)
            {
                    _itemBounds[x] = new Rectangle(0,
                        x * _itemSize.Height + _headerSize, _itemSize.Width + 1, _itemSize.Height + 1);
            }
        }

        /// <summary>
        /// Adds an item to this list view
        /// </summary>
        /// <param name="item">The item to add</param>
        public virtual void AddItem(ListViewItem item)
        {
            _items.Add(item);
            Invalidating = true;
        }

        /// <summary>
        /// Removes an item from this list view
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns>True if sucessful</returns>
        public virtual bool RemoveItem(ListViewItem item)
        {
            Invalidating = true;
            return _items.Remove(item);
        }

        /// <summary>
        /// Called when this control invalidates
        /// </summary>
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

        /// <summary>
        /// Ends invalidation on this control
        /// </summary>
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

        /// <summary>
        /// Called when the mouse is pressed over this control
        /// </summary>
        /// <param name="e">The mouse arguments</param>
        public override void MousePressed(MouseEventArgs e)
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
                    _selectedItem = _items[x];
                    _headerDrawnText = _headerText + " " + _items[x].Text;
                    _items[x].MousePressed.Raise(this, _items[x]);
                    Invalidating = true;
                }
            }
        }
    }

    /// <summary>
    /// Represents an item in a list view
    /// </summary>
    public class ListViewItem : EventArgs
    {
        Texture2D _texture;
        string _text;
        object _tag;
        Color _colorModifier = Color.White;
        EventHandler<ListViewItem> _mousePressed;
        bool _selected = false;

        /// <summary>
        /// Gets or sets this item's text
        /// </summary>
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        /// <summary>
        /// Gets or sets this item's mouse cicked event
        /// </summary>
        public EventHandler<ListViewItem> MousePressed
        {
            get { return _mousePressed; }
            set { _mousePressed += value; }
        }
        /// <summary>
        /// Gets or sets this item's tag
        /// </summary>
        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }
        /// <summary>
        /// Gets or sets this item's Color modifier
        /// </summary>
        public Color ColorModifier
        {
            get { return _colorModifier; }
            set { _colorModifier = value; }
        }
        /// <summary>
        /// Gets or sets whether this item is selected
        /// </summary>
        public bool Selected
        {
            get { return _selected; }
            set { _selected = value; }
        }
        /// <summary>
        /// Gets or sets the texture for this item's icon
        /// </summary>
        public Texture2D Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }
    }
}
