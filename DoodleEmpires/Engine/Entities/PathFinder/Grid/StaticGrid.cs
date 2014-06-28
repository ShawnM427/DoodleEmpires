/*! 
@file StaticGrid.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/eppathfinding.cs>
@date July 16, 2013
@brief StaticGrid Interface
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

An Interface for the StaticGrid Class.

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DoodleEmpires.Engine.Entities.PathFinder
{
    /// <summary>
    /// Represents a non-dynamic navigation grid
    /// </summary>
    public class StaticGrid : BaseGrid
    {
        /// <summary>
        /// Gets the width of this grid
        /// </summary>
        public override int Width { get; protected set; }
        /// <summary>
        /// Gets the height of this grid
        /// </summary>
        public override int Height { get; protected set; }

        private Node[][] m_nodes;

        /// <summary>
        /// Creates a new static grid
        /// </summary>
        /// <param name="iWidth">The width of the grid</param>
        /// <param name="iHeight">The height of the grid</param>
        /// <param name="iMatrix">The walkable matrix to apply to this map</param>
        public StaticGrid(int iWidth, int iHeight, bool[][] iMatrix = null):base()
        {
            Width = iWidth;
            Height = iHeight;
            m_gridRect.minX = 0;
            m_gridRect.minY = 0;
            m_gridRect.maxX = iWidth-1;
            m_gridRect.maxY = iHeight - 1;
            this.m_nodes = buildNodes(iWidth, iHeight, iMatrix);
        }

        /// <summary>
        /// Builds all the nodes for this map
        /// </summary>
        /// <param name="iWidth">The width of the map</param>
        /// <param name="iHeight">The height of the map</param>
        /// <param name="iMatrix">The walkable matrix to apply to this map</param>
        /// <returns>A collection of nodes</returns>
        private Node[][] buildNodes(int iWidth, int iHeight, bool[][] iMatrix)
        {

            Node[][] tNodes = new Node[iWidth][];
            for (int widthTrav = 0; widthTrav < iWidth; widthTrav++)
            {
                tNodes[widthTrav] = new Node[iHeight];
                for (int heightTrav = 0; heightTrav < iHeight; heightTrav++)
                {
                    tNodes[widthTrav][heightTrav] = new Node(widthTrav, heightTrav, false);
                }
            }

            if (iMatrix == null)
            {
                return tNodes;
            }

            if (iMatrix.Length != iWidth || iMatrix[0].Length != iHeight)
            {
                throw new System.ApplicationException("Matrix size does not fit");
            }


            for (int widthTrav = 0; widthTrav < iWidth; widthTrav++)
            {
                for (int heightTrav = 0; heightTrav < iHeight; heightTrav++)
                {
                    if (iMatrix[widthTrav][heightTrav])
                    {
                        tNodes[widthTrav][heightTrav].walkable = true;
                    }
                    else
                    {
                        tNodes[widthTrav][heightTrav].walkable = false;
                    }
                }
            }
            return tNodes;
        }

        /// <summary>
        /// Gets the node at the given co-ordinates
        /// </summary>
        /// <param name="iX">The x coord to get</param>
        /// <param name="iY">The y coord to get</param>
        /// <returns>The node at the given position</returns>
        public override Node GetNodeAt(int iX, int iY)
        {
            return this.m_nodes[iX][iY];
        }

        /// <summary>
        /// Gets whether the node at the given co-ordinates is walkable
        /// </summary>
        /// <param name="iX">The x coord to get</param>
        /// <param name="iY">The y coord to get</param>
        /// <returns>True if the node at the position is walkable</returns>
        public override bool IsWalkableAt(int iX, int iY)
        {
            return isInside(iX, iY) && this.m_nodes[iX][iY].walkable;
        }

        /// <summary>
        /// Checks whether the given coordinate is inside of this map
        /// </summary>
        /// <param name="iX">The x coord to check</param>
        /// <param name="iY">The y coord to check</param>
        /// <returns>True if the coordinate is within this map</returns>
        protected bool isInside(int iX, int iY)
        {
            return (iX >= 0 && iX < Width) && (iY >= 0 && iY < Height);
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
            this.m_nodes[iX][iY].walkable = iWalkable;
            return true;
        }

        /// <summary>
        /// Checks whether the given coordinate is inside of this map
        /// </summary>
        /// <param name="iPos">The position to check</param>
        /// <returns>True if the coordinate is within this map</returns>
        protected bool isInside(GridPos iPos)
        {
            return isInside(iPos.x, iPos.y);
        }

        /// <summary>
        /// Gets the node at the given co-ordinates
        /// </summary>
        /// <param name="iPos">The position to get</param>
        /// <returns>The node at the given position</returns>
        public override Node GetNodeAt(GridPos iPos)
        {
            return GetNodeAt(iPos.x, iPos.y);
        }

        /// <summary>
        /// Gets whether the node at the given co-ordinates is walkable
        /// </summary>
        /// <param name="iPos">The position to check</param>
        /// <returns>True if the node at the position is walkable</returns>
        public override bool IsWalkableAt(GridPos iPos)
        {
            return IsWalkableAt(iPos.x, iPos.y);
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
        /// Resets this grid with the given walkable matrix
        /// </summary>
        /// <param name="iMatrix">The matrix of walkable positions</param>
        public void Reset(bool[][] iMatrix)
        {
            for (int widthTrav = 0; widthTrav < Width; widthTrav++)
            {
                for (int heightTrav = 0; heightTrav < Height; heightTrav++)
                {
                    m_nodes[widthTrav][heightTrav].Reset();
                }
            }

            if (iMatrix == null)
            {
                return;
            }
            if (iMatrix.Length != Width || iMatrix[0].Length != Height)
            {
                throw new System.ApplicationException("Matrix size does not fit");
            }

            for (int widthTrav = 0; widthTrav < Width; widthTrav++)
            {
                for (int heightTrav = 0; heightTrav < Height; heightTrav++)
                {
                    if (iMatrix[widthTrav][heightTrav])
                    {
                        m_nodes[widthTrav][heightTrav].walkable = true;
                    }
                    else
                    {
                        m_nodes[widthTrav][heightTrav].walkable = false;
                    }
                }
            }
        }

        /// <summary>
        /// Creates a clone of this grid
        /// </summary>
        /// <returns>A clone of this grid</returns>
        public override BaseGrid Clone()
        {
            int tWidth = Width;
            int tHeight = Height;
            Node[][] tNodes = this.m_nodes;

            StaticGrid tNewGrid = new StaticGrid(tWidth, tHeight, null);

            Node[][] tNewNodes = new Node[tWidth][];
            for (int widthTrav = 0; widthTrav < tWidth; widthTrav++)
            {
                tNewNodes[widthTrav] = new Node[tHeight];
                for (int heightTrav = 0; heightTrav < tHeight; heightTrav++)
                {
                    tNewNodes[widthTrav][heightTrav] = new Node(widthTrav, heightTrav, tNodes[widthTrav][heightTrav].walkable);
                }
            }
            tNewGrid.m_nodes = tNewNodes;

            return tNewGrid;
        }
    }
}
