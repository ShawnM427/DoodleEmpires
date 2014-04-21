using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Render.Particle
{
    /// <summary>
    /// Represents an instance of a particle
    /// </summary>
    public struct Particle
    {
        /// <summary>
        /// Gets or sets this particles' position
        /// </summary>
        public Vector2 Position;
        /// <summary>
        /// Gets or sets this particles' velocity
        /// </summary>
        public Vector2 Velocity;
        /// <summary>
        /// Gets or sets this particles' rotation
        /// </summary>
        public float Rotation;
        /// <summary>
        /// Gets or sets this particles' angular velocity
        /// </summary>
        public float AngularVelocity;
        public Color Color;
        public float Scale;
        public double TTL;

        public Particle(Vector2 position, Vector2 velocity, float scale, float ttl, Color color, float rotation = 0, float angularVelocity = 0)
        {
            Position = position;
            Velocity = velocity;
            Rotation = rotation;
            AngularVelocity = angularVelocity;
            Scale = scale;
            TTL = ttl;
            Color = color;
        }

        public void Update(GameTime gameTime, Vector2 gravity)
        {
            TTL -= 1.0;
            Velocity += gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += Velocity;
        }

        public static bool operator ==(Particle a, Particle b)
        {
            return a.Position == b.Position & a.Rotation == b.Rotation & 
                   a.Velocity == b.Velocity & a.AngularVelocity == b.AngularVelocity;
        }

        public static bool operator !=(Particle a, Particle b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj.GetType() == typeof(Particle) && (Particle)obj == this;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + Position.GetHashCode();
            hash = (hash * 7) + Rotation.GetHashCode();
            hash = (hash * 7) + Velocity.GetHashCode();
            hash = (hash * 7) + AngularVelocity.GetHashCode();
            return hash;
        }
    }
}
