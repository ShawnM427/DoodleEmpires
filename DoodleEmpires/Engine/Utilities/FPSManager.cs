using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DoodleEmpires.Engine.Utilities
{
    /// <summary>
    /// A static class for getting the framerate
    /// </summary>
    public static class FPSManager
    {
        /// <summary>
        /// The standard frame speed
        /// </summary>
        public static int FrameSpeed = 60;
        /// <summary>
        /// Gets the total number of frames that have been rendered
        /// </summary>
        public static long TotalFrames { get; private set; }
        /// <summary>
        /// Gets the averaged framerate
        /// </summary>
        public static double AverageFramesPerSecond { get; private set; }
        /// <summary>
        /// Gets the framerate as according to the current frame time
        /// </summary>
        public static double CurrentFramesPerSecond { get; private set; }

        static float _multiplier = 1.0F;
        /// <summary>
        /// Gets a multiplier that synchs tick rate to frame speed
        /// </summary>
        public static float Multiplier
        {
            get { return _multiplier; }
        }

        /// <summary>
        /// Gets the number of stored sample frames
        /// </summary>
        public const int MAXIMUM_SAMPLES = 100;

        private static Queue<double> _sampleBuffer = new Queue<double>();

        /// <summary>
        /// Called when the application draws
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        public static void OnDraw(GameTime gameTime)
        {
            CurrentFramesPerSecond = 1.0 / gameTime.ElapsedGameTime.TotalSeconds;

            _sampleBuffer.Enqueue(CurrentFramesPerSecond);

            if (_sampleBuffer.Count > MAXIMUM_SAMPLES)
            {
                _sampleBuffer.Dequeue();
                AverageFramesPerSecond = _sampleBuffer.Average(i => i);
            }
            else
            {
                AverageFramesPerSecond = CurrentFramesPerSecond;
            }
            _multiplier = FrameSpeed / (float)AverageFramesPerSecond;

            TotalFrames++;
        }
    }
}
