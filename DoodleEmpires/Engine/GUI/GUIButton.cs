using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DoodleEmpires.Engine.Utilities;

namespace DoodleEmpires.Engine.GUI
{
    public class GUIButton<T> : GUILabel
    {
        public event Action<T> OnMousePressed;

        T _tag;

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
        /// Gets or sets the action tag associated to this object
        /// </summary>
        public T Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        public GUIButton(GraphicsDevice graphics, SpriteFont font, GUIContainer parent)
            : base(graphics, font, parent)
        {
        }
        
        protected override void Invalidate()
        {
            _graphics.DrawRect(_bounds, Color.Black);
            _spriteBatch.DrawString(_font, _text, Vector2.Zero, _foreColor);
        }

        public override bool MousePressed(MouseEventArgs e)
        {
            if (_screenBounds.Contains(e.Location))
            {
                if (OnMousePressed != null)
                {
                    OnMousePressed.Invoke(_tag);
                    return true;
                }
            }
            return false;
        }
    }
}
