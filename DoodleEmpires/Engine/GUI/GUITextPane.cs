﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.GUI
{
    public class GUITextPane : GUIElement
    {
        protected string _text;
        protected string _drawnText;
        protected SpriteFont _font;
        protected VertexPositionColor[] _cornerVerts = new VertexPositionColor[5]
        {
            new VertexPositionColor(new Vector3(0, 0, 0.5f), Color.Black),
            new VertexPositionColor(new Vector3( 32, 0, 0.5f), Color.Black),
            new VertexPositionColor(new Vector3(32, 32, 0.5f), Color.Black),
            new VertexPositionColor(new Vector3(0, 32, 0.5f), Color.Black),
            new VertexPositionColor(new Vector3(0, 0, 0.5f), Color.Black)
        };
        protected float _margin = 2.0f;
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
