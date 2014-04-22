using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using DoodleEmpires.Engine.Utilities;

namespace DoodleEmpires.Engine.Entities
{
    public class CameraControl : IFocusable
    {
        const Keys MOVELEFT = Keys.A;
        const Keys MOVERIGHT = Keys.D;
        const Keys MOVEUP = Keys.W;
        const Keys MOVEDOWN = Keys.S;
        const float SPEED = 4.0F;

        protected Vector2 _position = Vector2.Zero;
        protected Rectangle _bounds = new Rectangle(0, 0, 800, 400);

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public CameraControl(ICamera2D camera)
        {
            _bounds = camera.ScreenBounds;
        }

        KeyboardState _ks;
        public void Update(GameTime gameTime)
        {
            _ks = Keyboard.GetState();

            _position.X += (_ks.IsKeyDown(MOVELEFT) ? -SPEED : _ks.IsKeyDown(MOVERIGHT) ? SPEED : 0) * FPSManager.Multiplier;
            _position.Y += (_ks.IsKeyDown(MOVEUP) ? -SPEED : _ks.IsKeyDown(MOVEDOWN) ? SPEED : 0) * FPSManager.Multiplier;
        }
    }
}
