using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DoodleEmpires.Engine.Terrain;

namespace DoodleEmpires.Engine.Entities.AI
{
    public class BasicAI : AIHandler
    {
        public BasicAI(Entity entity) : base(entity) { }

        public override void Update(VoxelMap map, double elapsedMS)
        {
            Vector2 direction = _entity.Position - _target;
            direction.Normalize();

            _entity.Position += direction * 1.0f;
        }
    }
}
