using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleEmpires.Engine.Terrain
{
    public class PixelTerrain
    {
        PixelRegion[,] _regions;

        int _xRegions;
        int _yRegions;
        int _width;
        int _height;

        List<ILight> _lights;
        Color _ambientColor = Color.Black;
        GraphicsDevice _graphics;
        ColorLookup _ambienceLookup;
        Texture2D _nightTexture;
        SpriteBatch _batcher;

        TimeSpan _dayCycleTimer = new TimeSpan(0, 0, 15);
        TimeSpan _currentTime = new TimeSpan(0, 0, 0);

        public Color AmbientColor
        {
            get { return _ambientColor; }
            set
            {
                _ambientColor = value;
                UpdateLighting();
            }
        }
        
        public PixelTerrain(int width, int height, GraphicsDevice graphics, TerrainBuilder terrainBuilder, Texture2D dayNightGradient, Texture2D nightTexture)
        {
            if (width % PixelRegion.SIZE != 0 | height % PixelRegion.SIZE != 0)
                throw new ArgumentException("Width and height must both be mutliples of " + PixelRegion.SIZE);

            _width = width;
            _height = height;
            _xRegions = width / PixelRegion.SIZE;
            _yRegions = height / PixelRegion.SIZE;

            _nightTexture = nightTexture;

            _graphics = graphics;

            _batcher = new SpriteBatch(_graphics);

            _lights = new List<ILight>();

            _regions = new PixelRegion[_xRegions, _yRegions];

            for(int xx = 0; xx < _xRegions; xx ++)
            {
                for(int yy = 0; yy < _yRegions; yy++)
                {
                    _regions[xx, yy] = new PixelRegion(new Point(xx * PixelRegion.SIZE, yy * PixelRegion.SIZE), graphics, nightTexture);
                    _regions[xx, yy].ApplyTerrainBuilder(terrainBuilder);
                }
            }
            Color[] ambienceData = new Color[dayNightGradient.Width * dayNightGradient.Height];
            dayNightGradient.GetData<Color>(ambienceData);
            _ambienceLookup = new ColorLookup(ambienceData);
        }

        protected virtual void LightChanged(object sender, EventArgs e)
        {
            ILight light = (ILight)sender;

            _lights.Remove(light);
            AddLight(light);
        }

        public virtual void Render()
        {
            _graphics.SetRenderTarget(null);
            _graphics.Clear(AmbientColor);

            _batcher.Begin();

            for (int xx = 0; xx < _xRegions; xx++)
            {
                for (int yy = 0; yy < _yRegions; yy++)
                {
                    _regions[xx, yy].Render(_batcher);
                }
            }

            _batcher.End();
        }

        public virtual void Update(GameTime gameTime)
        {
            if (Global.GameSpeed > 0)
            {
                _currentTime += TimeSpan.FromSeconds(gameTime.ElapsedGameTime.TotalSeconds * Global.GameSpeed);
                Global.TimeOfDay = ((float)(Math.Cos(((2 * _currentTime.TotalSeconds) / _dayCycleTimer.TotalSeconds) * Math.PI) + 1.0f) / 2.0f) * Global.GameSpeed;
                AmbientColor = _ambienceLookup.Sample(Global.TimeOfDay);

                foreach (PixelRegion region in _regions)
                    region.Update();
            }
        }

        public virtual void ApplyBrush(IPixelBrush brush, Vector2 pos, float rotation = 0)
        {
            for (int xx = 0; xx < _xRegions; xx++)
            {
                for (int yy = 0; yy < _yRegions; yy++)
                {
                    _regions[xx, yy].ApplyBrush(brush, pos, rotation);
                }
            }
        }

        public virtual void AddLight(ILight light)
        {
            _lights.Add(light);
        }

        public virtual void AddLights(IEnumerable<ILight> lights)
        {
            _lights.AddRange(lights);
        }

        protected void UpdateLighting()
        {
            foreach (PixelRegion region in _regions)
                region.RenderLights(_lights, _ambientColor);
        }
    }
}
