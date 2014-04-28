using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using DoodleEmpires.Engine.Render;
using Microsoft.Xna.Framework;
using DoodleEmpires.Engine.Utilities;

namespace DoodleEmpires.Engine.Terrain
{
    public class ChunkedTerrainRenderer
    {
        const int CHUNK_WIDTH = 16;
        const int CHUNK_HEIGHT = 16;

        RenderChunk<VertexPositionColorTexture>[,] _chunks = new RenderChunk<VertexPositionColorTexture>[0,0];
        TextureAtlas _atlas;
        TileManager _tileManager;
        BasicEffect _effect;
        GraphicsDevice _graphics;
        SPMap _terrain;

        public byte this[int x, int y]
        {
            get { return _terrain[x, y]; }
            set
            {
                _terrain[x, y] = value;
                RebuildChunk(x / CHUNK_WIDTH, y / CHUNK_HEIGHT);
            }
        }

        public ChunkedTerrainRenderer(SPMap terrain)
        {
            _graphics = terrain.Graphics;
            _effect = new BasicEffect(_graphics);
            _atlas = terrain.Atlas;
            _tileManager = terrain.TileManager;
            _terrain = terrain;
        }

        private void InitializeChunk(int chunkX, int chunkY)
        {
            VertexPositionColorTexture[] verts = new VertexPositionColorTexture[CHUNK_WIDTH * CHUNK_HEIGHT * 4];
            short[] indices = new short[CHUNK_WIDTH * CHUNK_HEIGHT * 6];

            short i = 0;
            short i2 = 0;
            for (int y = 0; y < CHUNK_HEIGHT; y++)
            {
                for (int x = 0; x < CHUNK_WIDTH; x++)
                {
                    indices[i2] = i;
                    indices[i2 + 1] = (short)(i + 1);
                    indices[i2 + 2] = (short)(i + 2);
                    indices[i2 + 3] = (short)(i + 1);
                    indices[i2 + 4] = (short)(i + 2);
                    indices[i2 + 5] = (short)(i + 3);
                    i2 += 6;

                    for (int xx = 0; xx < 2; xx++)
                    {
                        for (int yy = 0; yy < 2; yy++)
                        {
                            verts[i] = new VertexPositionColorTexture(new Vector3(
                                    (float)(chunkX * CHUNK_WIDTH + x) * SPMap.TILE_WIDTH,
                                    (float)(chunkY * CHUNK_WIDTH + y) * SPMap.TILE_HEIGHT, 0),
                                    _tileManager.Tiles[_terrain[chunkX * CHUNK_WIDTH + x, chunkY * CHUNK_WIDTH + y]].Color, 
                                    Vector2.Zero);
                            i ++;
                        }
                    }
                }
            }

            _chunks[chunkX, chunkY].SetVertexBuffer(verts);
            _chunks[chunkX, chunkY].SetIndexBuffer(indices);
        }

        private void RebuildChunk(int chunkX, int chunkY)
        {
            VertexPositionColorTexture[] verts = new VertexPositionColorTexture[CHUNK_WIDTH * CHUNK_HEIGHT * 4];
            int i = 0;
            for (int y = 0; y < CHUNK_HEIGHT; y++)
            {
                for (int x = 0; x < CHUNK_WIDTH; x++)
                {
                    for (int xx = 0; xx < 2; xx++)
                    {
                        for (int yy = 0; yy < 2; yy++)
                        {
                            verts[i] = new VertexPositionColorTexture(new Vector3(
                                    (float)(chunkX * CHUNK_WIDTH + x) * SPMap.TILE_WIDTH,
                                    (float)(chunkY * CHUNK_WIDTH + y) * SPMap.TILE_HEIGHT, 0),
                                    _tileManager.Tiles[_terrain[chunkX * CHUNK_WIDTH + x, chunkY * CHUNK_WIDTH + y]].Color, 
                                    Vector2.Zero);
                            i ++;
                        }
                    }
                }
            }

            _chunks[chunkX, chunkY].SetVertexBuffer(verts);
        }

        public void Render()
        {
            _effect.CurrentTechnique.Passes[0].Apply();

            foreach (RenderChunk<VertexPositionColorTexture> chunk in _chunks)
                chunk.Render();
        }
    }
}
