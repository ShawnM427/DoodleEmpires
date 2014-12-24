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
        public const int SIZE = 512;

        static readonly Color TRANSPARENT_WHITE = new Color(255, 255, 255, 0);
        static readonly Rectangle INTERNAL_BOUNDS = new Rectangle(0, 0, SIZE, SIZE);

        static readonly BlendState MULTIPLY = new BlendState()
        {
            ColorSourceBlend = Blend.DestinationColor,
            ColorDestinationBlend = Blend.Zero,
            ColorBlendFunction = BlendFunction.Add
        }; 

        static readonly BlendState BRUSH_BLENDSTATE = new BlendState()
        {
            AlphaDestinationBlend = Blend.DestinationAlpha,
            AlphaSourceBlend = Blend.Zero,
            AlphaBlendFunction = BlendFunction.Min,
            ColorSourceBlend = Blend.DestinationColor,
            ColorDestinationBlend = Blend.Zero,
            ColorBlendFunction = BlendFunction.Min
        };

        static readonly BlendState MAX = new BlendState()
        {
            ColorBlendFunction = BlendFunction.Max,
            ColorSourceBlend = Blend.SourceColor,
            ColorDestinationBlend = Blend.DestinationColor
        };

        static readonly BlendState ADD = new BlendState()
        {
            AlphaSourceBlend = Blend.SourceAlpha,
            AlphaDestinationBlend = Blend.One,
            //ColorBlendFunction = BlendFunction.Add,
            ColorSourceBlend = Blend.SourceAlpha,
            ColorDestinationBlend = Blend.One
        };

        static readonly BlendState SUB = new BlendState()
        {
            ColorSourceBlend = Blend.SourceAlpha,
            ColorDestinationBlend = Blend.One,
            ColorBlendFunction = BlendFunction.ReverseSubtract,
            AlphaSourceBlend = Blend.SourceAlpha,
            AlphaDestinationBlend = Blend.One,
            AlphaBlendFunction = BlendFunction.ReverseSubtract
        };

        static readonly BlendState WHITE = new BlendState()
        {
            BlendFactor = Color.White,
            ColorBlendFunction = BlendFunction.Add,
            ColorSourceBlend = Blend.SourceColor,
            ColorDestinationBlend = Blend.DestinationColor
        };

        GraphicsDevice _graphicsDevice;
        QuadTree<bool> _dataTree;
        Rectangle _bounds;
        SpriteBatch _spriteBatch;

        Color[] _deformBuffer;
        Texture2D _backgroundTexture;
        RenderTarget2D _textureBuffer;
        RenderTarget2D _lightBuffer;
        RenderTarget2D _backBuffer;
        RenderTarget2D _backDrop;
        RenderTarget2D _tempBuffer;
                
        public Texture2D Terrain
        {
            get { return _textureBuffer; }
            set
            {
                _textureBuffer = (RenderTarget2D)value;
                _textureBuffer.GetData(_deformBuffer);
            }
        }

        public PixelRegion(Point position, GraphicsDevice graphics, Texture2D nightBackground)
        {
            _bounds = new Rectangle(position.X, position.Y, SIZE, SIZE);
            _dataTree = new QuadTree<bool>();

            _graphicsDevice = graphics;
            _spriteBatch = new SpriteBatch(graphics);

            _deformBuffer = new Color[SIZE * SIZE];
            _deformBuffer.Populate(TRANSPARENT_WHITE);

            _backgroundTexture = nightBackground;

            _textureBuffer = new RenderTarget2D(_graphicsDevice, SIZE, SIZE, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            _lightBuffer = new RenderTarget2D(_graphicsDevice, SIZE, SIZE, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            _backBuffer = new RenderTarget2D(_graphicsDevice, SIZE, SIZE, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            _backDrop = new RenderTarget2D(_graphicsDevice, SIZE, SIZE, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            _tempBuffer = new RenderTarget2D(_graphicsDevice, SIZE, SIZE, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
        }

        public virtual void Render(SpriteBatch batch)
        {
            batch.Draw(_backBuffer, _bounds, Color.White);
            //batch.Draw(_textureBuffer, _bounds, Color.White);
        }

        public virtual void RenderLights(List<ILight> lights, Color ambientLightColor)
        {
            _graphicsDevice.SetRenderTarget(_lightBuffer);
            _graphicsDevice.Clear(ambientLightColor);
                        
            _spriteBatch.Begin(SpriteSortMode.Immediate, ADD, SamplerState.LinearWrap, null, null);

            _spriteBatch.Draw(_backDrop, INTERNAL_BOUNDS, _bounds, Color.White * (1.0f - Global.TimeOfDay));
            
            foreach (ILight light in lights)
                _spriteBatch.Draw(light.Texture, light.Position - new Vector2(_bounds.X, _bounds.Y), null, Color.White * ((light.Intensity - Global.TimeOfDay) * 0.75f), MathHelper.ToRadians(light.Rotation), light.TextureOrigin, 1.0f, SpriteEffects.None, 0.0f);

            _spriteBatch.End();
        }

        protected virtual void UpdateBackBuffer()
        {
            _graphicsDevice.SetRenderTarget(_backBuffer);
            _graphicsDevice.Clear(TRANSPARENT_WHITE);

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null);
            _spriteBatch.Draw(_textureBuffer, INTERNAL_BOUNDS, Color.White);
            _spriteBatch.End();

            _spriteBatch.Begin(SpriteSortMode.Immediate, MULTIPLY);
            _spriteBatch.Draw(_lightBuffer, INTERNAL_BOUNDS, Color.White);
            _spriteBatch.End();
        }

        protected virtual void UpdateBackdrop()
        {
            _graphicsDevice.SetRenderTarget(_backDrop);
            _graphicsDevice.Clear(Color.LightGray);

            _spriteBatch.Begin();
            _spriteBatch.Draw(_textureBuffer, INTERNAL_BOUNDS, Color.Black);
            _spriteBatch.End();

            _spriteBatch.Begin(SpriteSortMode.Immediate, MULTIPLY, SamplerState.LinearWrap, null, null);
            _spriteBatch.Draw(_backgroundTexture, INTERNAL_BOUNDS, _bounds, Color.White);
            _spriteBatch.End();
        }

        public virtual void Update()
        {
            UpdateBackBuffer();
        }

        public void ApplyBrush(IPixelBrush brush, Vector2 position, float rotation = 0)
        {
            _graphicsDevice.SetRenderTarget(_tempBuffer);
            _graphicsDevice.Clear(TRANSPARENT_WHITE);
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, null, null);
            _spriteBatch.Draw(_textureBuffer, INTERNAL_BOUNDS, Color.White);
            _spriteBatch.End();

            _graphicsDevice.SetRenderTarget(_textureBuffer);
            _graphicsDevice.Clear(Color.White);

            _spriteBatch.Begin(SpriteSortMode.Immediate, BRUSH_BLENDSTATE, SamplerState.PointWrap, null, null);
            _spriteBatch.Draw(_tempBuffer, INTERNAL_BOUNDS, Color.White);
            _spriteBatch.Draw(brush.Texture, position - new Vector2(_bounds.X, _bounds.Y), null, Color.White, rotation, brush.HalfSize, 1.0f, SpriteEffects.None, 0.5f);
            _spriteBatch.End();

            UpdateBackdrop();
        }

        protected void SetPixel(int x, int y, Color color)
        {
            _deformBuffer[x + y * _textureBuffer.Width] = color;
        }

        public void ApplyTerrainBuilder(TerrainBuilder builder)
        {            
            for(int x = 0; x < SIZE; x ++)
            {
                int yy = (int)(NoiseGeneration.Noise.PerlinNoise_1D((x + _bounds.X) / 300.0f) * 100.0f) + 200;
                for(int y = 0; y < SIZE; y++)
                {
                    SetPixel(x, y, y + _bounds.Y == yy ? Color.Black : y + _bounds.Y > yy ? Color.Brown : TRANSPARENT_WHITE);
                }
            }

            _textureBuffer.SetData(_deformBuffer);
            UpdateBackdrop();
        }
    }

    internal class QuadTree<T>
    {
        public void Set(int x, int y, T data) { }
    }
}
