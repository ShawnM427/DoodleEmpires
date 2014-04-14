using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Drawing;

namespace DoodleAnims.Lib.Anim
{
    /// <summary>
    /// Represents an animation for a skeleton
    /// </summary>
    public class Animation
    {
        Skeleton _skeleton;
        List<AnimKeyFrame> _keyFrames;

        double _currentTime = 0;
        int _currentFrame = 0;
        bool _running = false;
        bool _isReversed = false;
        AnimKeyFrame _currentKeyFrame;
        double _length = 0;
        float _playSpeed = 1.0f;

        /// <summary>
        /// Gets the length of this animation
        /// </summary>
        public TimeSpan Length
        {
            get { return new TimeSpan(0, 0, 0, 0, (int)_length); }
        }
        /// <summary>
        /// Gets the current frame of the animation
        /// </summary>
        public AnimKeyFrame CurrentFrame
        {
            get { return _currentKeyFrame; }
        }
        /// <summary>
        /// Gets or sets the current time in the animation
        /// </summary>
        public TimeSpan CurrentTime
        {
            get { return new TimeSpan(0, 0, 0, 0, (int)_currentTime); }
            set
            {
                _currentTime = value.TotalMilliseconds;
                _currentTime = _currentTime > _length ? _length : _currentTime;
            }
        }
        /// <summary>
        /// Gets or sets whether this animation is playing in reverse
        /// </summary>
        public bool IsReversed
        {
            get { return _isReversed; }
            set { _isReversed = value; }
        }
        /// <summary>
        /// Gets or sets a multiplier for the playing speed
        /// </summary>
        public float PlaySpeed
        {
            get { return _playSpeed; }
            set { _playSpeed = value; }
        }

        /// <summary>
        /// Creates a new animation
        /// </summary>
        /// <param name="skeleton">The skeleton to bind to</param>
        public Animation(Skeleton skeleton)
        {
            _skeleton = skeleton;
            _keyFrames = new List<AnimKeyFrame>();
        }

        /// <summary>
        /// Adds a keyframe to this animation
        /// </summary>
        /// <param name="frame">The frame to add</param>
        public void AddKeyFrame(AnimKeyFrame frame)
        {
            _keyFrames.Add(frame);
            _keyFrames.Sort(SortByTime);
            _length = _keyFrames.Last().Time;
        }
        
        /// <summary>
        /// Removes a keyrame from this animation
        /// </summary>
        /// <param name="frame">The frame to remove</param>
        /// <returns>true if sucessfull</returns>
        public bool RemoveKeyFrame(AnimKeyFrame frame)
        {
            bool opp = _keyFrames.Remove(frame);
            if (opp)
            {
                _keyFrames.Sort(SortByTime);
                _length = _keyFrames.Last().Time;
            }
            return opp;
        }

        /// <summary>
        /// Gets the sort ID for two frames based on time
        /// </summary>
        /// <param name="X">The first frame</param>
        /// <param name="Y">The second frame</param>
        /// <returns></returns>
        private int SortByTime(AnimKeyFrame X, AnimKeyFrame Y)
        {
            return X.Time.CompareTo(Y.Time);
        }
        
        /// <summary>
        /// Restarts and plays this animation
        /// </summary>
        public void Restart()
        {
            _currentTime = 0;
            _currentFrame = 0;
            _running = true;
        }

        /// <summary>
        /// Begins playing this animation
        /// </summary>
        public void Play()
        {
            _running = true;
        }

        /// <summary>
        /// Pauses this animation
        /// </summary>
        public void Pause()
        {
            _running = false;
        }

        /// <summary>
        /// Stops this animation and sets it back to the start
        /// </summary>
        public void Stop()
        {
            _currentTime = 0;
            _currentFrame = 0;
            _running = false;
        }

        /// <summary>
        /// Update the animation timeline.
        /// </summary>
        /// <param name="elapsedTime">The time since the last frame was displayed</param>
        public void Update(TimeSpan elapsedTime)
        {
            if (_running)
            {
                if (_currentTime >= _length | _currentTime < 0)
                {
                    Reset();
                    return;
                }

                _currentTime += elapsedTime.TotalMilliseconds * (_isReversed ? -1 : 1) * _playSpeed;

                _currentKeyFrame = AnimKeyFrame.Lerp( _keyFrames[_currentFrame], _keyFrames[_currentFrame + 1],
                (float)_currentTime / (float)_keyFrames[_currentFrame + 1].Time);

                if (_currentTime >= _keyFrames[_currentFrame + 1].Time && _currentFrame != _keyFrames.Count)
                    _currentFrame++;

                _skeleton.ApplyFrame(_currentKeyFrame);
            }
        }

        /// <summary>
        /// Resets the animation to it's initial state
        /// </summary>
        public void Reset()
        {
            _currentTime = 0;
            _currentFrame = 0;
            _running = false;
            _isReversed = false;
            _skeleton.ApplyFrame(_keyFrames[0]);
        }
    }
}
