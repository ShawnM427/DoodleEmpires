/*! 
@file DynamicGridWPool.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding.cs>
@date July 16, 2013
@brief DynamicGrid with Pool Interface
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

An Interface for the DynamicGrid with Pool Class.

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;


namespace DoodleEmpires.Engine.Entities.PathFinder
{
    /// <summary>
    /// Represents a dynamic search grid that uses a node pool
    /// </summary>
    public class DynamicGridWPool : BaseGrid
    {
         private bool m_notSet;
         private NodePool m_nodePool;

         /// <summary>
         /// Gets the width of this grid
         /// </summary>
        public override int Width
        {
            get
            {
                if (m_notSet)
                    setBoundingBox();
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
                if (m_notSet)
                    setBoundingBox();
                return m_gridRect.maxY - m_gridRect.minY;
            }
            protected set
            {

            }
        }

        /// <summary>
        /// Creates a new dynamic grid with a node pool
        /// </summary>
        /// <param name="iNodePool">The node pool to use</param>
        public DynamicGridWPool(NodePool iNodePool)
            : base()
        {
            m_gridRect = new GridRect();
            m_gridRect.minX = 0;
            m_gridRect.minY = 0;
            m_gridRect.maxX = 0;
            m_gridRect.maxY = 0;
            m_notSet = true;
            m_nodePool = iNodePool;
        }

        private void setBoundingBox()
        {
              foreach (KeyValuePair<GridPos, Node> pair in m_nodePool.Nodes)
            {
                if (pair.Key.x < m_gridRect.minX || m_notSet)
                    m_gridRect.minX = pair.Key.x;
                if (pair.Key.x > m_gridRect.maxX || m_notSet)
                    m_gridRect.maxX = pair.Key.x;
                if (pair.Key.y < m_gridRect.minY || m_notSet)
                    m_gridRect.minY = pair.Key.y;
                if (pair.Key.y > m_gridRect.maxY || m_notSet)
                    m_gridRect.maxY = pair.Key.y;
                m_notSet = false;
            }
            m_notSet = false;
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
            GridPos pos = new GridPos(iX, iY);
            m_nodePool.SetNode(pos, iWalkable);
            if (iWalkable)
            {
                if (iX < m_gridRect.minX || m_notSet)
                    m_gridRect.minX = iX;
                if (iX > m_gridRect.maxX || m_notSet)
                    m_gridRect.maxX = iX;
                if (iY < m_gridRect.minY || m_notSet)
                    m_gridRect.minY = iY;
                if (iY > m_gridRect.maxY || m_notSet)
                    m_gridRect.maxY = iY;
                m_notSet = false;
            }
            else
            {
                if (iX == m_gridRect.minX || iX == m_gridRect.maxX || iY == m_gridRect.minY || iY == m_gridRect.maxY)
                    m_notSet = true;
                
            }
            return true;
        }

        /// <summary>
        /// Gets the node at the given co-ordinates
        /// </summary>
        /// <param name="iPos">The position to get</param>
        /// <returns>The node at the given position</returns>
        public override Node GetNodeAt(GridPos iPos)
        {
            return m_nodePool.GetNode(iPos);
        }
        /// <summary>
        /// Gets whether the node at the given co-ordinates is walkable
        /// </summary>
        /// <param name="iPos">The position to check</param>
        /// <returns>True if the node at the position is walkable</returns>
        public override bool IsWalkableAt(GridPos iPos)
        {
            return  m_nodePool.Nodes.ContainsKey(iPos);
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
            foreach (KeyValuePair<GridPos, Node> keyValue in m_nodePool.Nodes)
            {
                keyValue.Value.Reset();
            }
        }

        /// <summary>
        /// Creates a clone of this grid
        /// </summary>
        /// <returns>A clone of this grid</returns>
        public override BaseGrid Clone()
        {
            DynamicGridWPool tNewGrid = new DynamicGridWPool(m_nodePool);
            return tNewGrid;
        }
    }
}