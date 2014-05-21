using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Entities.AI
{
    public class AIState<T>
    {
        AIStatus _status;
        T _aiTag;

        public T AITag
        {
            get { return _aiTag; }
            set { _aiTag = value; }
        }
        public AIStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public AIState()
        {
            _status = AIStatus.Wander;
            _aiTag = default(T);
        }
    }

    public class AITag
    {
        Vector2 _target;

        public Vector2 Target
        {
            get { return _target; }
            set { _target = value; }
        }

        public AITag()
        {
            _target = Vector2.Zero;
        }
    }

    public enum AIStatus
    {
        Attack,
        Defend,
        Hide,
        Wander
    }
}
