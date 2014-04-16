using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.GUI
{
    public class GUILabel : GUIElement
    {
        protected string _text;
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
                Invalidating = true;
            }
        }

        public GUILabel(GraphicsDevice graphics, SpriteFont font, GUIContainer parent)
            : base(graphics, parent)
        {
            _font = font;
            _text = "";
        }

        protected override void BeginInvalidate()
        {
            _bounds.Width = (int)_font.MeasureString(_text).X;
            _bounds.Height = (int)_font.MeasureString(_text).Y;

            base.BeginInvalidate();
        }

        protected override void Invalidate()
        {
            _spriteBatch.DrawString(_font, _text, Vector2.Zero, _foreColor);
        }
    }
}
