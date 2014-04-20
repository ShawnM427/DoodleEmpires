using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DoodleEmpires.Engine.Utilities;

namespace DoodleEmpires.Engine.GUI
{
    public class GUIButton : GUILabel
    {
        protected Vector2 _textSize;
        protected Vector2 _textPos;

        public event Action OnMousePressed;
        
        /// <summary>
        /// Gets or sets the text on this button
        /// </summary>
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                _text = value;
                _textSize = _font.MeasureString(_text);
                _textPos = new Vector2(_bounds.Width / 2, _bounds.Height / 2) - (_textSize / 2);
                Invalidating = true;
            }
        }

        public GUIButton(GraphicsDevice graphics, SpriteFont font, GUIContainer parent)
            : base(graphics, font, parent)
        {
        }

        protected override void Resized()
        {
            _textPos = new Vector2(
                    (int)(_bounds.Width / 2 - _textSize.X / 2), 
                    (int)(_bounds.Height / 2 - _textSize.Y / 2));
        }

        protected override void Invalidate()
        {
            _graphics.DrawRect(0, 0, _bounds.Width, _bounds.Height, Color.Black);
            _spriteBatch.DrawString(_font, _text, _textPos, _foreColor);
        }

        public override bool MousePressed(MouseEventArgs e)
        {
            if (_screenBounds.Contains(e.Location))
            {
                if (OnMousePressed != null)
                {
                    OnMousePressed.Invoke();
                    return true;
                }
            }
            return false;
        }
    }
}
