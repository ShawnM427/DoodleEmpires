using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DoodleEmpires.Engine.Utilities;

namespace DoodleEmpires.Engine.GUI
{
    /// <summary>
    /// A GUI element representing a clickable button
    /// </summary>
    public class GUIButton : GUILabel
    {
        /// <summary>
        /// The size of the text being drawn on the button
        /// </summary>
        protected Vector2 _textSize;
        /// <summary>
        /// The position of the text within the button
        /// </summary>
        protected Vector2 _textPos;

        /// <summary>
        /// Occurs when the mouse is pressed over this button
        /// </summary>
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

        /// <summary>
        /// Creates a new instance of a button
        /// </summary>
        /// <param name="graphics">The graphics device to bind to</param>
        /// <param name="font">The font to render text with</param>
        /// <param name="parent">The parent control</param>
        public GUIButton(GraphicsDevice graphics, SpriteFont font, GUIContainer parent)
            : base(graphics, font, parent)
        {
        }

        /// <summary>
        /// Called when this control has been resized
        /// </summary>
        protected override void Resized()
        {
            _textPos = new Vector2(
                    (int)(_bounds.Width / 2 - _textSize.X / 2), 
                    (int)(_bounds.Height / 2 - _textSize.Y / 2));
        }

        /// <summary>
        /// Called when this control needs to invalidate
        /// </summary>
        protected override void Invalidate()
        {
            _graphics.DrawRect(0, 0, _bounds.Width, _bounds.Height, Color.Black);
            _spriteBatch.DrawString(_font, _text, _textPos, _foreColor);
        }

        /// <summary>
        /// Called when this control has been pressed by the mouse
        /// </summary>
        /// <param name="e">The mouse arguments</param>
        /// <returns>True if this control has handled the mouse</returns>
        public override void MousePressed(MouseEventArgs e)
        {
            if (OnMousePressed != null)
            {
                OnMousePressed.Invoke();
            }
        }
    }
}
