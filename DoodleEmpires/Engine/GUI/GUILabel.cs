using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.GUI
{
    /// <summary>
    /// A simple GUI element for drawing text
    /// </summary>
    public class GUILabel : GUIElement
    {
        /// <summary>
        /// This label's text
        /// </summary>
        protected string _text;
        /// <summary>
        /// This label's font
        /// </summary>
        protected SpriteFont _font;

        /// <summary>
        /// Gets or sets the text for this label
        /// </summary>
        public virtual string Text
        {
            get { return _text; }
            set
            {
                _text = value;

                _bounds.Width = (int)_font.MeasureString(_text).X;
                _bounds.Height = (int)_font.MeasureString(_text).Y;

                Bounds = _bounds;

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
            _font = font;
            _text = "";

            _bounds.Width = (int)_font.MeasureString(_text).X;
            _bounds.Height = (int)_font.MeasureString(_text).Y;
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
