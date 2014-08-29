using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MonoUI
{
    /// <summary>
    /// A box that can be checked or unchecked
    /// </summary>
    public class GUICheckBox : GUITextElement
    {
        /// <summary>
        /// The spacing between the check box and the text
        /// </summary>
        protected int _checkSpacing = 2;
        /// <summary>
        /// The width/height of the check box
        /// </summary>
        protected int _checkBoxSize = 1;
        /// <summary>
        /// The bouds to draw the text box in
        /// </summary>
        protected Rectangle _checkBoxBounds;
        /// <summary>
        /// The texture to show when unchecked
        /// </summary>
        protected Texture2D _uncheckedTexture;
        /// <summary>
        /// The texture to show when checked
        /// </summary>
        protected Texture2D _checkedTexture;
        /// <summary>
        /// The internal position to base bounds around
        /// </summary>
        protected Point _position;
        /// <summary>
        /// Whether this check box is checked or not
        /// </summary>
        protected bool _checked = false;

        /// <summary>
        /// Gets or sets the font for this text element
        /// </summary>
        public override SpriteFont Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                _font = value;
                _checkBoxSize = (int)Math.Round(_font.MeasureString(" ").Y);
                RecalculateBounds();
            }
        }
        /// <summary>
        /// Gets or sets the text for this label
        /// </summary>
        public override string Text
        {
            get { return _text; }
            set
            {
                _text = value;

                RecalculateBounds();
            }
        }
        /// <summary>
        /// Gets or sets the text alignment for this control
        /// </summary>
        public override TextAlignment Alignment
        {
            get { return _alignment; }
            set
            {
                _alignment = value;
                RecalculateBounds();
            }
        }
        /// <summary>
        /// Gets or sets the location for this label
        /// </summary>
        public virtual Point Location
        {
            get { return _position; }
            set
            {
                _position = value;
                RecalculateBounds();
                Invalidating = true;
            }
        }
        /// <summary>
        /// Gets or sets the texture to display when unchecked
        /// </summary>
        public virtual Texture2D UncheckedTexture
        {
            get { return _uncheckedTexture; }
            set
            {
                _uncheckedTexture = value;
                Invalidating = true;
            }
        }
        /// <summary>
        /// Gets or sets the texture to display when checked
        /// </summary>
        public virtual Texture2D CheckedTexture
        {
            get { return _checkedTexture; }
            set
            {
                _checkedTexture = value;
                Invalidating = true;
            }
        }
        /// <summary>
        /// Gets or sets whether this check box is checked
        /// </summary>
        public virtual bool Checked
        {
            get { return _checked; }
            set
            {
                _checked = value;
                Invalidating = true;
            }
        }

        /// <summary>
        /// Creates a new instance of a GUI label
        /// </summary>
        /// <param name="graphics">The graphics device to bind to</param>
        /// <param name="font">The font to use to render text</param>
        /// <param name="parent">The parent GUI container</param>
        public GUICheckBox(GraphicsDevice graphics, SpriteFont font, GUIContainer parent)
            : base(graphics, parent)
        {
            _checkedTexture = StaticContentLoader.GetItem<Texture2D>("CheckBox_Checked");
            _uncheckedTexture = StaticContentLoader.GetItem<Texture2D>("CheckBox_Unchecked");
            Font = font ?? _font;

            _bounds.Width = (int)_font.MeasureString(_text).X;
            _bounds.Height = (int)_font.MeasureString(_text).Y;

            RecalculateBounds();
        }
        
        protected virtual void RecalculateBounds()
        {
            _checkBoxBounds = new Rectangle(0, 0, _checkBoxSize, _checkBoxSize);
            _bounds.Width = (int)_font.MeasureString(_text).X + _checkBoxSize + _checkSpacing;
            _bounds.Height = (int)_font.MeasureString(_text).Y;

            switch(_alignment)
            {
                case TextAlignment.Centred:
                    _bounds.X = _position.X - _bounds.Width / 2;
                    _bounds.Y = _position.Y - _bounds.Height / 2;
                    break;
                case TextAlignment.CentreLeft:
                    _bounds.X = _position.X;
                    _bounds.Y = _position.Y - _bounds.Height / 2;
                    break;
                case TextAlignment.CentreRight:
                    _bounds.X = _position.X - _bounds.Width;
                    _bounds.Y = _position.Y - _bounds.Height / 2;
                    break;
                default:
                    throw new NotImplementedException(string.Format("Cannot handle alignment type {0}",_alignment));
            }

            Bounds = _bounds;
            Invalidating = true;
        }
        
        protected virtual void DrawCheckBox(Rectangle bounds, bool isChecked)
        {
            _spriteBatch.Draw(isChecked ? _checkedTexture : _uncheckedTexture, bounds, Color.White);
        }

        /// <summary>
        /// Called when this label needs to invalidate
        /// </summary>
        protected override void Invalidate()
        {
            DrawCheckBox(_checkBoxBounds, _checked);
            _spriteBatch.DrawString(_font, _text, new Vector2(_checkSpacing + _checkBoxSize, 0), _foreColor);
        }

        public override void MousePressed(MouseEventArgs e)
        {
            if (_checkBoxBounds.Contains(e.Position - _screenBounds.Location.ToVector2()))
                Checked = !Checked;
        }
    }
}
