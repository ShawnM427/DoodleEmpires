using DoodleEmpires.Engine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmileyFaceWars
{
    public class PlayerInstance : IFocusable
    {
        Vector2 _position;

        public Vector2 Position
        {
            get { return _position; }
        }

        public PlayerInstance(Vector2 position)
        {
            _position = position;
        }
    }
}
