using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DoodleEmpires.Engine.Terrain;

namespace DoodleEmpires.Engine.Entities.AI
{
    /// <summary>
    /// A basic AI handler
    /// </summary>
    public class BasicAI : AIHandler
    {
        /// <summary>
        /// Creates a new basic AI handler
        /// </summary>
        /// <param name="entity">The entity to handle</param>
        public BasicAI(Entity entity) : base(entity) { }

        /// <summary>
        /// Updates this AI handler
        /// </summary>
        /// <param name="map">The current map</param>
        /// <param name="elapsedMS">The time in milliseconds since the last update</param>
        public override void Update(VoxelMap map, double elapsedMS)
        {
            Vector2 direction = _entity.Position - _target;
            direction.Normalize();

            _entity.Position += direction * 1.0f;
        }
    }
}
