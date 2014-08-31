using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleEmpires.Engine.Terrain
{
    public class PixelBrush
    {
        Color _eraseColor = Color.White;
        Vector2 _halfSize;

        Color[] _alphaMaskArray;

        Texture2D _alphaMask;

        public Texture2D AlphaMask
        {
            get { return _alphaMask; }
            set { _alphaMask = value; }
        }

        public int Width
        {
            get { return _alphaMask.Width; }            
        }
        public int Height
        {
            get { return _alphaMask.Height; }
        }

        public Vector2 HalfSize
        {
            get { return _halfSize; }
        }

        public Color EraseColor
        {
            get { return _eraseColor; }
            set { _eraseColor = value; }
        }

        public Color this[int x, int y]
        {
            get { return Sample(x, y); }
        }

        public PixelBrush(Texture2D alphaMask)
        {
            _alphaMask = alphaMask;
            _alphaMaskArray = new Color[alphaMask.Width * alphaMask.Height];
            _alphaMask.GetData<Color>(_alphaMaskArray);

            _halfSize = new Vector2(_alphaMask.Width / 2.0f, _alphaMask.Height / 2.0f);
        }

        public Color Sample(int x, int y)
        {
            return _alphaMaskArray[y * _alphaMask.Width + x];
        }
    }
}
