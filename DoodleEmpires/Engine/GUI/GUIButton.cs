using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DoodleEmpires.Engine.Utilities;
using Microsoft.Xna.Framework.Input;

namespace DoodleEmpires.Engine.GUI
{
    /// <summary>
    /// A GUI element representing a clickable button
    /// </summary>
    public class GUIButton : GUILabel
    {
        /// <summary>
        /// The margin from either side of the bounds
        /// </summary>
        protected float _margin = 2.0f;
        /// <summary>
        /// The alignment of text within this control
        /// </summary>
        protected TextAlignment _alignment = TextAlignment.Centred;

        /// <summary>
        /// Occurs when the mouse is pressed over this button
        /// </summary>
        public event Action OnMousePressed;
        /// <summary>
        /// Gets or sets the text alignment for this control
        /// </summary>
        public virtual TextAlignment Alignment
        {
            get { return _alignment; }
            set { _alignment = value; }
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
                Invalidating = true;
            }
        }
        
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
        /// Called when this control needs to invalidate
        /// </summary>
        protected override void Invalidate()
        {
            _graphics.DrawRect(0, 0, _bounds.Width, _bounds.Height, Color.Black);

            Vector2 textPos = Vector2.Zero;
            Vector2 textSize = _font.MeasureString(_text);
            Vector2 centre = new Vector2(_bounds.Width / 2, _bounds.Height / 2);

            switch (_alignment)
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

            _spriteBatch.DrawString(_font, _text, textPos, _foreColor);
        }

        /// <summary>
        /// Called when this control has been pressed by the mouse
        /// </summary>
        /// <param name="e">The mouse arguments</param>
        /// <returns>True if this control has handled the mouse</returns>
        public override void MousePressed(MouseEventArgs e)
        {
            base.MousePressed(e);

            if (OnMousePressed != null)
            {
                OnMousePressed.Invoke();
            }
        }
    }
}
