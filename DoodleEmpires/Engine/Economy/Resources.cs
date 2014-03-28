using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoodleEmpires.Engine.Economy
{
    /// <summary>
    /// Represents an amount of resources
    /// </summary>
    public struct Resources
    {
        /// <summary>
        /// The amount of food in this resource pile
        /// </summary>
        public float Food { get; set; }
        /// <summary>
        /// The amount of wood in this resource pile
        /// </summary>
        public float Wood { get; set; }
        /// <summary>
        /// The amount of gold in this resource pile
        /// </summary>
        public float Gold { get; set; }
        /// <summary>
        /// The amount of dirt in this resource pile
        /// </summary>
        public float Dirt { get; set; }
        /// <summary>
        /// The amount of stone in this resource pile
        /// </summary>
        public float Stone { get; set; }
        /// <summary>
        /// The amount of coal in this resource pile
        /// </summary>
        public float Coal { get; set; }
        /// <summary>
        /// The amount of iron in this resource pile
        /// </summary>
        public float Limestone { get; set; }
        /// <summary>
        /// The amount of limestone in this resource pile
        /// </summary>
        public float Iron { get; set; }
        /// <summary>
        /// The amount of uranium in this resource pile
        /// </summary>
        public float Uranium { get; set; }
        /// <summary>
        /// The amount of doodolium in this resource pile
        /// </summary>
        public float Doodolium { get; set; }
    }
}
