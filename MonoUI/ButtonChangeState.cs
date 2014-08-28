using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoUI
{
    /// <summary>
    /// Represents how the state of a button has changed since the previos check
    /// </summary>
    public enum ButtonChangeState
    {
        /// <summary>
        /// No change of state has occured
        /// </summary>
        None = 0,
        /// <summary>
        /// The button has been pressed since the last check
        /// </summary>
        Pressed = 1,
        /// <summary>
        /// The button has been released since the last check
        /// </summary>
        Released = 2
    }
}
