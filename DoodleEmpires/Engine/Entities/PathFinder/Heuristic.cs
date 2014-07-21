﻿/*! 
@file Heuristic.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding.cs>
@date July 16, 2013
@brief Heuristic Function Interface
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

An Interface for the Heuristic Function Class.

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoodleEmpires.Engine.Entities.PathFinder
{
    /// <summary>
    /// The Heuristic to use to find the length of the path
    /// </summary>
    public enum HeuristicMode
    {
        /// <summary>
        /// Simply sums the x and y components
        /// </summary>
        MANHATTAN,
        /// <summary>
        /// Uses a square root to measure accurate distance
        /// </summary>
        EUCLIDEAN,
        /// <summary>
        /// Gets the maximum of the x or y
        /// </summary>
        CHEBYSHEV,
        
    };

    /// <summary>
    /// Implementations of heuristic methods
    /// </summary>
    public class Heuristic
    {
        /// <summary>
        /// Find manhattan distance
        /// </summary>
        /// <param name="iDx">The x coord</param>
        /// <param name="iDy">The y coord</param>
        /// <returns>A relative distance</returns>
        public static float Manhattan(int iDx, int iDy)
        {
            return (float)iDx + iDy;
        }

        /// <summary>
        /// Find euclidean distance
        /// </summary>
        /// <param name="iDx">The x coord</param>
        /// <param name="iDy">The y coord</param>
        /// <returns>A relative distance</returns>
        public static float Euclidean(int iDx, int iDy)
        {
            float tFdx = (float)iDx;
            float tFdy = (float)iDy;
            return (float)Math.Sqrt((double)(tFdx * tFdx + tFdy * tFdy));
        }

        /// <summary>
        /// Find Chebyshev distance
        /// </summary>
        /// <param name="iDx">The x coord</param>
        /// <param name="iDy">The y coord</param>
        /// <returns>A relative distance</returns>
        public static float Chebyshev(int iDx, int iDy)
        {
            return (float)Math.Max(iDx, iDy);
        }
    }
}