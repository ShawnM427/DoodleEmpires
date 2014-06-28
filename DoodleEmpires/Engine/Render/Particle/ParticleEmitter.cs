using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Render.Particle
{
    /// <summary>
    /// Represents an emitter of particles
    /// </summary>
    public class ParticleEmitter
    {
        /// <summary>
        /// The general direction to fire particles
        /// </summary>
        public Vector2 GeneralDirection { get; set;}
        /// <summary>
        /// The force to eject particles with
        /// </summary>
        public float Force { get; set; }
        /// <summary>
        /// The particles to emit per second
        /// </summary>
        public int ParticlesPerSecond { get; set; }
        /// <summary>
        /// The position of this emitter
        /// </summary>
        public Vector2 Position { get; set; }
        /// <summary>
        /// The initial rotational velocity of particles
        /// </summary>
        public float RotationalVelocity { get; set; }
        /// <summary>
        /// The default time to live for particles
        /// </summary>
        public float TTL { get; set; }
        /// <summary>
        /// The default color for particles
        /// </summary>
        public Color Color { get; set; }

        ParticleEngine _engine;
        float _elapsedTime;
        Random _random;

        /// <summary>
        /// Creates a new particle emitter
        /// </summary>
        /// <param name="engine">The particle engine to bind to</param>
        /// <param name="position">The POsition of this emitter</param>
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

        /// <summary>
        /// Updates this particle emitter
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        public void Update(GameTime gameTime)
        {
            _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_elapsedTime >= 1.0F / ParticlesPerSecond)
            {
                _engine.AddParticle(Position, 
                    GeneralDirection * Force * ((float)_random.NextDouble() + 0.5F),
                    (float)_random.NextDouble() + 0.5F, TTL + (float)_random.NextDouble() / 8.0F,
                    Color, (float)_random.NextDouble() * 6.28f, (float)_random.NextDouble());
            }
        }
    }
}
