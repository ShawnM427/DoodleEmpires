using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using DoodleEmpires.Engine.Utilities;

namespace DoodleEmpires.Engine.Entities
{
    /// <summary>
    /// A basic controller for a 2D camera
    /// </summary>
    public class CameraControl : IFocusable
    {
        const Keys MOVELEFT = Keys.A;
        const Keys MOVERIGHT = Keys.D;
        const Keys MOVEUP = Keys.W;
        const Keys MOVEDOWN = Keys.S;
        const float SPEED = 4.0F;

        /// <summary>
        /// The current target position
        /// </summary>
        protected Vector2 _position = Vector2.Zero;
        /// <summary>
        /// The bounds that the controller must stay within
        /// </summary>
        protected Rectangle _bounds = new Rectangle(0, 0, 800, 400);

        /// <summary>
        /// Gets or sets the camera controller's position
        /// </summary>
        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        /// <summary>
        /// Creates a new camera controller
        /// </summary>
        /// <param name="camera">The camera to control</param>
        public CameraControl(ICamera2D camera)
        {
            camera.Focus = this;
            _bounds = camera.ScreenBounds;
        }

        KeyboardState _ks;
        /// <summary>
        /// Updates this camera controller
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        public void Update(GameTime gameTime)
        {
            _ks = Keyboard.GetState();

            _position.X += (_ks.IsKeyDown(MOVELEFT) ? -SPEED : _ks.IsKeyDown(MOVERIGHT) ? SPEED : 0) * FPSManager.Multiplier;
            _position.Y += (_ks.IsKeyDown(MOVEUP) ? -SPEED : _ks.IsKeyDown(MOVEDOWN) ? SPEED : 0) * FPSManager.Multiplier;
        }
    }
}
