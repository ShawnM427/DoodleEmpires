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
        /// <summary>
        /// The color of this particle
        /// </summary>
        public Color Color;
        /// <summary>
        /// The scale of this particle
        /// </summary>
        public float Scale;
        /// <summary>
        /// How long this particle has to live, in seconds
        /// </summary>
        public double TTL;

        /// <summary>
        /// Creates a new particle
        /// </summary>
        /// <param name="position">The position of the particle</param>
        /// <param name="velocity">The velocity of the particle</param>
        /// <param name="scale">The scale of the particle</param>
        /// <param name="ttl">The time to live for this particle in seconds</param>
        /// <param name="color">The color of this particle</param>
        /// <param name="rotation">The initial rotation of this particle</param>
        /// <param name="angularVelocity">The angula velocity of this particle</param>
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

        /// <summary>
        /// Updates this particle
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        /// <param name="gravity">The current gravity</param>
        public void Update(GameTime gameTime, Vector2 gravity)
        {
            TTL -= 1.0;
            Velocity += gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += Velocity;
        }

        /// <summary>
        /// Checks equality between two particles
        /// </summary>
        /// <param name="a">The particle</param>
        /// <param name="b">The particle to check against</param>
        /// <returns>True if the particles are equal</returns>
        public static bool operator ==(Particle a, Particle b)
        {
            return a.Position == b.Position & a.Rotation == b.Rotation & 
                   a.Velocity == b.Velocity & a.AngularVelocity == b.AngularVelocity;
        }

        /// <summary>
        /// Checks inequality between two particles
        /// </summary>
        /// <param name="a">The particle</param>
        /// <param name="b">The particle to check against</param>
        /// <returns>True if the particles are not equal</returns>
        public static bool operator !=(Particle a, Particle b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Checks if this particle is equal to an object
        /// </summary>
        /// <param name="obj">The object to check against</param>
        /// <returns>True if the object is equal to this particle</returns>
        public override bool Equals(object obj)
        {
            return obj.GetType() == typeof(Particle) && (Particle)obj == this;
        }

        /// <summary>
        /// Gets a unique hashcode for this particle
        /// </summary>
        /// <returns>A unique hashcode</returns>
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
