using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoodleEmpires.Engine
{
    /// <summary>
    /// A static class holding some random extensions
    /// </summary>
    public static class RandomExtensions
    {
        /// <summary>
        /// Converts this point to a Vector2
        /// </summary>
        /// <param name="point">The point to convert</param>
        /// <returns>A Vector2 containing the point's data</returns>
        public static Vector2 ToVector2(this Point point)
        {
            return new Vector2(point.X, point.Y);
        }
    }
}
