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
    /// Represents a scrollable GUI container
    /// <b>STILL A W.I.P</b>
    /// </summary>
    public class GUIScrollPanel : GUIContainer
    {
        /// <summary>
        /// The render target for the scroll bar
        /// </summary>
        protected RenderTarget2D _scrollBar;
        /// <summary>
        /// The render target for the scroll bar housing
        /// </summary>
        protected RenderTarget2D _scrollHousing;
        /// <summary>
        /// The render target for the internal components
        /// </summary>
        protected RenderTarget2D _internalTarget;
        /// <summary>
        /// The width of the scroll bar
        /// </summary>
        protected int _scrollSize = 8;
        /// <summary>
        /// The rectangle representing the scroll housing
        /// </summary>
        protected Rectangle _scrollHousingRect;

        /// <summary>
        /// How far down this panel is scrolled
        /// </summary>
        protected float _scrollValue = 0.0f;

        /// <summary>
        /// Creates a new GUI scroll panel
        /// </summary>
        /// <param name="graphics">The graphics device to bind to</param>
        /// <param name="parent">The parent container</param>
        public GUIScrollPanel(GraphicsDevice graphics, GUIContainer parent) : base(graphics, parent) { }

        /// <summary>
        /// Called when this scroll panel has been resized
        /// </summary>
        protected override void Resized()
        {
            RenderScrollBar();

            if (_bounds.Width != 0 && _bounds.Height != 0)
                _internalTarget = new RenderTarget2D(_graphics, _bounds.Width, _bounds.Height);

            base.Resized();
        }

        /// <summary>
        /// Renders the scroll bar
        /// </summary>
        protected virtual void RenderScrollBar()
        {
            _scrollHousingRect = new Rectangle(_bounds.Width - _scrollSize, 0, _scrollSize, _bounds.Height);

            _scrollHousing = new RenderTarget2D(_graphics, _scrollSize, _bounds.Height);
            _graphics.SetRenderTarget(_scrollHousing);
            _graphics.Clear(Color.Transparent);
            _effect.Projection = Matrix.CreateOrthographicOffCenter(0, _scrollHousingRect.Width, _scrollHousingRect.Height, 0, 0.0f, 1000.0f);

            _effect.CurrentTechnique.Passes[0].Apply();
            _graphics.DrawRect(0,0, _scrollSize, _bounds.Height, Color.Black);
            _graphics.DrawLine(0, _scrollSize, _scrollSize, _scrollSize, Color.Black);
            _graphics.DrawLine(0, _bounds.Height - _scrollSize, _scrollSize, _bounds.Height - _scrollSize, Color.Black);

            _effect.Projection = Matrix.CreateOrthographicOffCenter(0, _bounds.Width, _bounds.Height, 0, 1.0f, 1000.0f);
        }

        /// <summary>
        /// Begins invalidation on this control
        /// </summary>
        /// <returns></returns>
        protected override bool BeginInvalidate()
        {
            if (_screenBounds.Width > 0 && _screenBounds.Height > 0)
            {
                if (_renderTarget != null)
                {
                    _renderTarget.Dispose();
                    _renderTarget = new RenderTarget2D(_graphics, _screenBounds.Width, _screenBounds.Height);
                }
                if (_internalTarget != null)
                {
                    _internalTarget.Dispose();
                    _internalTarget = new RenderTarget2D(_graphics, _screenBounds.Width, _screenBounds.Height);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Called when this control invalidates
        /// </summary>
        protected override void Invalidate()
        {
        }

        /// <summary>
        /// Ends invalidation on this control
        /// </summary>
        protected override void EndInvalidate()
        {
            _graphics.SetRenderTarget(_internalTarget);
            _graphics.Clear(_backColor);
            _spriteBatch.Begin();

            foreach (IGUI control in _controls)
            {
                if (control != null && control.Image != null)
                    _spriteBatch.Draw(control.Image, control.Bounds, Color.White);
            }

            _spriteBatch.End();
            _graphics.SetRenderTarget(null);

            _graphics.SetRenderTarget(_renderTarget);
            _graphics.Clear(_backColor);

            _spriteBatch.Begin();
            _spriteBatch.Draw(_internalTarget, new Vector2(0, _scrollValue), _colorMultiplier);
            _spriteBatch.Draw(_scrollHousing, _scrollHousingRect, _colorMultiplier);
            _spriteBatch.End();

            _effect.CurrentTechnique.Passes[0].Apply();
            _graphics.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, _cornerVerts, 0, 4);
            _graphics.SetRenderTarget(null);
        }

        /// <summary>
        /// Called when this control is clicked, returns true if the mouse input was handled
        /// </summary>
        /// <param name="e">The mouse event arguments</param>
        /// <returns>True if the input was handled</returns>
        public override void MousePressed(MouseEventArgs e)
        {
            base.MousePressed(e);

            Vector2 mouseP = new Vector2(e.X - _screenBounds.X, e.Y - _screenBounds.Y);
            if (e.LeftButton == ButtonChangeState.Pressed && _scrollHousingRect.Contains(mouseP))
            {
                if (mouseP.Y < _scrollSize)
                    _scrollValue -= 1.0f;

                else if (mouseP.Y > _bounds.Height - _scrollSize)
                    _scrollValue += 1.0f;

                _scrollValue = _scrollValue < 0 ? 0 : _scrollValue;

                Invalidating = true;
            }
        }
    }
}
