using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.GUI
{
    /// <summary>
    /// A GUI element for basic rendering of text
    /// </summary>
    public class GUITextPane : GUIElement
    {
        /// <summary>
        /// The text passed to this component
        /// </summary>
        protected string _text;
        /// <summary>
        /// The text that is actually drawn
        /// </summary>
        protected string _drawnText;
        /// <summary>
        /// The font to draw text in
        /// </summary>
        protected SpriteFont _font; 
        /// <summary>
        /// The margin from either side of the bounds
        /// </summary>
        protected float _margin = 2.0f;
        /// <summary>
        /// The text's position in the frame
        /// </summary>
        protected Vector2 _textPos = new Vector2(2.0f, 2.0f);

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
        /// Gets or sets the text for this label
        /// </summary>
        public virtual string Text
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
                _textPos.X = value;
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
            _font = font;
            _text = "";
            BackColor = Color.White;

            _bounds.Width = (int)_font.MeasureString(_text).X;
            _bounds.Height = (int)_font.MeasureString(_text).Y;
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
            _spriteBatch.DrawString(_font, _drawnText, _textPos, _foreColor);
        }
    }
}
