using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Entities
{
    /// <summary>
    /// Represents the state of a single unit
    /// </summary>
    public struct UnitState
    {
        Vector2 _position;
        byte _teamID;
        UnitStatus _status;
        float _hp;

        /// <summary>
        /// Gets or sets the position of this unit
        /// </summary>
        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }
        /// <summary>
        /// Gets or sets the x coord of this unit
        /// </summary>
        public float X
        {
            get { return _position.X; }
            set { _position.X = value; }
        }
        /// <summary>
        /// Gets or sets the y coord of this unit
        /// </summary>
        public float Y
        {
            get { return _position.Y; }
            set { _position.Y = value; }
        }

        /// <summary>
        /// Gets or sets this unit state's health
        /// </summary>
        public float HP
        {
            get { return _hp; }
            set { _hp = value; }
        }
        
        /// <summary>
        /// Gets or sets the team ID for this unit state
        /// </summary>
        public byte TeamID
        {
            get { return _teamID; }
            set { _teamID = value; }
        }

        /// <summary>
        /// Gets or sets this unit's status
        /// </summary>
        public UnitStatus Stance
        {
            get { return _status; }
            set { _status = value; }
        }

        /// <summary>
        /// Creates a new unit state
        /// </summary>
        /// <param name="teamID">The team ID for this unit</param>
        /// <param name="hp">The starting HP of this unit</param>
        public UnitState(byte teamID, float hp)
        {
            _teamID = teamID;
            _hp = hp;

            _position = Vector2.Zero;
            _status = UnitStatus.Standing;
        }
    }

    /// <summary>
    /// Represents the status of a unit
    /// </summary>
    public enum UnitStatus
    {
        /// <summary>
        /// This unit has no special status, something went wrong
        /// </summary>
        None,
        /// <summary>
        /// This unit is currently standing
        /// </summary>
        Standing,
        /// <summary>
        /// This unit is currently walking
        /// </summary>
        Walking,
        /// <summary>
        /// This unit is currently hiding
        /// </summary>
        Hiding,
        /// <summary>
        /// This unit is currently climbing
        /// </summary>
        Climbing,
        /// <summary>
        /// This unit is currently attacking
        /// </summary>
        Attacking,
        /// <summary>
        /// This unit is currently defending itself
        /// </summary>
        Defending,
        /// <summary>
        /// This unit is currently reloading
        /// </summary>
        Reloading,
        /// <summary>
        /// This unit is in the process of dying
        /// </summary>
        Dying,
        /// <summary>
        /// This unit is dead
        /// </summary>
        Dead
    }
}
