/*! 
@file GridPos.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding.cs>
@date July 16, 2013
@brief Grid Position Interface
@version 2.0

@section LICENSE

The MIT License (MIT)

Copyright (c) 2013 Woong Gyu La <juhgiyo@gmail.com>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

@section DESCRIPTION

An Interface for the Grid Position Struct.

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleEmpires.Engine.Entities.PathFinder
{
    /// <summary>
    /// Represents a position within a grid
    /// </summary>
    public struct GridPos
    {
        /// <summary>
        /// The x coord of this position
        /// </summary>
        public int x;
        /// <summary>
        /// The y coord of this position
        /// </summary>
        public int y;

        /// <summary>
        /// Creates a new grid position
        /// </summary>
        /// <param name="iX">The x coord of the position</param>
        /// <param name="iY">The y coord of the position</param>
        public GridPos(int iX, int iY)
        {
            this.x = iX;
            this.y = iY;
        }

        /// <summary>
        /// Gets a semi-unique hash code for this position
        /// </summary>
        /// <returns>A semi-unique integer</returns>
        public override int GetHashCode()
        {
            return x ^ y;
        }

        /// <summary>
        /// Checks if this position is equal to another object
        /// </summary>
        /// <param name="obj">The object to check against</param>
        /// <returns>True if this is equal to <i>obj</i></returns>
        public override bool Equals(System.Object obj)
        {
            if (!(obj is GridPos))
                return false;
            GridPos p = (GridPos)obj;
            // Return true if the fields match:
            return (x == p.x) && (y == p.y);
        }

        /// <summary>
        /// Checks if this position is equal to another position
        /// </summary>
        /// <param name="p">The position to check against</param>
        /// <returns>True if this is equal to <i>p</i></returns>
        public bool Equals(GridPos p)
        {
            // Return true if the fields match:
            return (x == p.x) && (y == p.y);
        }

        /// <summary>
        /// Checks if two grid positions are equal
        /// </summary>
        /// <param name="a">The first position</param>
        /// <param name="b">The second position</param>
        /// <returns>True if <i>a</i> and <i>b</i> are equal</returns>
        public static bool operator ==(GridPos a, GridPos b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // Return true if the fields match:
            return a.x == b.x && a.y == b.y;
        }

        /// <summary>
        /// Checks if two grid positions are inequal
        /// </summary>
        /// <param name="a">The first position</param>
        /// <param name="b">The second position</param>
        /// <returns>True if <i>a</i> and <i>b</i> are not equal</returns>
        public static bool operator !=(GridPos a, GridPos b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Sets this position's values to those given
        /// </summary>
        /// <param name="iX">The x coord of the position</param>
        /// <param name="iY">The y coord of the position</param>
        /// <returns>This position, with the modified values</returns>
        public GridPos Set(int iX, int iY)
        {
            this.x = iX;
            this.y = iY;
            return this;
        }
    }
}
