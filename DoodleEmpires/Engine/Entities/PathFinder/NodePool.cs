/*! 
@file NodePool.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding.cs>
@date July 16, 2013
@brief NodePool Interface
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

An Interface for the NodePool Class.

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DoodleEmpires.Engine.Entities.PathFinder
{
    /// <summary>
    /// Represents a collection of nodes that internally uses a dictionary
    /// with GridPos as the lookup
    /// </summary>
    public class NodePool
    {
        /// <summary>
        /// The internal dictionary of nodes
        /// </summary>
        protected Dictionary<GridPos, Node> m_nodes;

        /// <summary>
        /// Creates a new node pool
        /// </summary>
        public NodePool()
        {
            m_nodes = new Dictionary<GridPos, Node>();
        }

        /// <summary>
        /// Gets the dictionary of nodes in this pool
        /// </summary>
        public Dictionary<GridPos, Node> Nodes
        {
            get { return m_nodes; }
        }

        /// <summary>
        /// Gets the node at the given x and y coords
        /// </summary>
        /// <param name="iX">The x coord to find</param>
        /// <param name="iY">The y coord to find</param>
        /// <returns>The node at [x, y], or null if none exists</returns>
        public Node GetNode(int iX, int iY)
        {
            GridPos pos = new GridPos(iX, iY);
            return GetNode(pos);
        }
        /// <summary>
        /// Gets the node at the given coords
        /// </summary>
        /// <param name="iPos">The position to get the node at</param>
        /// <returns>The node at <i>iPos</i>, or null if none exists</returns>
        public Node GetNode(GridPos iPos)
        {
            if (m_nodes.ContainsKey(iPos))
                return m_nodes[iPos];
           return null;
        }

        /// <summary>
        /// Sets whether the node at the coords is walkable, or has an unkown state
        /// </summary>
        /// <param name="iX">The x coord to set</param>
        /// <param name="iY">The y coord to set</param>
        /// <param name="iWalkable">True if walkable, or null if unkown</param>
        /// <returns>The node that has been set</returns>
        public Node SetNode(int iX, int iY, bool? iWalkable = null)
        {
            GridPos pos = new GridPos(iX, iY);
            return SetNode(pos,iWalkable);
        }
        /// <summary>
        /// Sets whether the node at the coords is walkable, or has an unkown state
        /// </summary>
        /// <param name="iPos">The coords to set</param>
        /// <param name="iWalkable">True if walkable, or null if unkown</param>
        /// <returns>The node that has been set</returns>
        public Node SetNode(GridPos iPos, bool? iWalkable = null)
        {
            if (iWalkable.HasValue)
            {
                if (iWalkable.Value == true)
                {
                    if (m_nodes.ContainsKey(iPos))
                        return m_nodes[iPos];
                    Node newNode = new Node(iPos.x, iPos.y, iWalkable);
                    m_nodes.Add(iPos, newNode);
                    return newNode;
                }
                else
                {
                    removeNode(iPos);
                }
                
            }
            else
            {
                Node newNode = new Node(iPos.x, iPos.y, true);
                m_nodes.Add(iPos, newNode);
                return newNode;
            }
            return null;
        }

        /// <summary>
        /// Removes a node from this node pool
        /// </summary>
        /// <param name="iX">The x coord to remove at</param>
        /// <param name="iY">The y coord to remove at</param>
        protected void removeNode(int iX, int iY)
        {
            GridPos pos = new GridPos(iX, iY);
            removeNode(pos);
        }
        /// <summary>
        /// Removes a node from this node pool
        /// </summary>
        /// <param name="iPos">The coords to remove at</param>
        protected void removeNode(GridPos iPos)
        {
            if (m_nodes.ContainsKey(iPos))
                m_nodes.Remove(iPos);
        }
    }
}