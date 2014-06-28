using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Entities
{
    /// <summary>
    /// Represents an object which a 2D camera can focus on
    /// </summary>
    public interface IFocusable
    {
        /// <summary>
        /// Gets the position of the focusable object
        /// </summary>
        Vector2 Position { get; }
    }
}
