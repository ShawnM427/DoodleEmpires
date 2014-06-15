using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DoodleEmpires.Engine.Terrain;

namespace DoodleEmpires.Engine.Entities.AI
{
    /// <summary>
    /// The bas class for AI handlers
    /// </summary>
    public abstract class AIHandler
    {
        /// <summary>
        /// The entity to apply this AI handler to
        /// </summary>
        protected Entity _entity;
        /// <summary>
        /// The entities target position
        /// </summary>
        protected Vector2 _target;

        /// <summary>
        /// Gets or sets this AI's target position
        /// </summary>
        public virtual Vector2 Target
        {
            get { return _target; }
            set { _target = value; }
        }

        /// <summary>
        /// Creates a new AI Handler
        /// </summary>
        /// <param name="entity">The entity to handle</param>
        protected AIHandler(Entity entity)
        {
            _entity = entity;
        }

        /// <summary>
        /// Updates this AI handler
        /// </summary>
        /// <param name="map">The current map</param>
        /// <param name="elapsedMS">The time in milliseconds since the last update</param>
        public abstract void Update(VoxelMap map, double elapsedMS);
    }
}
