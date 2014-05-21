using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Entities
{
    public class Entity
    {
        Rectangle _bounds;
        Vector2 _position;
        int _textureID;

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Entity()
        {
        }
    }
}
