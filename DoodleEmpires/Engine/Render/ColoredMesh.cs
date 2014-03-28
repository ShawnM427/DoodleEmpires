using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace DoodleEmpires.Engine.Render
{
    /// <summary>
    /// Represents a simple colored mesh
    /// </summary>
    public class ColoredMesh
    {
        const int EXPANDSIZE = 32;

        int _vCount = 0;
        int _iCount = 0;

        VertexPositionColor[] _verts;
        int[] _indices;
        PrimitiveType _primitiveType;

        int _primitiveCount = 0;

        GraphicsDevice _graphics;

        /// <summary>
        /// Gets or sets a vertex in this mesh
        /// </summary>
        /// <param name="index">The index to look up</param>
        /// <returns>The vertex at the given index</returns>
        public VertexPositionColor this[int index]
        {
            get { return _verts[index]; }
            set { _verts[index] = value; }
        }

        /// <summary>
        /// Creates a new Colored mesh
        /// </summary>
        /// <param name="primitiveType">The primitive type for this mesh</param>
        /// <param name="graphics">The Graphics Device to use</param>
        public ColoredMesh(PrimitiveType primitiveType, GraphicsDevice graphics)
        {
            _primitiveType = primitiveType;

            _verts = new VertexPositionColor[EXPANDSIZE];
            _indices = new int[EXPANDSIZE];

            _graphics = graphics;
        }

        /// <summary>
        /// Adds a vertice to this mesh
        /// </summary>
        /// <param name="vert">The vertex to add</param>
        /// <returns>The index of the vertex</returns>
        public int AddVert(VertexPositionColor vert)
        {
            if (_verts.Length <= _vCount + 1)
                Array.Resize<VertexPositionColor>(ref _verts, _verts.Length + EXPANDSIZE);

            _verts[_vCount] = vert;
            _vCount++;
            return _vCount - 1;
        }

        /// <summary>
        /// Adds a vertex to this mesh
        /// </summary>
        /// <param name="index">The inndex of the vertice to add</param>
        public void AddIndex(int index)
        {
            if (_indices.Length <= _iCount + 1)
                Array.Resize<int>(ref _indices, _indices.Length + EXPANDSIZE);

            _indices[_iCount] = index;
            _iCount++;
        }

        /// <summary>
        /// Calculates the number of primitives, must be called when geometry is changed
        /// </summary>
        public void Finish()
        {
            switch (_primitiveType)
            {
                case PrimitiveType.LineList:
                    _primitiveCount = _iCount / 2;
                    break;
                case PrimitiveType.LineStrip:
                    _primitiveCount = _iCount - 1;
                    break;
                case PrimitiveType.TriangleList:
                    _primitiveCount = _iCount / 3;
                    break;
                case PrimitiveType.TriangleStrip:
                    _primitiveCount = _iCount - 2;
                    break;
            }
        }

        /// <summary>
        /// Renders this mesh
        /// </summary>
        public void Render()
        {
            if (_primitiveCount != 0)
                _graphics.DrawUserIndexedPrimitives<VertexPositionColor>(_primitiveType, _verts, 0, _vCount, 
                    _indices, 0, _primitiveCount); 
        }
    }
}
