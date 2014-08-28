using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoUI
{
    /// <summary>
    /// Provides a snapshot of mouse values
    /// </summary>
    public struct MouseEventArgs
    {
        /// <summary>
        /// The x-coordinate of the mouse relative to the top-left corner of the window
        /// </summary>
        public int X;
        /// <summary>
        /// The y-coordinate of the mouse relative to the top-left corner of the window
        /// </summary>
        public int Y;
        /// <summary>
        /// The state of the left mouse button relative to it's previous state
        /// </summary>
        public ButtonChangeState LeftButton;
        /// <summary>
        /// The state of the middle mouse button relative to it's previous state
        /// </summary>
        public ButtonChangeState MiddleButton;
        /// <summary>
        /// The state of the right button relative to it's previous state
        /// </summary>
        public ButtonChangeState RightButton;
        /// <summary>
        /// The position of the mouse relative to the top left corner of the window
        /// </summary>
        public Vector2 Position
        {
            get { return new Vector2(X, Y); }
        }

        /// <summary>
        /// Creates a new mouse event argument
        /// </summary>
        /// <param name="x">The x position of the mouse</param>
        /// <param name="y">The y position of the mouse</param>
        /// <param name="left">The change in state of the left mouse button</param>
        /// <param name="middle">The change in state of the middle mouse button</param>
        /// <param name="right">The change in state of the right mouse button</param>
        public MouseEventArgs(int x, int y, ButtonChangeState left, ButtonChangeState middle, ButtonChangeState right)
        {
            X = x;
            Y = y;
            LeftButton = left;
            MiddleButton = middle;
            RightButton = right;
        }
    }
}
