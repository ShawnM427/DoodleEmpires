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
    /// A simple GUI element for drawing text
    /// </summary>
    public class GUILabel : GUITextElement
    {
        /// <summary>
        /// The internal position to base bounds around
        /// </summary>
        protected Point _position;

        /// <summary>
        /// Gets or sets the text for this label
        /// </summary>
        public override string Text
        {
            get { return _text; }
            set
            {
                _text = value;

                _bounds.Width = (int)_font.MeasureString(_text).X;
                _bounds.Height = (int)_font.MeasureString(_text).Y;

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
        /// Creates a new instance of a GUI label
        /// </summary>
        /// <param name="graphics">The graphics device to bind to</param>
        /// <param name="font">The font to use to render text</param>
        /// <param name="parent">The parent GUI container</param>
        public GUILabel(GraphicsDevice graphics, SpriteFont font, GUIContainer parent)
            : base(graphics, parent)
        {
            _font = font ?? _font;

            _bounds.Width = (int)_font.MeasureString(_text).X;
            _bounds.Height = (int)_font.MeasureString(_text).Y;

            RecalculateBounds();
        }
        
        protected virtual void RecalculateBounds()
        {
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

        /// <summary>
        /// Called when this label needs to invalidate
        /// </summary>
        protected override void Invalidate()
        {
            _spriteBatch.DrawString(_font, _text, Vector2.Zero, _foreColor);
        }
    }
}
