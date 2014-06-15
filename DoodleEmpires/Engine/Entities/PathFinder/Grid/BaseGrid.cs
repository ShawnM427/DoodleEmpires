/*! 
@file BaseGrid.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding.cs>
@date July 16, 2013
@brief BaseGrid Interface
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

An Interface for the BaseGrid Class.

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DoodleEmpires.Engine.Entities.PathFinder
{
    /// <summary>
    /// Represents a node in a pathfinding grid
    /// </summary>
    public class Node : IComparable
    {
        /// <summary>
        /// The x coord of this node
        /// </summary>
        public int x;
        /// <summary>
        /// The y coord of this node
        /// </summary>
        public int y;
        /// <summary>
        /// Whether or not the node is walkable
        /// </summary>
        public bool walkable;
        /// <summary>
        /// The heuristics-based length from this node to the end node
        /// </summary>
        public float heuristicStartToEndLen;
        /// <summary>
        /// The length from this node to the end node
        /// </summary>
        public float startToCurNodeLen;
        /// <summary>
        /// The length from this node to the goal node
        /// </summary>
        public float? heuristicCurNodeToEndLen;
        /// <summary>
        /// Whether this node has been opened
        /// </summary>
        public bool isOpened;
        /// <summary>
        /// Whether this node has been closed
        /// </summary>
        public bool isClosed;
        /// <summary>
        /// The parent node
        /// </summary>
        public Object parent;

        /// <summary>
        /// Creates a new pathfinding node
        /// </summary>
        /// <param name="iX">The x coord of this node</param>
        /// <param name="iY">The y coord of this node</param>
        /// <param name="iWalkable">True if this node is walkable</param>
        public Node(int iX, int iY, bool? iWalkable = null)
        {
            this.x = iX;
            this.y = iY;
            this.walkable = iWalkable.HasValue ? iWalkable.Value : false;
            this.heuristicStartToEndLen = 0;
            this.startToCurNodeLen = 0;
            this.heuristicCurNodeToEndLen = null;
            this.isOpened = false;
            this.isClosed = false;
            this.parent = null;

        }

        /// <summary>
        /// Resets this node
        /// </summary>
        /// <param name="iWalkable">Whether or not this not is walkable</param>
        public void Reset(bool? iWalkable = null)
        {
            if (iWalkable.HasValue)
                walkable = iWalkable.Value;
            this.heuristicStartToEndLen = 0;
            this.startToCurNodeLen = 0;
            this.heuristicCurNodeToEndLen = null;
            this.isOpened = false;
            this.isClosed = false;
            this.parent = null;
        }
        
        /// <summary>
        /// Compares this object to another for sorting
        /// </summary>
        /// <param name="iObj">The object to compare to</param>
        /// <returns>A value representing which instance is a shorter distance</returns>
        public int CompareTo(object iObj)
        {
            Node tOtherNode = (Node)iObj;
            float result=this.heuristicStartToEndLen - tOtherNode.heuristicStartToEndLen;
            if (result > 0.0f)
                return 1;
            else if (result == 0.0f)
                return 0;
            return -1;
        }

        /// <summary>
        /// Perfoms a back tracing algorithm on a node
        /// </summary>
        /// <param name="iNode">The node to trace</param>
        /// <returns>A list of grid positions that represents the path to the node</returns>
        public static List<GridPos> Backtrace(Node iNode)
        {
            List<GridPos> path = new List<GridPos>();
            path.Add(new GridPos(iNode.x, iNode.y));
            while (iNode.parent != null)
            {
                iNode = (Node)iNode.parent;
                path.Add(new GridPos(iNode.x, iNode.y));
            }
            path.Reverse();
            return path;
        }
        
        /// <summary>
        /// Gets a hash code for this node
        /// </summary>
        /// <returns>A unique hash code for this node</returns>
        public override int GetHashCode()
        {
            return x ^ y;
        }

        /// <summary>
        /// Checks if this node is equal to another object
        /// </summary>
        /// <param name="obj">The object to check equality against</param>
        /// <returns>True if this item is equal to the given object</returns>
        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            Node p = obj as Node;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (x == p.x) && (y == p.y);
        }

        /// <summary>
        /// Checks if this node is equal to another node
        /// </summary>
        /// <param name="p">The node to check against</param>
        /// <returns>True if these nodes are equal</returns>
        public bool Equals(Node p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (x == p.x) && (y == p.y);
        }

        /// <summary>
        /// Checks if two nodes are equal
        /// </summary>
        /// <param name="a">The first node to check</param>
        /// <param name="b">The second node to check</param>
        /// <returns>True if these nodes are equal</returns>
        public static bool operator ==(Node a, Node b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.x == b.x && a.y == b.y;
        }

        /// <summary>
        /// Checks if two nodes are not equal
        /// </summary>
        /// <param name="a">The first node to check</param>
        /// <param name="b">The second node to check</param>
        /// <returns>True if these nodes are not equal</returns>
        public static bool operator !=(Node a, Node b)
        {
            return !(a == b);
        }
    }

    /// <summary>
    /// The base class for node grids
    /// </summary>
    public abstract class BaseGrid
    {
        /// <summary>
        /// Creates a new base grid
        /// </summary>
        public BaseGrid()
        {
        }

        /// <summary>
        /// The grid rectangle for this grid
        /// </summary>
        protected GridRect m_gridRect;
        /// <summary>
        /// Gets the grid rectangle for this grid
        /// </summary>
        public GridRect gridRect
        {
            get { return m_gridRect; }
        }

        /// <summary>
        /// Gets the width of this grid
        /// </summary>
        public abstract int Width { get; protected set; }

        /// <summary>
        /// Gets the height of this grid
        /// </summary>
        public abstract int Height { get; protected set; }

        /// <summary>
        /// Gets the node at the given co-ordinates
        /// </summary>
        /// <param name="iX">The x coord to get</param>
        /// <param name="iY">The y coord to get</param>
        /// <returns>The node at the given position</returns>
        public abstract Node GetNodeAt(int iX, int iY);
        /// <summary>
        /// Gets whether the node at the given co-ordinates is walkable
        /// </summary>
        /// <param name="iX">The x coord to get</param>
        /// <param name="iY">The y coord to get</param>
        /// <returns>True if the node at the position is walkable</returns>
        public abstract bool IsWalkableAt(int iX, int iY);
        /// <summary>
        /// Sets whether the node at the given co-ordinates is walkable
        /// </summary>
        /// <param name="iX">The x coord to set</param>
        /// <param name="iY">The y coord to set</param>
        /// <param name="iWalkable">Whether to node is walkable</param>
        /// <returns>The sucess of the operation</returns>
        public abstract bool SetWalkableAt(int iX, int iY, bool iWalkable);

        /// <summary>
        /// Gets the node at the given co-ordinates
        /// </summary>
        /// <param name="iPos">The position to get</param>
        /// <returns>The node at the given position</returns>
        public abstract Node GetNodeAt(GridPos iPos);
        /// <summary>
        /// Gets whether the node at the given co-ordinates is walkable
        /// </summary>
        /// <param name="iPos">The position to check</param>
        /// <returns>True if the node at the position is walkable</returns>
        public abstract bool IsWalkableAt(GridPos iPos);
        /// <summary>
        /// Sets whether the node at the given co-ordinates is walkable
        /// </summary>
        /// <param name="iPos">The position to set</param>
        /// <param name="iWalkable">Whether the node at the position is walkable</param>
        /// <returns>The sucess of the operation</returns>
        public abstract bool SetWalkableAt(GridPos iPos, bool iWalkable);

        /// <summary>
        /// Gets the neighbors of a given node
        /// </summary>
        /// <param name="iNode">The node to get the neighbors for</param>
        /// <param name="iCrossCorners">True if corners should be counted</param>
        /// <param name="iCrossAdjacentPoint">True if one adjacent side must be free to enter a corner</param>
        /// <returns>The node's neighbors</returns>
        public List<Node> GetNeighbors(Node iNode, bool iCrossCorners, bool iCrossAdjacentPoint)
        {
            int tX = iNode.x;
            int tY = iNode.y;
            List<Node> neighbors = new List<Node>();
            bool tS0 = false, tD0 = false,
                tS1 = false, tD1 = false,
                tS2 = false, tD2 = false,
                tS3 = false, tD3 = false;

            GridPos pos = new GridPos();
            if (this.IsWalkableAt(pos.Set(tX, tY - 1)))
            {
                neighbors.Add(GetNodeAt(pos));
                tS0 = true;
            }
            if (this.IsWalkableAt(pos.Set(tX + 1, tY)))
            {
                neighbors.Add(GetNodeAt(pos));
                tS1 = true;
            }
            if (this.IsWalkableAt(pos.Set(tX, tY + 1)))
            {
                neighbors.Add(GetNodeAt(pos));
                tS2 = true;
            }
            if (this.IsWalkableAt(pos.Set(tX - 1, tY)))
            {
                neighbors.Add(GetNodeAt(pos));
                tS3 = true;
            }
            if (iCrossCorners && iCrossAdjacentPoint)
            {
                tD0 = true;
                tD1 = true;
                tD2 = true;
                tD3 = true;
            }
            else if (iCrossCorners)
            {
                tD0 = tS3 || tS0;
                tD1 = tS0 || tS1;
                tD2 = tS1 || tS2;
                tD3 = tS2 || tS3;
            }
            else
            {
                tD0 = tS3 && tS0;
                tD1 = tS0 && tS1;
                tD2 = tS1 && tS2;
                tD3 = tS2 && tS3;
            }

            if (tD0 && this.IsWalkableAt(pos.Set(tX - 1, tY - 1)))
            {
                neighbors.Add(GetNodeAt(pos));
            }
            if (tD1 && this.IsWalkableAt(pos.Set(tX + 1, tY - 1)))
            {
                neighbors.Add(GetNodeAt(pos));
            }
            if (tD2 && this.IsWalkableAt(pos.Set(tX + 1, tY + 1)))
            {
                neighbors.Add(GetNodeAt(pos));
            }
            if (tD3 && this.IsWalkableAt(pos.Set(tX - 1, tY + 1)))
            {
                neighbors.Add(GetNodeAt(pos));
            }
            return neighbors;
        }

        /// <summary>
        /// Resets this grid
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Creates a clone of this grid
        /// </summary>
        /// <returns>A clone of this grid</returns>
        public abstract BaseGrid Clone();

    }
}