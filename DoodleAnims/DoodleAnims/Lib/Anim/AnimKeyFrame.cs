using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DoodleAnims.Lib.Anim
{
    /// <summary>
    /// Represents a keyframe in an animation
    /// </summary>
    public class AnimKeyFrame
    {
        AnimState[] _keyStates;
        double _time;

        /// <summary>
        /// Gets the states for this frame
        /// </summary>
        public AnimState[] KeyStates
        {
            get { return _keyStates; }
        }
        /// <summary>
        /// Gets the time for this key frame in milliseconds
        /// </summary>
        public double Time
        {
            get { return _time; }
        }

        /// <summary>
        /// Creates a new animation keyframe
        /// </summary>
        /// <param name="limbs">The list of limbs to build this keyframe for</param>
        /// <param name="time">The time in milliseconds for this keyframe</param>
        public AnimKeyFrame(Limb[] limbs, double time)
        {
            _keyStates = new AnimState[limbs.Length];

            for (int i = 0; i < limbs.Length; i++)
            {
                if (limbs[i] != null)
                {
                    _keyStates[i] = new AnimState(limbs[i]);
                }
            }

            _time = time;
        }

        private AnimKeyFrame(AnimState[] states, double time)
        {
            _keyStates = states;
            _time = time;
        }

        /// <summary>
        /// Adds a limb refrence to this animation keyframe
        /// </summary>
        /// <param name="limb">The limb to add</param>
        public void AddLimbRef(Limb limb)
        {
            if (_keyStates.Length - 1 < limb.ID)
                Array.Resize<AnimState>(ref _keyStates, limb.ID + 1);

            _keyStates[limb.ID] = new AnimState(limb);
        }

        /// <summary>
        /// Removes a limb refrence to this animation keyframe
        /// </summary>
        /// <param name="limb">The limb to add</param>
        public void RemoveLimbRef(Limb limb)
        {
            if (_keyStates.Length > limb.ID & limb.ID != 0)
            {
                _keyStates[limb.ID] = null;
            }
        }

        /// <summary>
        /// Performs linear interpolation between two keyframes
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <param name="percent">The percent from min to max, from 0 to 1</param>
        /// <returns>A keyframe lerped from min-max</returns>
        public static AnimKeyFrame Lerp(AnimKeyFrame min, AnimKeyFrame max, float percent)
        {
            if (min._keyStates.Length == max._keyStates.Length)
            {
                AnimState[] kStates = new AnimState[min._keyStates.Length];

                for (int i = 0; i < kStates.Length; i++)
                {
                    if (min._keyStates[i] != null & max._keyStates[i] != null)
                    {
                        kStates[i] = AnimState.Lerp(min._keyStates[i], max._keyStates[i], percent);
                    }
                    else
                    {
                        throw new InvalidOperationException("The min and max keyframes must be from the same skeleton!");
                    }
                }

                return new AnimKeyFrame(kStates, Math2.Lerp(min._time, max._time, percent));
            }
            else
            {
                throw new InvalidOperationException("The min and max keyframes must be from the same skeleton!");
            }
        }
    }
}
