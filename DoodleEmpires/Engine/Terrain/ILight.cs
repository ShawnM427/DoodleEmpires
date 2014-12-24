using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleEmpires.Engine.Terrain
{
    public abstract class ILight
    {
        private Color _color;
        private Vector2 _position;
        private Texture2D _texture;
        private float _intensity = 1.0f;

        protected GraphicsDevice _graphics;

        public virtual Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
            }

        }
        public virtual Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
            }
        }
        public virtual Texture2D Texture
        {
            get { return _texture; }
            protected set { _texture = value; }
        }
        public virtual Vector2 TextureOrigin
        {
            get;
            protected set;
        }
        public float Rotation { get; set; }
        public float Intensity 
        {
            get { return _intensity; }
            set { _intensity = value; }
        }

        public ILight(GraphicsDevice graphics)
        {
            _graphics = graphics;
        }
    }
}
