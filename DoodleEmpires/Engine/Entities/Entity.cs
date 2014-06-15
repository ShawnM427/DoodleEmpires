using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Entities
{
    /// <summary>
    /// The base class for all world entities
    /// </summary>
    public abstract class Entity
    {
        Rectangle _bounds;
        Vector2 _position;
        //int _textureID;

        /// <summary>
        /// Gets or sets the position of this entity
        /// </summary>
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;

                _bounds.X = (int)(_position.X - _bounds.Width / 2);
                _bounds.Y = (int)(_position.Y - _bounds.Height);
            }
        }

        /// <summary>
        /// Creates a new entity
        /// </summary>
        public Entity()
        {
        }
    }
}
