/*! 
@file DynamicGrid.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding.cs>
@date July 16, 2013
@brief DynamicGrid Interface
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

An Interface for the DynamicGrid Class.

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;


namespace DoodleEmpires.Engine.Entities.PathFinder
{
    /// <summary>
    /// A dynamic pathfinding grid
    /// </summary>
    public class DynamicGrid : BaseGrid
    {
        /// <summary>
        /// A dictionary containing all non-null nodes
        /// </summary>
        protected Dictionary<GridPos, Node> m_nodes;
        private bool m_notSet;
        
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
        /// Creates a new dynamic grid
        /// </summary>
        /// <param name="iWalkableGridList">A list of all walkable positions</param>
        public DynamicGrid(List<GridPos> iWalkableGridList = null)
            : base()
        {
            m_gridRect = new GridRect();
            m_gridRect.minX = 0;
            m_gridRect.minY = 0;
            m_gridRect.maxX = 0;
            m_gridRect.maxY = 0;
            m_notSet = true;
            buildNodes(iWalkableGridList);
        }

        /// <summary>
        /// Builds the nodes from a list of walkable points
        /// </summary>
        /// <param name="iWalkableGridList">A list of walkable grid coords</param>
        protected void buildNodes(List<GridPos> iWalkableGridList)
        {

            m_nodes = new Dictionary<GridPos, Node>();
            if (iWalkableGridList == null)
                return;
            foreach (GridPos gridPos in iWalkableGridList)
            {
                SetWalkableAt(gridPos.x, gridPos.y, true);
            }
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

            if (iWalkable)
            {
                if (m_nodes.ContainsKey(pos))
                {
                   // this.m_nodes[pos].walkable = iWalkable;
                    return true;
                }
                else
                {
                    if (iX < m_gridRect.minX || m_notSet)
                        m_gridRect.minX = iX;
                    if (iX > m_gridRect.maxX || m_notSet)
                        m_gridRect.maxX = iX;
                    if (iY < m_gridRect.minY || m_notSet)
                        m_gridRect.minY = iY;
                    if (iY > m_gridRect.maxY || m_notSet)
                        m_gridRect.maxY = iY;
                    m_nodes.Add(new GridPos(pos.x, pos.y), new Node(pos.x, pos.y, iWalkable));
                    m_notSet = false;
                }
            }
            else
            {
                if (m_nodes.ContainsKey(pos))
                {
                    m_nodes.Remove(pos);
                    if (iX == m_gridRect.minX || iX == m_gridRect.maxX || iY == m_gridRect.minY || iY == m_gridRect.maxY)
                        m_notSet = true;
                }
            }
            return true;
        }

        private void setBoundingBox()
        {
            m_notSet = true;
            foreach (KeyValuePair<GridPos, Node> pair in m_nodes)
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
        /// <param name="iPos">The position to get</param>
        /// <returns>The node at the given position</returns>
        public override Node GetNodeAt(GridPos iPos)
        {
            if (m_nodes.ContainsKey(iPos))
            {
                return m_nodes[iPos];
            }
            return null;
        }

        /// <summary>
        /// Gets whether the node at the given co-ordinates is walkable
        /// </summary>
        /// <param name="iPos">The position to check</param>
        /// <returns>True if the node at the position is walkable</returns>
        public override bool IsWalkableAt(GridPos iPos)
        {
            return m_nodes.ContainsKey(iPos);
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
            Reset(null);
        }

        /// <summary>
        /// Resets this grid using a list of walkable points
        /// </summary>
        /// <param name="iWalkableGridList">A list of walkable positions</param>
        public void Reset(List<GridPos> iWalkableGridList)
        {

            foreach (KeyValuePair<GridPos, Node> keyValue in m_nodes)
            {
                keyValue.Value.Reset();
            }

            if (iWalkableGridList == null)
                return;
            foreach (KeyValuePair<GridPos, Node> keyValue in m_nodes)
            {
                if (iWalkableGridList.Contains(keyValue.Key))
                    SetWalkableAt(keyValue.Key, true);
                else
                    SetWalkableAt(keyValue.Key, false);
            }
        }

        /// <summary>
        /// Creates a clone of this grid
        /// </summary>
        /// <returns>A duplicate of this grid</returns>
        public override BaseGrid Clone()
        {
            DynamicGrid tNewGrid = new DynamicGrid(null);

            foreach (KeyValuePair<GridPos, Node> keyValue in m_nodes)
            {
                tNewGrid.SetWalkableAt(keyValue.Key.x, keyValue.Key.y, true);

            }

            return tNewGrid;
        }
    }

}
