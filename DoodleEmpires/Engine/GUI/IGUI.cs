using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoodleEmpires.Engine.GUI
{
    public abstract class IGUI
    {
        /// <summary>
        /// Begins the invalidation of this control
        /// </summary>
        protected abstract void BeginInvalidate();

        /// <summary>
        /// Renders this control
        /// </summary>
        protected abstract void Invalidate();

        /// <summary>
        /// Ends the invalidation of the control
        /// </summary>
        protected abstract void EndInvalidate();

        /// <summary>
        /// Draws this control to the screen
        /// </summary>
        public abstract void Draw();
    }
}
