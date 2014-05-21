using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DoodleEmpires.Engine.Terrain;

namespace DoodleEmpires.Engine.Entities.AI
{
    public abstract class AIHandler
    {
        protected Entity _entity;
        protected Vector2 _target;

        public virtual Vector2 Target
        {
            get { return _target; }
            set { _target = value; }
        }

        public AIHandler(Entity entity)
        {
            _entity = entity;
        }

        public abstract void Update(VoxelMap map, double elapsedMS);
    }
}
