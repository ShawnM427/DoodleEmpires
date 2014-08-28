using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace DoodleEmpires.Engine.Render
{
    /// <summary>
    /// Represents a chunk of cached graphics data
    /// </summary>
    /// <typeparam name="T">The vertex type for this chunk</typeparam>
    public class RenderChunk<T> : IDisposable where T : struct, IVertexType
    {
        GraphicsDevice _graphics;
        IndexBuffer _indexBuffer;
        VertexBuffer _vertexBuffer;
        int _primitiveCount = 0;
        PrimitiveType _primitiveType;

        /// <summary>
        /// Gets this render chunk's index buffer
        /// </summary>
        public IndexBuffer IndexBuffer
        {
            get { return _indexBuffer; }
        }
        /// <summary>
        /// Gets this render chunk's vertex buffer
        /// </summary>
        public VertexBuffer VertexBuffer
        {
            get { return _vertexBuffer; }
        }

        /// <summary>
        /// Creates a new graphcis device
        /// </summary>
        /// <param name="graphics">The graphics device to bind to</param>
        /// <param name="primitiveType">The type of primitive to draw</param>
        public RenderChunk(GraphicsDevice graphics, PrimitiveType primitiveType)
        {
            _graphics = graphics;
            _primitiveType = primitiveType;
        }

        /// <summary>
        /// Sets the vertex buffer to a given list of vertices
        /// </summary>
        /// <param name="vertices">The vertices to use</param>
        public void SetVertexBuffer(T[] vertices)
        {
            _vertexBuffer = new VertexBuffer(_graphics, typeof(T), vertices.Length, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(vertices);
        }

        /// <summary>
        /// Sets the index buffer to a given list of indices
        /// </summary>
        /// <param name="indices">The indices to use</param>
        public void SetIndexBuffer(short[] indices)
        {
            _indexBuffer = new IndexBuffer(_graphics, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
            _indexBuffer.SetData(indices);

            CalcPrimitives();
        }

        /// <summary>
        /// Caclulate the number of primitives to draw based off the primitive type
        /// </summary>
        private void CalcPrimitives()
        {
            switch (_primitiveType)
            {
                case PrimitiveType.LineList:
                    _primitiveCount = _indexBuffer.IndexCount / 2;
                    break;
                case PrimitiveType.LineStrip:
                    _primitiveCount = _indexBuffer.IndexCount - 1;
                    break;
                case PrimitiveType.TriangleList:
                    _primitiveCount = _indexBuffer.IndexCount / 3;
                    break;
                case PrimitiveType.TriangleStrip:
                    _primitiveCount = _indexBuffer.IndexCount - 2;
                    break;
            }
        }

        /// <summary>
        /// Renders this item using the underlying graphics device
        /// </summary>
        public void Render()
        {
            _graphics.SetVertexBuffer(_vertexBuffer);
            _graphics.Indices = _indexBuffer;
            _graphics.DrawPrimitives(_primitiveType, 0, _primitiveCount);
        }

        /// <summary>
        /// Disposes of this object and free's it's resources
        /// </summary>
        public void Dispose()
        {
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
        }
    }
}
