using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleEmpires.Engine.Terrain
{
    public class PixelRegion
    {
        public const int WIDTH = 512;
        public const int HEIGHT = 512;

        static readonly VertexPositionColorTexture[] QuadVerts = new VertexPositionColorTexture[]{
            new VertexPositionColorTexture(new Vector3(0, 0, 0), Color.White, new Vector2(0,0)),
            new VertexPositionColorTexture(new Vector3(WIDTH, 0, 0), Color.White, new Vector2(1,0)),
            new VertexPositionColorTexture(new Vector3(WIDTH, HEIGHT, 0), Color.White, new Vector2(1,1)),
            new VertexPositionColorTexture(new Vector3(0, HEIGHT, 0), Color.White, new Vector2(0,1))
        };

        BasicEffect _renderEffect;
        GraphicsDevice _graphicsDevice;
        QuadTree<bool> _dataTree;
        Rectangle _bounds;
        SpriteBatch _spriteBatch;

        Color[] _deformBuffer;
        Texture2D _textureBuffer;

        public Texture2D Terrain
        {
            get { return _textureBuffer; }
            set
            {
                _textureBuffer = value;
                _textureBuffer.GetData(_deformBuffer);
            }
        }

        public PixelRegion(Point position, GraphicsDevice graphics)
        {
            _bounds = new Rectangle(position.X, position.Y, WIDTH, HEIGHT);
            _dataTree = new QuadTree<bool>();

            _graphicsDevice = graphics;
            _spriteBatch = new SpriteBatch(graphics);

            _renderEffect = new BasicEffect(graphics);
            _renderEffect.TextureEnabled = true;
            _renderEffect.VertexColorEnabled = true;
            _renderEffect.Projection = Matrix.CreateOrthographicOffCenter(0, WIDTH, HEIGHT, 0, 1.0f, 1000.0f);
            _renderEffect.World = Matrix.Identity;
            _renderEffect.View = Matrix.Identity;

            _deformBuffer = new Color[WIDTH * HEIGHT];
            _textureBuffer = new Texture2D(graphics, WIDTH, HEIGHT);
            _textureBuffer.SetData(_deformBuffer);

            _renderEffect.Texture = _textureBuffer;
        }

        public void Render()
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_textureBuffer, _bounds, Color.White);
            _spriteBatch.End();

            //_renderEffect.CurrentTechnique.Passes[0].Apply();
            //_graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, QuadVerts, 0, 2);
        }

        public void ApplyBrush(PixelBrush brush, Vector2 position, float rotation = 0)
        {
            Matrix mat = Matrix.CreateTranslation(new Vector3(-brush.HalfSize, 0)) * Matrix.CreateRotationZ(rotation)
                * Matrix.CreateTranslation(position.X, position.Y, 0);
            
            for (int x = 0; x < brush.Width; x++)
            {
                for (int y = 0; y < brush.Height; y++)
                {
                    Vector2 imagePos = new Vector2(x, y);
                    Vector2 screenPos = Vector2.Transform(imagePos, mat);

                    int screenX = (int)screenPos.X;
                    int screenY = (int)screenPos.Y;

                    if (screenX >= 0 && screenX < WIDTH && screenY >= 0 && screenY < HEIGHT)
                    {
                        if (_deformBuffer[screenX + screenY * _textureBuffer.Width].A != 0)
                        {
                            if (brush[x, y].A > 10)
                            {
                                if (brush[x, y] == brush.EraseColor)
                                {
                                    _deformBuffer[screenX + screenY * _textureBuffer.Width] = Color.Transparent;
                                    _dataTree.Set(screenX, screenY, false);
                                }
                                else
                                {
                                    _deformBuffer[screenX + screenY * _textureBuffer.Width] = brush[x, y];
                                    _dataTree.Set(screenX, screenY, true);
                                }
                            }
                        }
                    }
                }
            }

            _textureBuffer.SetData(_deformBuffer);
        }
    }

    internal class QuadTree<T>
    {
        public void Set(int x, int y, T data) { }
    }
}
