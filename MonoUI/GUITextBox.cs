using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MonoUI
{
    /// <summary>
    /// A simple GUI element for drawing text
    /// </summary>
    public class GUITextBox : GUITextElement
    {
        /// <summary>
        /// The character to hide text behind, null is no hiding
        /// </summary>
        protected char? _passwordChar = null;
        /// <summary>
        /// The edge margin to space text from the edge of this control
        /// </summary>
        protected float _margin = 2.5f;

        /// <summary>
        /// Gets or sets the margin along the edges of this control
        /// </summary>
        public virtual float Margin
        {
            get { return _margin; }
            set { _margin = value; }
        }
        /// <summary>
        /// Gets or sets the character to hide text behind, null is no hiding
        /// </summary>
        public virtual char? PasswordChar
        {
            get { return _passwordChar; }
            set { _passwordChar = value; }
        }

        /// <summary>
        /// Creates a new instance of a GUI label
        /// </summary>
        /// <param name="graphics">The graphics device to bind to</param>
        /// <param name="font">The font to use to render text</param>
        /// <param name="parent">The parent GUI container</param>
        public GUITextBox(GraphicsDevice graphics, SpriteFont font, GUIContainer parent)
            : base(graphics, parent)
        {
            _font = font ?? _font;
            _text = "";
        }
        
        /// <summary>
        /// Begins the invalidation of this control
        /// </summary>
        protected override bool BeginInvalidate()
        {
            if (base.BeginInvalidate())
            {
                _effect.CurrentTechnique.Passes[0].Apply();
                _graphics.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, _cornerVerts, 0, 4);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Called when this label needs to invalidate
        /// </summary>
        protected override void Invalidate()
        {
            Vector2 textPos = Vector2.Zero;
            Vector2 textSize = _font.MeasureString(_text);
            Vector2 centre = new Vector2(_bounds.Width / 2, _bounds.Height / 2);

            switch(_alignment)
            {
                case TextAlignment.Centred:
                    textPos = centre - textSize / 2;
                    break;
                case TextAlignment.TopLeft:
                    textPos = new Vector2(_margin, _margin);
                    break;
                case TextAlignment.CentreLeft:
                    textPos = new Vector2(_margin, centre.Y - textSize.Y / 2);
                    break;
                case TextAlignment.BottomLeft:
                    textPos = new Vector2(_margin, _bounds.Height - textSize.Y - _margin);
                    break;
                default:
                    throw new NotImplementedException();
            }

            textPos.X = (int)textPos.X;
            textPos.Y = (int)textPos.Y;

            string drawnText = _passwordChar == null ? _text : new string(_passwordChar.Value, _text.Length);

            if (Focused)
                _spriteBatch.DrawString(_font, drawnText, textPos, _foreColor);
            else
                _spriteBatch.DrawString(_font, drawnText, textPos, Color.DarkGray);
        }

        /// <summary>
        /// Called when text has been entered via the keyboard
        /// </summary>
        /// <param name="sender">The object to invoke the event</param>
        /// <param name="e">The text input event arguments</param>
        public virtual void OnTextEntered(object sender, TextInputEventArgs e)
        {
            if (Focused)
            {
                switch (e.Character)
                {
                    case '\b':
                        if (_text.Length > 0)
                            _text = _text.Remove(_text.Length - 1, 1);
                        break;
                    default:
                        _text += e.Character;
                        break;
                }

                Invalidating = true;
            }
        }
    }
}
