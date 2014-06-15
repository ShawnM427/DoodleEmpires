/*! 
@file PartialGridWPool.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding.cs>
@date July 16, 2013
@brief PartialGrid with Pool Interface
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

An Interface for the PartialGrid with Pool Class.

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using DoodleEmpires.Engine.Entities.PathFinder.Utils;

namespace DoodleEmpires.Engine.Entities.PathFinder
{
    /// <summary>
    /// Represents a search grid that uses a node pool
    /// </summary>
    public class PartialGridWPool : BaseGrid
    {
        private NodePool m_nodePool;

        /// <summary>
        /// Gets the width of this grid
        /// </summary>
        public override int Width
        {
            get
            {
                return m_gridRect.maxX - m_gridRect.minX;
            }
            protected set
            {

            }
        }

        /// <summary>
        /// Gets the height of this grid
        /// </summary>
        public override int Height
        {
            get
            {
                return m_gridRect.maxY - m_gridRect.minY;
            }
            protected set
            {

            }
        }

        /// <summary>
        /// Creates a new partial grid with a node pool
        /// </summary>
        /// <param name="iNodePool">The node pool to use</param>
        /// <param name="iGridRect">The rectangle representing the grids bounds</param>
        public PartialGridWPool(NodePool iNodePool, GridRect? iGridRect = null)
            : base()
        {
            if (iGridRect == null)
                m_gridRect = new GridRect();
            else
                m_gridRect = iGridRect.Value;
            m_nodePool = iNodePool;
        }

       /// <summary>
       /// Sets the grids bounds
       /// </summary>
       /// <param name="iGridRect">The new bounds to use</param>
        public void SetGridRect(GridRect iGridRect)
        {
            m_gridRect = iGridRect;
        }

        /// <summary>
        /// Checks if a point is within this grid's bounds
        /// </summary>
        /// <param name="iX">The x coord to check</param>
        /// <param name="iY">The y coord to check</param>
        /// <returns>True if [iX, iY] is inside this grid</returns>
        public bool IsInside(int iX, int iY)
        {
            if (iX < m_gridRect.minX || iX > m_gridRect.maxX || iY < m_gridRect.minY || iY > m_gridRect.maxY)
                return false;
            return true;
        }
        /// <summary>
        /// Checks if a point is within this grid's bounds
        /// </summary>
        /// <param name="iPos">The position to check</param>
        /// <returns>True if <i>iPos</i> is inside this grid</returns>
        public bool IsInside(GridPos iPos)
        {
            return IsInside(iPos.x, iPos.y);
        }

        /// <summary>
        /// Gets the node at the given co-ordinates
        /// </summary>
        /// <param name="iX">The x coord to get</param>
        /// <param name="iY">The y coord to get</param>
        /// <returns>The node at the given position</returns>
        public override Node GetNodeAt(int iX, int iY)
        {
            GridPos pos = new GridPos(iX, iY);
            return GetNodeAt(pos);
        }
        /// <summary>
        /// Gets whether the node at the given co-ordinates is walkable
        /// </summary>
        /// <param name="iX">The x coord to get</param>
        /// <param name="iY">The y coord to get</param>
        /// <returns>True if the node at the position is walkable</returns>
        public override bool IsWalkableAt(int iX, int iY)
        {
            GridPos pos = new GridPos(iX, iY);
            return IsWalkableAt(pos);
        }
        /// <summary>
        /// Sets whether the node at the given co-ordinates is walkable
        /// </summary>
        /// <param name="iX">The x coord to set</param>
        /// <param name="iY">The y coord to set</param>
        /// <param name="iWalkable">Whether to node is walkable</param>
        /// <returns>The sucess of the operation</returns>
        public override bool SetWalkableAt(int iX, int iY, bool iWalkable)
        {
            if (!IsInside(iX,iY))
                return false;
            GridPos pos = new GridPos(iX, iY);
            m_nodePool.SetNode(pos, iWalkable);
            return true;
        }

        /// <summary>
        /// Gets the node at the given co-ordinates
        /// </summary>
        /// <param name="iPos">The position to get</param>
        /// <returns>The node at the given position</returns>
        public override Node GetNodeAt(GridPos iPos)
        {
            if (!IsInside(iPos))
                return null;
            return m_nodePool.GetNode(iPos);
        }
        /// <summary>
        /// Gets whether the node at the given co-ordinates is walkable
        /// </summary>
        /// <param name="iPos">The position to check</param>
        /// <returns>True if the node at the position is walkable</returns>
        public override bool IsWalkableAt(GridPos iPos)
        {
            if (!IsInside(iPos))
                return false;
            return m_nodePool.Nodes.ContainsKey(iPos);
        }
        /// <summary>
        /// Sets whether the node at the given co-ordinates is walkable
        /// </summary>
        /// <param name="iPos">The position to set</param>
        /// <param name="iWalkable">Whether the node at the position is walkable</param>
        /// <returns>The sucess of the operation</returns>
        public override bool SetWalkableAt(GridPos iPos, bool iWalkable)
        {
            return SetWalkableAt(iPos.x, iPos.y, iWalkable);
        }

        /// <summary>
        /// Resets this grid
        /// </summary>
        public override void Reset()
        {
            int rectCount=(m_gridRect.maxX-m_gridRect.minX) * (m_gridRect.maxY-m_gridRect.minY);
            if (m_nodePool.Nodes.Count > rectCount)
            {
                GridPos travPos = new GridPos(0, 0);
                for (int xTrav = m_gridRect.minX; xTrav <= m_gridRect.maxX; xTrav++)
                {
                    travPos.x = xTrav;
                    for (int yTrav = m_gridRect.minY; yTrav <= m_gridRect.maxY; yTrav++)
                    {
                        travPos.y = yTrav;
                        Node curNode=m_nodePool.GetNode(travPos);
                        if (curNode!=null)
                            curNode.Reset();
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<GridPos, Node> keyValue in m_nodePool.Nodes)
                {
                    keyValue.Value.Reset();
                }
            }
        }

        /// <summary>
        /// Creates a clone of this grid
        /// </summary>
        /// <returns>A clone of this grid</returns>
        public override BaseGrid Clone()
        {
            PartialGridWPool tNewGrid = new PartialGridWPool(m_nodePool,m_gridRect);
            return tNewGrid;
        }
    }

}