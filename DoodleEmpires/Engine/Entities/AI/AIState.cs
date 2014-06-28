using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Entities.AI
{
    /// <summary>
    /// A generic class representing the current status of an AI
    /// </summary>
    /// <typeparam name="T">The AI tag data type</typeparam>
    public class AIState<T>
    {
        AIStatus _status;
        T _aiTag;

        /// <summary>
        /// Gets or sets the current AI tag
        /// </summary>
        public T AITag
        {
            get { return _aiTag; }
            set { _aiTag = value; }
        }
        /// <summary>
        /// Gets or sets the current AI status
        /// </summary>
        public AIStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        /// <summary>
        /// Creates a new blank AI state
        /// </summary>
        public AIState()
        {
            _status = AIStatus.Wander;
            _aiTag = default(T);
        }
    }

    /// <summary>
    /// A basic tag for using with AIState
    /// </summary>
    public class AITag
    {
        Vector2 _target;

        /// <summary>
        /// Gets or sets the target position
        /// </summary>
        public Vector2 Target
        {
            get { return _target; }
            set { _target = value; }
        }

        /// <summary>
        /// Creates a new blank AI tag
        /// </summary>
        public AITag()
        {
            _target = Vector2.Zero;
        }
    }

    /// <summary>
    /// Represents the status of an AI
    /// </summary>
    public enum AIStatus
    {
        /// <summary>
        /// The AI's current goal is to attack the position
        /// </summary>
        Attack,
        /// <summary>
        /// The AI's current goal is to defend the position
        /// </summary>
        Defend,
        /// <summary>
        /// The AI's current goal is to hide somewhere
        /// </summary>
        Hide,
        /// <summary>
        /// The AI's current goal is to wander around aimlessly
        /// </summary>
        Wander
    }
}
