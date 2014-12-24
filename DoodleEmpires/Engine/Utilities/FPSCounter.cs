using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleEmpires.Engine
{
    public class FPSCounter
    {
        protected const int MAX_CACHED_FRAMES = 60;

        Queue<double> _frames;
        double _frameRate;
        TimeSpan _targetElapsedTime;

        public double FrameRatePercent
        {
            get { return _frameRate; }
        }
        public int FramesPerSecond
        {
            get { return (int)(_frameRate * (1 / _targetElapsedTime.TotalSeconds)); }
        }

        public FPSCounter(TimeSpan targetElapsedTime)
        {
            _frames = new Queue<double>();
            _targetElapsedTime = targetElapsedTime;
        }

        public void AddFrameToBuffer(TimeSpan elapsedGameTime)
        {
            _frames.Enqueue(_targetElapsedTime.TotalSeconds / elapsedGameTime.TotalSeconds);

            if (_frames.Count > MAX_CACHED_FRAMES)
                _frames.Dequeue();

            _frameRate = _frames.Average();
        }
    }
}
