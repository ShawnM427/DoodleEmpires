/*! 
@file GridRect.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding.cs>
@date July 16, 2013
@brief GridRect Interface
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

An Interface for the GridRect Struct.

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Entities.PathFinder
{
    /// <summary>
    /// A basic implementation of a rectangle class
    /// </summary>
    public struct GridRect
    {
        /// <summary>
        /// The minumum x of this grid rectangle
        /// </summary>
        public int minX;
        /// <summary>
        /// The minumum y of this grid rectangle
        /// </summary>
        public int minY;
        /// <summary>
        /// The maximum x of this grid rectangle
        /// </summary>
        public int maxX;
        /// <summary>
        /// The maximum y of this grid rectangle
        /// </summary>
        public int maxY;

        /// <summary>
        /// Creates a new grid rectangle
        /// </summary>
        /// <param name="iMinX">The minimum x coord</param>
        /// <param name="iMinY">The minimum y cood</param>
        /// <param name="iMaxX">The maximum x coord</param>
        /// <param name="iMaxY">The maxiumum y coord</param>
        public GridRect(int iMinX, int iMinY, int iMaxX, int iMaxY)
        {
            minX = iMinX;
            minY = iMinY;
            maxX = iMaxX;
            maxY = iMaxY;
        }

        /// <summary>
        /// Gets a semi-unique hash cod for this object
        /// </summary>
        /// <returns>A semi-unique integer for this object</returns>
        public override int GetHashCode()
        {
            return minX ^ minY ^ maxX ^ maxY;
        }

        /// <summary>
        /// Checks equality between this object and another
        /// </summary>
        /// <param name="obj">The object to check against</param>
        /// <returns>True if this object is equal to the other object</returns>
        public override bool Equals(System.Object obj)
        {
            if (!(obj is GridRect))
                return false;
            GridRect p = (GridRect)obj;
            // Return true if the fields match:
            return (minX == p.minX) && (minY == p.minY) && (maxX == p.maxX) && (maxY == p.maxY);
        }

        /// <summary>
        /// Checks if this rectangle is equal to another
        /// </summary>
        /// <param name="p">The rectangle to check against</param>
        /// <returns>True if this is equal to <i>p</i></returns>
        public bool Equals(GridRect p)
        {
            // Return true if the fields match:
            return (minX == p.minX) && (minY == p.minY) && (maxX == p.maxX) && (maxY == p.maxY);
        }

        /// <summary>
        /// Checks if two rectangles are equal
        /// </summary>
        /// <param name="a">The first rectangle</param>
        /// <param name="b">The second rectangle</param>
        /// <returns>True if <i>a</i> and <i>b</i> are equal</returns>
        public static bool operator ==(GridRect a, GridRect b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // Return true if the fields match:
            return (a.minX == b.minX) && (a.minY == b.minY) && (a.maxX == b.maxX) && (a.maxY == b.maxY);
        }

        /// <summary>
        /// Checks if two rectangles are inequal
        /// </summary>
        /// <param name="a">The first rectangle</param>
        /// <param name="b">The second rectangle</param>
        /// <returns>True if <i>a</i> and <i>b</i> are not equal</returns>
        public static bool operator !=(GridRect a, GridRect b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Converts an XNA rectangle to a GridRect
        /// </summary>
        /// <param name="xnaRect">The xna rectangle to create a grid rectangle from</param>
        /// <returns>A GridRect with the same values as the XNA rectangle</returns>
        public static implicit operator GridRect(Rectangle xnaRect)
        {
            return new GridRect(xnaRect.Left, xnaRect.Top, xnaRect.Right, xnaRect.Bottom);
        }

        /// <summary>
        /// Sets all values in this rectangle to the given ones
        /// </summary>
        /// <param name="iMinX">The minimum x coord</param>
        /// <param name="iMinY">The minimum y cood</param>
        /// <param name="iMaxX">The maximum x coord</param>
        /// <param name="iMaxY">The maxiumum y coord</param>
        /// <returns>This rectangle with given modifications</returns>
        public GridRect Set(int iMinX, int iMinY, int iMaxX, int iMaxY)
        {
            this.minX = iMinX;
            this.minY = iMinY;
            this.maxX = iMaxX;
            this.maxY = iMaxY;
            return this;
        }
    }
}