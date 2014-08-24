using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using DoodleEmpires.Engine.Utilities;
using Microsoft.Xna.Framework.Input;

namespace DoodleEmpires.Engine.GUI
{
    /// <summary>
    /// A GUI element that shows a grid of items
    /// </summary>
    public class GUIGridView : GUIElement
    {
        List<GridViewItem> _items = new List<GridViewItem>();

        /// <summary>
        /// A rectangle representing the size of one item
        /// </summary>
        protected Rectangle _itemSize = new Rectangle(0, 0, 0, 0);
        /// <summary>
        /// An array containing all the item bounds
        /// </summary>
        protected Rectangle[,] _itemBounds = new Rectangle[4,4];
        /// <summary>
        /// The internal item bounds
        /// </summary>
        protected Rectangle _internalItemBounds = new Rectangle(0, 0, 0, 0);
        /// <summary>
        /// The height of the header
        /// </summary>
        protected int _headerSize = 8;
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
        /// The currently selected item index
        /// </summary>
        protected int _selectedIndex = -1;

        /// <summary>
        /// The number of items along the x axis
        /// </summary>
        protected int _xItems = 4;
        /// <summary>
        /// The number of items along the y axis
        /// </summary>
        protected int _yItems = 4;

        /// <summary>
        /// The amount of items that this panel is scrolled by along the x axis
        /// </summary>
        protected int _xOffset = 0;

        SpriteFont _font;
        /// <summary>
        /// Gets or sets the font to render text with
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

                _itemSize.Width = _internalItemBounds.Width / _xItems;
                _itemSize.Height = _internalItemBounds.Height / _yItems;

                BuildItemBounds();
            }
        }

        /// <summary>
        /// Gets or sets the bounds for this component
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

                _itemSize.Width = _internalItemBounds.Width / _xItems;
                _itemSize.Height = _internalItemBounds.Height / _yItems;

                BuildItemBounds();

                base.Bounds = value;
            }
        }

        /// <summary>
        /// Creates a new instance of a GUI grid view
        /// </summary>
        /// <param name="graphics">The graphics device to bind to</param>
        /// <param name="parent">The parent container</param>
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

        /// <summary>
        /// Adds an item to this grid view
        /// </summary>
        /// <param name="item">The item to add</param>
        public virtual void AddItem(GridViewItem item)
        {
            _items.Add(item);
        }

        /// <summary>
        /// Removes an item from this grid view
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns>True if this item was removed</returns>
        public virtual bool RemoveItem(GridViewItem item)
        {
            return _items.Remove(item);
        }

        /// <summary>
        /// Called when this component needs to invalidate
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
                _spriteBatch.DrawString(_font, _headerDrawnText, new Vector2(5.0f, 0.0f), _foreColor);

            _spriteBatch.End();
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, null, null);

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

        /// <summary>
        /// Called when this component ends it's invalidation
        /// </summary>
        protected override void EndInvalidate()
        {
            _spriteBatch.End();

            _effect.CurrentTechnique.Passes[0].Apply();
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

        /// <summary>
        /// Called when the mouse is pressed over this component
        /// </summary>
        /// <param name="e">The mouse event arguments</param>
        public override void MousePressed(MouseEventArgs e)
        {
            base.MousePressed(e);

            Vector2 sMousePos = new Vector2(e.X - _screenBounds.X, e.Y - _screenBounds.Y);

            for (int x = 0; x < _xItems; x++)
            {
                for (int y = 0; y < _yItems; y++)
                {
                    if (_itemBounds[x, y].Contains(sMousePos))
                    {
                        int ID = x * _yItems + y;

                        if (ID < _items.Count && _items[ID].MousePressed != null)
                        {
                            if (_selectedIndex >= 0)
                                _items[_selectedIndex].Selected = false;

                            _items[ID].Selected = true;
                            _selectedIndex = ID;
                            _headerDrawnText = _headerText + " " + _items[ID].Text;
                            _items[ID].MousePressed.Invoke(this, _items[ID]);
                            Invalidating = true;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Represents an item in a grid view
    /// </summary>
    public class GridViewItem : EventArgs
    {
        Texture2D _texture;
        string _text;
        EventHandler<GridViewItem> _mouseClicked;
        object _tag;
        Color _colorModifier = Color.White;
        bool _selected = false;

        /// <summary>
        /// The texture of the item
        /// </summary>
        public Texture2D Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }
        /// <summary>
        /// The text to display on the item
        /// </summary>
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        /// <summary>
        /// The event to fire when this item is clicked
        /// </summary>
        public EventHandler<GridViewItem> MousePressed
        {
            get{return _mouseClicked;}
            set{_mouseClicked = value;}
        }
        /// <summary>
        /// This items tag
        /// </summary>
        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }
        /// <summary>
        /// The color modifier for this item
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
    }
}
