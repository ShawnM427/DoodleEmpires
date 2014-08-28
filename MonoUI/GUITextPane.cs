using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MonoUI
{
    /// <summary>
    /// A GUI element for basic rendering of text
    /// </summary>
    public class GUITextPane : GUITextElement
    {
        /// <summary>
        /// The text that is actually drawn
        /// </summary>
        protected string _drawnText;
        /// <summary>
        /// The margin from either side of the bounds
        /// </summary>
        protected float _margin = 2.0f;

        /// <summary>
        /// The bounds relative to the parent container
        /// </summary>
        public override Rectangle Bounds
        {
            get { return base.Bounds; }
            set
            {
                if (_bounds.Width != value.Width || _bounds.Height != value.Height)
                {
                    _cornerVerts[0].Position = new Vector3(0, 0, 0.5f);
                    _cornerVerts[1].Position = new Vector3(value.Width - 1, 0, 0.5f);
                    _cornerVerts[2].Position = new Vector3(value.Width - 1, value.Height - 1, 0.5f);
                    _cornerVerts[3].Position = new Vector3(0, value.Height - 1, 0.5f);
                    _cornerVerts[4].Position = new Vector3(0, 0, 0.5f);
                }

                base.Bounds = value;
            }
        }
        /// <summary>
        /// Gets or sets the text for this pane
        /// </summary>
        public override string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                _drawnText = _text.Wrap(_font, _bounds.Width - 2.0f * _margin);
                Invalidating = true;
            }
        }
        /// <summary>
        /// Gets or sets the horizontal margin within this control
        /// </summary>
        public virtual float Margin
        {
            get { return _margin; }
            set
            {
                _margin = value;
                _drawnText = _text.Wrap(_font, _bounds.Width - 2.0f * _margin);
                Invalidating = true;
            }
        }

        /// <summary>
        /// Creates a new instance of a text pane
        /// </summary>
        /// <param name="graphics">The graphics device to bind to</param>
        /// <param name="font">The font to use for text rendering</param>
        /// <param name="parent">The parent container for this component</param>
        public GUITextPane(GraphicsDevice graphics, SpriteFont font, GUIContainer parent)
            : base(graphics, parent)
        {
            _font = font ?? _font;
            _text = "";
            _drawnText = "";
            BackColor = Color.White;
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
        /// Renders this control's main features to the screen
        /// </summary>
        protected override void Invalidate()
        {
            string[] lines = _drawnText.Split('\r');
            float y = _margin;

            foreach (string line in lines)
            {
                Vector2 textPos = Vector2.Zero;
                Vector2 textSize = _font.MeasureString(line);
                Vector2 centre = new Vector2(_bounds.Width / 2, y + textSize.Y / 2);

                switch (_alignment)
                {
                    case TextAlignment.Centred:
                        textPos = centre - textSize / 2;
                        break;
                    case TextAlignment.CentreLeft:
                        textPos = new Vector2(_margin, centre.Y - textSize.Y / 2);
                        break;
                    case TextAlignment.CentreRight:
                        textPos = new Vector2(_bounds.Width - textSize.X - _margin, centre.Y - textSize.Y / 2);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                textPos.X = (int)textPos.X;
                textPos.Y = (int)textPos.Y;

                _spriteBatch.DrawString(_font, line, textPos, _foreColor);

                y += _font.LineSpacing;
            }
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
                            Text = _text.Remove(_text.Length - 1, 1);
                        break;
                    default:
                        Text += e.Character;
                        break;
                }

                Invalidating = true;
            }
        }
    }
}
