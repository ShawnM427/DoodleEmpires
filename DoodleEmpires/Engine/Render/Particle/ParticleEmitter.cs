using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Render.Particle
{
    public class ParticleEmitter
    {
        public Vector2 GeneralDirection { get; set;}
        public float Force { get; set; }
        public int ParticlesPerSecond { get; set; }
        public Vector2 Position { get; set; }
        public float RotationalVelocity { get; set; }
        public float TTL { get; set; }
        public Color Color { get; set; }

        ParticleEngine _engine;
        float _elapsedTime;
        Random _random;

        public ParticleEmitter(ParticleEngine engine, Vector2 position)
        {
            _engine = engine;
            Position = position;
            GeneralDirection = new Vector2(0, 0);
            Force = 0.0F;
            ParticlesPerSecond = 1;
            RotationalVelocity = 0.0F;
            TTL = 5.0F;
            Color = Color.White;

            _random = new Random();
        }

        public void Update(GameTime gameTime)
        {
            _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_elapsedTime >= 1.0F / ParticlesPerSecond)
            {
                _engine.AddParticle(Position, 
                    GeneralDirection * Force * ((float)_random.NextDouble() + 0.5F),
                    (float)_random.NextDouble() + 0.5F, TTL + (float)_random.NextDouble() / 2.0F,
                    Color, (float)_random.NextDouble() * 6.28f, (float)_random.NextDouble());
            }
        }
    }
}
