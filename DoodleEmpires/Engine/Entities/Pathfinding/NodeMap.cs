using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Entities.Pathfinding
{
    public class PathNode : IHasNeighbours<PathNode>
    {
        List<PathNode> _neighbours;
        float _x;
        float _y;

        public IEnumerable<PathNode> Neighbours
        {
            get
            {
                return _neighbours;
            }
        }
        public float X
        {
            get { return _x; }
            set { _x = value; }
        }
        public float Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public PathNode(float X, float Y)
        {
            _neighbours = new List<PathNode>();

            _x = X;
            _y = Y;
        }

        public void AddNeighbour(PathNode node)
        {
            node._neighbours.Add(this);
            _neighbours.Add(node);
        }

        public bool RemoveNeighbour(PathNode node)
        {
            node._neighbours.Remove(this);
            return _neighbours.Remove(node);
        }

        public override bool Equals(object obj)
        {
            return (obj.GetType() == typeof(PathNode)) && ((PathNode)obj).X == X && ((PathNode)obj).Y == Y;
        }

        public static readonly Func<PathNode, PathNode, double> Distance 
            = new Func<PathNode,PathNode,double>((A, B) =>
            {
                return (A.X - B.X) * (A.X - B.X) + (A.Y - B.Y) * (A.Y - B.Y);
            });

        public static readonly Func<PathNode, double> Estimate
            = new Func<PathNode, double>((A) =>
            {
                return 0;
            });
    }

    public class NodeMap
    {
        PathNode[,] _nodes;
        int _width;
        int _height;
        int _cellSpacing;
        float _cellOffX;
        float _cellOffY;

        Texture2D _pixelTex;

        public NodeMap(int width, int height, int cellSize)
        {
            _nodes = new PathNode[width, height];
            _width = width;
            _height = height;

            _cellSpacing = cellSize;
            _cellOffX = cellSize / 2.0f;
            _cellOffY = cellSize / 2.0f;
        }

        public void ActivateNode(int x, int y)
        {
            if (x >= 0 && x < _width && y >= 0 && y < _height)
                _ActivateNode(x, y);
        }

        public void DeactivateNode(int x, int y)
        {
            if (x >= 0 && x < _width && y >= 0 && y < _height)
                _DeactivateNode(x, y);
        }

        private void _ActivateNode(int x, int y)
        {
            if (_nodes[x, y] == null)
            {
                _nodes[x, y] = new PathNode(x * _cellSpacing + _cellOffX, y * _cellSpacing + _cellOffY);
                _ActivatedCellState(x, y);
            }
        }

        private void _DeactivateNode(int x, int y)
        {
            if (_nodes[x, y] != null)
            {
                _DeactivatedCellState(x, y);
                _nodes[x, y] = null;
            }
        }

        private void _ActivatedCellState(int x, int y)
        {
            for (int xx = x - 1 < 0 ? 0 : x - 1; xx <= (x + 1 >= _width ? _width - 1 : x + 1); xx++)
            {
                for (int yy = y - 1 < 0 ? 0 : y - 1; yy <= (y + 1 >= _height ? _height - 1 : y + 1); yy++)
                {
                    if (_nodes[xx, yy] != null && !_nodes[xx, yy].Neighbours.Contains(_nodes[x, y]))
                        _nodes[xx, yy].AddNeighbour(_nodes[x, y]);
                }
            }
        }

        private void _DeactivatedCellState(int x, int y)
        {
            for (int xx = x - 1 < 0 ? 0 : x - 1; xx <= (x + 1 >= _width ? _width - 1 : x + 1); xx++)
            {
                for (int yy = y - 1 < 0 ? 0 : y - 1; yy <= (y + 1 >= _height ? _height - 1 : y + 1); yy++)
                {
                    if (_nodes[xx, yy] != null)
                        _nodes[xx, yy].RemoveNeighbour(_nodes[x, y]);
                }
            }
        }

        public Path<PathNode> FindPath(PathNode start, PathNode end)
        {
            return AStar.FindPath<PathNode>(start, end, PathNode.Distance, PathNode.Estimate);
        }

        public void Draw(SpriteBatch batch, Texture2D nodeIcon, Rectangle bounds)
        {
            if (_pixelTex == null)
            {
                _pixelTex = new Texture2D(batch.GraphicsDevice, 1, 1);
                _pixelTex.SetData<Color>(new Color[] { Color.White });
            }

            bounds.X /= _cellSpacing;
            bounds.Y /= _cellSpacing;

            bounds.Width /= _cellSpacing;
            bounds.Height /= _cellSpacing;

            List<VertexPositionColor> _lineStrip = new List<VertexPositionColor>();

            for (int x = bounds.X; x < bounds.Right; x++)
            {
                for (int y = bounds.Y; y < bounds.Bottom; y++)
                {
                    if (_nodes[x, y] != null)
                    {
                        batch.Draw(nodeIcon, new Vector2(_nodes[x, y].X, _nodes[x, y].Y), null, Color.White,
                            0.0f, new Vector2(nodeIcon.Width / 2, nodeIcon.Height / 2), 0.01f, SpriteEffects.None, 0);

                        foreach (PathNode node in _nodes[x, y].Neighbours)
                        {
                            DrawLine(batch, _pixelTex, new Vector2(_nodes[x, y].X, _nodes[x, y].Y), new Vector2(node.X, node.Y), Color.Black);
                        }
                    }
                }
            }
        }
        public static void DrawLine(SpriteBatch spriteBatch, Texture2D tex, Vector2 begin, Vector2 end, Color color, int width = 1)
        {
            Rectangle r = new Rectangle((int)begin.X, (int)begin.Y, (int)(end - begin).Length() + width, width);
            Vector2 v = Vector2.Normalize(begin - end);
            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
            if (begin.Y > end.Y) angle = MathHelper.TwoPi - angle;
            spriteBatch.Draw(tex, r, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}
