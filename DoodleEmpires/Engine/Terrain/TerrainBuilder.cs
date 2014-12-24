using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoodleEmpires.Engine.Terrain
{
    /// <summary>
    /// Represents an object that can be used to generate a pixel terrain from a seed value
    /// </summary>
    public abstract class TerrainBuilder
    {
        /// <summary>
        /// The object to use as a seed for this terrain builder
        /// </summary>
        protected object _seed;

        /// <summary>
        /// Creates a new terrainbuilder with the given seed
        /// </summary>
        /// <param name="seed">The object to use as the seed for this builder</param>
        public TerrainBuilder(object seed)
        {
            _seed = seed;
        }

        /// <summary>
        /// Gets the color for the given position
        /// </summary>
        /// <param name="x">The x coord to get the color at</param>
        /// <param name="y">The y coord to get the color at</param>
        /// <returns>The color at [x, y]</returns>
        public virtual Color GetColorAtPosition(int x, int y)
        {
            return Color.White;
        }
    }
}
