using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoodleEmpires.Engine.Render;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Terrain
{
    /// <summary>
    /// Represents the basic terrain
    /// </summary>
    public class Terrain
    {
        public const int CHUNKSIZE = 512;
        const float GRASSDEPTH = 20.0F;
        const float STONEDEPTH =  GRASSDEPTH + 15.0F;
        const float TERRAINOFFSET = 2400.0F;

        MapColorScheme _colorScheme;

        float _width;
        float _terrainDepth;
        float[] _heightmap;
        Materials[] _materials;
        ColoredMesh[] _chunks;
        float _worldChunkSize;

        VertexPositionColorTexture[] _backVerts = new VertexPositionColorTexture[4];
        VertexPositionColorTexture[] _backLowerVerts = new VertexPositionColorTexture[4];
        int[] _backIndices = new int[] { 0, 1, 2, 2, 1, 3};
        BasicEffect _backPaperEffect;
        BasicEffect _backDeskEffect;
        Color _backgroundColor = Color.White;

        float _spacing = 1;

        GraphicsDevice _graphics;
        BasicEffect _effect;
        
        public float TotalWidth
        {
            get { return _width; }
        }

        public Terrain(int chunks, float spacing, float terrainDepth, GraphicsDevice graphics, MapColorScheme colors, Texture2D background, Texture2D lowerBackground)
        {
            _width = CHUNKSIZE * spacing * chunks;
            _worldChunkSize = CHUNKSIZE * spacing;
            _heightmap = new float[CHUNKSIZE * chunks];
            _materials = new Materials[CHUNKSIZE * chunks];
            _spacing = spacing;
            _terrainDepth = terrainDepth;
            _graphics = graphics;
            _colorScheme = colors;

            Matrix projection = Matrix.CreateOrthographicOffCenter(0, _graphics.Viewport.Width, _graphics.Viewport.Height, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            Matrix _projection = halfPixelOffset * projection;
            
            for (int i = 0; i < _materials.Length; i++)
                _materials[i] = Materials.Grass;

            _heightmap = VolcanicFormation.GenMap(_heightmap.Length, 16, 32);

            _effect = new BasicEffect(_graphics);
            _effect.VertexColorEnabled = true;
            _effect.Projection = _projection;
            
            _chunks = new ColoredMesh[chunks];

            for (int i = 0; i < _chunks.Length; i++)
                _chunks[i] = new ColoredMesh(PrimitiveType.TriangleList, _graphics);

            _backPaperEffect = new BasicEffect(graphics);
            _backPaperEffect.VertexColorEnabled = true;
            _backPaperEffect.TextureEnabled = true;
            _backPaperEffect.Texture = background;
            _backPaperEffect.Projection = _projection;

            _backDeskEffect = new BasicEffect(graphics);
            _backDeskEffect.VertexColorEnabled = true;
            _backDeskEffect.TextureEnabled = true;
            _backDeskEffect.Texture = lowerBackground;
            _backDeskEffect.Projection = _projection;

            BuildRenders();
        }

        /// <summary>
        /// Rebuilds the render states
        /// </summary>
        public void BuildRenders()
        {
            float _texW = _width / _backPaperEffect.Texture.Width;
            float _texH = (TERRAINOFFSET + _terrainDepth) / _backPaperEffect.Texture.Height;

            _backVerts[0] = new VertexPositionColorTexture(new Vector3(0, 0, 0), _backgroundColor, Vector2.Zero);
            _backVerts[1] = new VertexPositionColorTexture(new Vector3(_width, 0, 0), _backgroundColor, new Vector2(_texW, 0));
            _backVerts[2] = new VertexPositionColorTexture(new Vector3(0, (TERRAINOFFSET + _terrainDepth), 0), _backgroundColor, new Vector2(0, _texH));
            _backVerts[3] = new VertexPositionColorTexture(new Vector3(_width, (TERRAINOFFSET + _terrainDepth), 0), _backgroundColor, new Vector2(_texW, _texH));

            _texW = _width / _backDeskEffect.Texture.Width;
            _texH = (TERRAINOFFSET + _terrainDepth) / _backDeskEffect.Texture.Height;

            _backLowerVerts[0] = new VertexPositionColorTexture(new Vector3(0, 0, 0), _backgroundColor, Vector2.Zero);
            _backLowerVerts[1] = new VertexPositionColorTexture(new Vector3(_width, 0, 0), _backgroundColor, new Vector2(_texW, 0));
            _backLowerVerts[2] = new VertexPositionColorTexture(new Vector3(0, (TERRAINOFFSET + _terrainDepth), 0), _backgroundColor, new Vector2(0, _texH));
            _backLowerVerts[3] = new VertexPositionColorTexture(new Vector3(_width, (TERRAINOFFSET + _terrainDepth), 0), _backgroundColor, new Vector2(_texW, _texH));
            ColoredMesh chunk;
            int gr, dr, sr, br, lX, hX;
            float wX;

            for (int x = 0; x < _chunks.Length; x++)
            {
                chunk = _chunks[x];

                lX = x * CHUNKSIZE;

                chunk.AddVert(
                    new VertexPositionColor(new Vector3(lX * _spacing, TERRAINOFFSET - _heightmap[lX], 0.1F), _colorScheme.GrassColor));
                chunk.AddVert(
                    new VertexPositionColor(new Vector3(lX * _spacing, TERRAINOFFSET - _heightmap[lX] + GRASSDEPTH, 0.1F), _colorScheme.DirtColor));
                chunk.AddVert(
                    new VertexPositionColor(new Vector3(lX * _spacing, TERRAINOFFSET - _heightmap[lX] + STONEDEPTH, 0.1F), _colorScheme.StoneColor));
                chunk.AddVert(
                    new VertexPositionColor(new Vector3(lX * _spacing, TERRAINOFFSET + _terrainDepth, 0.1F), _colorScheme.BottomColor));


                for (int x2 = 0; x2 < CHUNKSIZE + 1; x2++)
                {                    
                    hX = lX + x2;

                    if (hX >= _heightmap.Length)
                        continue;

                    wX = (lX + x2) * _spacing;

                    gr = chunk.AddVert(
                        new VertexPositionColor(new Vector3(wX, TERRAINOFFSET - _heightmap[hX], 0.1F), _colorScheme.GrassColor));
                    dr = chunk.AddVert(
                        new VertexPositionColor(new Vector3(wX, TERRAINOFFSET - _heightmap[hX] + GRASSDEPTH, 0.1F), _colorScheme.DirtColor));
                    sr = chunk.AddVert(
                        new VertexPositionColor(new Vector3(wX, TERRAINOFFSET - _heightmap[hX] + STONEDEPTH, 0.1F), _colorScheme.StoneColor));
                    br = chunk.AddVert(
                        new VertexPositionColor(new Vector3(wX, TERRAINOFFSET + _terrainDepth, 0.1F), _colorScheme.BottomColor));

                    chunk.AddIndex(gr - 4);
                    chunk.AddIndex(gr);
                    chunk.AddIndex(dr);

                    chunk.AddIndex(gr - 4);
                    chunk.AddIndex(dr);
                    chunk.AddIndex(dr - 4);

                    chunk.AddIndex(dr - 4);
                    chunk.AddIndex(dr);
                    chunk.AddIndex(sr - 4);

                    chunk.AddIndex(dr);
                    chunk.AddIndex(sr);
                    chunk.AddIndex(sr - 4);

                    chunk.AddIndex(sr - 4);
                    chunk.AddIndex(sr);
                    chunk.AddIndex(br - 4);

                    chunk.AddIndex(sr);
                    chunk.AddIndex(br);
                    chunk.AddIndex(br - 4);
                }
            }
            foreach(ColoredMesh c in _chunks)
                c.Finish();
        }

        /// <summary>
        /// Adjusts the height of the terrain at a specific point
        /// </summary>
        /// <param name="x">The x coord to change, in map co-ords</param>
        /// <param name="change">The amount to change by</param>
        /// <param name="changeToDirt">True if the material should be set to dirt</param>
        /// <param name="smooth">True if the terrain should be smoothed</param>
        public void ChangeHeight(int x, float change, bool changeToDirt = false, bool smooth = true)
        {
            x = x < 0 ? 0 : x >= _heightmap.Length ? _heightmap.Length - 1 : x;
            SetHeight(x, _heightmap[x] + change, false);

            if (smooth)
                _heightmap.Smooth(x - 5, x + 5, 2);

            UpdatePatch(x - 5, x + 5);
        }

        /// <summary>
        /// Sets the height of the terrain at a given point
        /// </summary>
        /// <param name="x">The x-coord, in map coords</param>
        /// <param name="height">The new height to set the point to</param>
        /// <param name="build">True if the terrain stack should be rebuilt</param>
        public void SetHeight(int x, float height, bool build = true)
        {
            height = height > _terrainDepth - STONEDEPTH ? _terrainDepth - STONEDEPTH : height;
            _heightmap[x] = height;

            if (build)
                UpdateStack(x);
        }

        int _UPDATESTACK_chunkX;
        int _UPDATESTACK_relX;
        int _UPDATESTACK_id;
        /// <summary>
        /// Updates a single stack of vertices
        /// </summary>
        /// <param name="x">The x coord to update (map)</param>
        protected void UpdateStack(int x)
        {
            _UPDATESTACK_chunkX = ((x - 1) / CHUNKSIZE);
            _UPDATESTACK_relX = (x - (_UPDATESTACK_chunkX * CHUNKSIZE));
            _UPDATESTACK_id = _UPDATESTACK_relX * 4;

            if (_UPDATESTACK_chunkX >= 0 & _UPDATESTACK_chunkX < _chunks.Length)
            {
                if (_UPDATESTACK_relX == CHUNKSIZE)
                {
                    if (_UPDATESTACK_chunkX != _chunks.Length - 1)
                    {
                        VertexPositionColor vert = _chunks[_UPDATESTACK_chunkX][CHUNKSIZE * 4 + 4];
                        vert.Position.Y = TERRAINOFFSET - _heightmap[x + 1];
                        _chunks[_UPDATESTACK_chunkX][CHUNKSIZE * 4 + 4] = vert;

                        vert = _chunks[_UPDATESTACK_chunkX][CHUNKSIZE * 4 + 5];
                        vert.Position.Y = TERRAINOFFSET - _heightmap[x + 1] + GRASSDEPTH;
                        _chunks[_UPDATESTACK_chunkX][CHUNKSIZE * 4 + 5] = vert;

                        vert = _chunks[_UPDATESTACK_chunkX][CHUNKSIZE * 4 + 6];
                        vert.Position.Y = TERRAINOFFSET - _heightmap[x + 1] + STONEDEPTH;
                        _chunks[_UPDATESTACK_chunkX][CHUNKSIZE * 4 + 6] = vert;
                    }
                    else
                    {
                        VertexPositionColor vert = _chunks[_UPDATESTACK_chunkX][CHUNKSIZE * 4];
                        vert.Position.Y = TERRAINOFFSET - _heightmap[x - 1];
                        _chunks[_UPDATESTACK_chunkX][CHUNKSIZE * 4] = vert;

                        vert = _chunks[_UPDATESTACK_chunkX][CHUNKSIZE * 4];
                        vert.Position.Y = TERRAINOFFSET - _heightmap[x - 1] + GRASSDEPTH;
                        _chunks[_UPDATESTACK_chunkX][CHUNKSIZE * 4] = vert;

                        vert = _chunks[_UPDATESTACK_chunkX][CHUNKSIZE * 4];
                        vert.Position.Y = TERRAINOFFSET - _heightmap[x - 1] + STONEDEPTH;
                        _chunks[_UPDATESTACK_chunkX][CHUNKSIZE * 4] = vert;
                        return;
                    }
                }

                if (_UPDATESTACK_id >= 0)
                {
                    VertexPositionColor vert = _chunks[_UPDATESTACK_chunkX][_UPDATESTACK_id];
                    vert.Position.Y = TERRAINOFFSET - _heightmap[x];
                    _chunks[_UPDATESTACK_chunkX][_UPDATESTACK_id] = vert;

                    vert = _chunks[_UPDATESTACK_chunkX][_UPDATESTACK_id + 1];
                    vert.Position.Y = TERRAINOFFSET - _heightmap[x] + GRASSDEPTH;
                    _chunks[_UPDATESTACK_chunkX][_UPDATESTACK_id + 1] = vert;

                    vert = _chunks[_UPDATESTACK_chunkX][_UPDATESTACK_id + 2];
                    vert.Position.Y = TERRAINOFFSET - _heightmap[x] + STONEDEPTH;
                    _chunks[_UPDATESTACK_chunkX][_UPDATESTACK_id + 2] = vert;
                }
            }
        }

        /// <summary>
        /// Updates a patch of vertices
        /// </summary>
        /// <param name="minX">The minimum x co-ord (map)</param>
        /// <param name="maxX">The maximum x co-ord (map)</param>
        protected void UpdatePatch(int minX, int maxX)
        {
            minX = minX < 0 ? 0 : minX <= _heightmap.Length ? minX : _heightmap.Length - 1;
            maxX = maxX > _heightmap.Length ? _heightmap.Length : maxX;

            for (int x = minX; x <= maxX; x++)
                UpdateStack(x);
        }

        /// <summary>
        /// Sets the ground material at a given map co-ord
        /// </summary>
        /// <param name="x">The x co-ordinate to set, in map coords</param>
        /// <param name="material">The material to set to</param>
        /// <param name="setDirt">True if the dirt layer should be set as well</param>
        public void SetMaterial(int x, Materials material, bool setDirt = false)
        {
            int _chunkX = ((x - 1)  / CHUNKSIZE);
            int _relX = (x - (_chunkX * CHUNKSIZE));
            int id = _relX * 4;

            if (_chunkX >= 0 & _chunkX < _chunks.Length)
            {
                if (_relX == CHUNKSIZE & _chunkX + 1 <= _chunks.Length)
                {
                    VertexPositionColor vert = _chunks[_chunkX][CHUNKSIZE * 4 + 4];
                    vert.Color = LookupColor(material);
                    _chunks[_chunkX][CHUNKSIZE * 4 + 4] = vert;

                    if (setDirt)
                    {
                        vert = _chunks[_chunkX][CHUNKSIZE * 4 + 5];
                        vert.Color = LookupColor(material);
                        _chunks[_chunkX][CHUNKSIZE * 4 + 5] = vert;
                    }
                }

                if (id >= 0)
                {
                    VertexPositionColor vert = _chunks[_chunkX][id];
                    vert.Color = LookupColor(material);
                    _chunks[_chunkX][id] = vert;

                    if (setDirt)
                    {
                        vert = _chunks[_chunkX][id + 1];
                        vert.Color = LookupColor(material);
                        _chunks[_chunkX][id + 1] = vert;
                    }
                }
            }
        }

        /// <summary>
        /// Looks up the color for a material
        /// </summary>
        /// <param name="material">The material to look up</param>
        /// <returns>The surface color for the material</returns>
        private Color LookupColor(Materials material)
        {
            switch (material)
            {
                case Materials.Grass:
                    return _colorScheme.GrassColor;
                case Materials.Dirt:
                    return _colorScheme.DirtColor;
                case Materials.Stone:
                    return _colorScheme.StoneColor;
                case Materials.Sand:
                    return _colorScheme.SandColor;
                default:
                    return Color.Black;
            }
        }

        /// <summary>
        /// Gets the height at a given x world position
        /// </summary>
        /// <param name="x">The x to check</param>
        /// <returns>The height at the given position, or the closest position possible</returns>
        public float GetHeight(float x)
        {
            int l = (int)(x / _spacing);
            int r = l + 1;
            l = l < 0 ? 0 : l >= _heightmap.Length ? _heightmap.Length - 1 : l;
            r = r < 0 ? 0 : r >= _heightmap.Length ? _heightmap.Length - 1 : r;
            return MathHelper.Lerp(_heightmap[l], _heightmap[r], (x % _spacing) / _spacing);
        }

        int _LEFT, _RIGHT;
        /// <summary>
        /// Renders this terrain
        /// </summary>
        /// <param name="view">The camera to render with</param>
        public void Render(Camera2D view)
        {
            _effect.World = view.Transform;
            _backPaperEffect.World = view.Transform;
            _backDeskEffect.World = view.Transform;

            foreach (EffectPass p in _backDeskEffect.CurrentTechnique.Passes)
            {
                p.Apply();
                _graphics.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, _backLowerVerts, 0, 4, _backIndices, 0, 2);
            }

            foreach (EffectPass p in _backPaperEffect.CurrentTechnique.Passes)
            {
                p.Apply();
                _graphics.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, _backVerts, 0, 4, _backIndices, 0, 2);
            }

            _LEFT = view.Position.X - view.Origin.X < 0 ? 0 : (int)((view.Position.X - view.Origin.X) / _worldChunkSize);
            _RIGHT = (int)((view.Position.X + view.Origin.X) / _worldChunkSize) + 1;
            _RIGHT = _RIGHT > _chunks.Length ? _chunks.Length : _RIGHT;

            for (int x = _LEFT; x < _RIGHT; x ++ )
            {
                foreach (EffectPass p in _effect.CurrentTechnique.Passes)
                {
                    p.Apply();
                    _chunks[x].Render();
                }
            }
        }
    }

    /// <summary>
    /// Represents the material for a spot in the terrain
    /// </summary>
    public enum Materials : byte
    {
        Grass,
        Sand,
        Stone,
        Dirt
    }

    /// <summary>
    /// Represents the color scheme that the map uses
    /// </summary>
    public struct MapColorScheme
    {
        public Color GrassColor { get; set; }
        public Color DirtColor { get; set; }
        public Color SandColor { get; set; }
        public Color StoneColor { get; set; }
        public Color BottomColor { get; set; }

        /// <summary>
        /// The default, colored world
        /// </summary>
        public static readonly MapColorScheme Default = new MapColorScheme
        {
            GrassColor = Color.Green,
            DirtColor = Color.FromNonPremultiplied(90, 30, 0, 255),
            StoneColor = Color.LightGray,
            BottomColor = new Color(10,10,10),
            SandColor = Color.Tan
        };

        /// <summary>
        /// A black and gray world
        /// </summary>
        public static readonly MapColorScheme Sketch = new MapColorScheme
        {
            GrassColor = Color.Black,
            DirtColor = Color.LightGray,
            StoneColor = Color.LightGray,
            BottomColor = Color.LightGray,
            SandColor = Color.DarkGray
        };
    }
}
