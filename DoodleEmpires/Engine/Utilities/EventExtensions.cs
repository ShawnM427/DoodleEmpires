using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoUI;

namespace DoodleEmpires.Engine.Utilities
{
    /// <summary>
    /// A class for event extensions
    /// </summary>
    public static class EventExtensions
    {
        /// <summary>
        /// Handles raising an event
        /// </summary>
        /// <typeparam name="T">The type of the event handler</typeparam>
        /// <param name="handler">The event handler to use</param>
        /// <param name="sender">The sending object</param>
        /// <param name="args">The arguments to use</param>
        public static void Raise<T>(this EventHandler<T> handler, object sender,  T args) where T : EventArgs
        {
            if (handler != null) 
                handler(sender, args);
        }
    }
}
