using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoodleEmpires.Engine.Utilities;

namespace DoodleEmpires.Engine.Entities
{
    /// <summary>
    /// Represents an entity manager
    /// </summary> 
    public class EntityManager : ObjectManager<Entity> 
    {
        /// <summary>
        /// Creates a new entity manager
        /// </summary>
        public EntityManager() : base() { }
    }
}
