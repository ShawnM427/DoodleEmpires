using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleEmpires.Engine.Terrain.Brushes
{
    public class TexturePixelBrush : IPixelBrush
    {
        Color[] _alphaMaskArray;

        Texture2D _alphaMask;

        public Texture2D AlphaMask
        {
            get { return _alphaMask; }
            set { _alphaMask = value; }
        }

        public TexturePixelBrush(Texture2D alphaMask)
        {
            Texture = alphaMask;
            //_alphaMask = alphaMask;
            //_alphaMaskArray = new Color[alphaMask.Width * alphaMask.Height];
            //_alphaMask.GetData<Color>(_alphaMaskArray);

            Width = alphaMask.Width;
            Height = alphaMask.Height;
        }

        public override Color Sample(int x, int y)
        {
            return _alphaMaskArray[y * _alphaMask.Width + x];
        }
    }
}
