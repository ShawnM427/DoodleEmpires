using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Render.Particle
{
    public class ParticleEngine
    {
        const int MAX_PARTICLES = 100000;

        List<Particle> _particles = new List<Particle>();
        List<ParticleEmitter> _emitters = new List<ParticleEmitter>();

        GraphicsDevice _graphics;
        SpriteBatch _spriteBatch;

        public virtual Vector2 Gravity{ get; set;}
        public virtual Texture2D ParticleTexture { get; set; }

        public ParticleEngine(GraphicsDevice graphics)
        {
            Gravity = new Vector2(0, 9.81f);

            _graphics = graphics;
            _spriteBatch = new SpriteBatch(_graphics);
        }

        public void AddParticle(Vector2 position, Vector2 velocity, float scale, float ttl, Color color, float rotation = 0, float angularVelocity = 0)
        {
            _particles.Add(new Particle(position, velocity, scale, ttl, color, rotation, angularVelocity));
        }
        
        public virtual void Render(GameTime gameTime)
        {
            _spriteBatch.Begin();
            foreach (Particle p in _particles)
            {
                _spriteBatch.Draw(ParticleTexture, p.Position, null, p.Color, p.Rotation, Vector2.Zero, p.Scale, SpriteEffects.None, 0F);
            }
            _spriteBatch.End();
        }

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
    }
}
