using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Render.Particle
{
    /// <summary>
    /// Represents a particle engine
    /// </summary>
    public class ParticleEngine : IDisposable
    {
        const int MAX_PARTICLES = 100000;

        List<Particle> _particles = new List<Particle>();
        List<ParticleEmitter> _emitters = new List<ParticleEmitter>();

        GraphicsDevice _graphics;
        SpriteBatch _spriteBatch;

        /// <summary>
        /// Gets or sets the gravity on this particle engine
        /// </summary>
        public virtual Vector2 Gravity{ get; set;}
        /// <summary>
        /// Gets or sets the particle texture
        /// </summary>
        public virtual Texture2D ParticleTexture { get; set; }

        /// <summary>
        /// Creates a new particle engine
        /// </summary>
        /// <param name="graphics">The graphics device to bind to</param>
        public ParticleEngine(GraphicsDevice graphics)
        {
            Gravity = new Vector2(0, 9.81f);

            _graphics = graphics;
            _spriteBatch = new SpriteBatch(_graphics);
        }

        /// <summary>
        /// Adds an emitter to this particle engine
        /// </summary>
        /// <param name="emitter">The particle emitter</param>
        public void AddEmitter(ParticleEmitter emitter)
        {
            _emitters.Add(emitter);
        }

        /// <summary>
        /// Adds a particle to this particle engine
        /// </summary>
        /// <param name="position">The position of the particle</param>
        /// <param name="velocity">The initial velocity of the particle</param>
        /// <param name="scale">The scale of the particle</param>
        /// <param name="ttl">The time to live</param>
        /// <param name="color">The color of the particle</param>
        /// <param name="rotation">The initial rotation of the speed</param>
        /// <param name="angularVelocity">The angular velocity of this particle</param>
        public void AddParticle(Vector2 position, Vector2 velocity, float scale, float ttl, Color color, float rotation = 0, float angularVelocity = 0)
        {
            _particles.Add(new Particle(position, velocity, scale, ttl, color, rotation, angularVelocity));
        }
        
        /// <summary>
        /// Renders this particle system
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        public virtual void Render(GameTime gameTime)
        {
            _spriteBatch.Begin();
            foreach (Particle p in _particles)
            {
                _spriteBatch.Draw(ParticleTexture, p.Position, null, p.Color, p.Rotation, Vector2.Zero, p.Scale, SpriteEffects.None, 0F);
            }
            _spriteBatch.End();
        }

        /// <summary>
        /// Updates this particle system
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        public virtual void Update(GameTime gameTime)
        {
            foreach (ParticleEmitter e in _emitters)
            {
                e.Update(gameTime);
            }

            for (int i = 0; i < _particles.Count; i++)
            {
                _particles[i].Update(gameTime, Gravity);

                if (_particles[i].TTL <= 0)
                {
                    _particles.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// Disposes of this object and free's it's resources
        /// </summary>
        public void Dispose()
        {
            _spriteBatch.Dispose();
        }
    }
}
