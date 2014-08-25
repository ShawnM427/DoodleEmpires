using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoodleAnims.Lib.Anim;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Timers;
using WinFormsTools;

namespace DoodleAnims
{
    /// <summary>
    /// Represents a control for editing animations
    /// </summary>
    public class AnimController : DoubleBufferedPanel
    {
        Animation _currentAnim;
        /// <summary>
        /// Gets or sets the animation to work on
        /// </summary>
        public Animation Animation
        {
            get { return _currentAnim; }
            set
            {
                _currentAnim = value;
                Invalidate();
            }
        }
        /// <summary>
        /// Gets or sets the color for the line
        /// </summary>
        public Color ButtonColor
        {
            get { return _buttonBackColor; }
            set
            {
                _buttonBackColor = value;
                _buttonBrush = new SolidBrush(value);
            }
        }
        /// <summary>
        /// Gets or sets the color for the line
        /// </summary>
        public Color LineColor
        {
            get { return _lineColor; }
            set
            {
                _lineColor = value;
                _linePen.Color = value;
                _dottedLinePen.Color = value;
            }
        }

        double _shownTime = 1;

        double _minTime = 0;
        double _maxTime = 1.0;

        Rectangle _playButton;
        Rectangle _pauseButton;
        Rectangle _stopButton;
        Rectangle _reverseButton;

        Rectangle _panLeft;
        Rectangle _panRight;

        Rectangle _zoomIn;
        Rectangle _zoomOut;

        Point _keyframeTopBorderLeft;
        Point _keyframeTopBorderRight;

        Point _keyframeBottomBorderLeft;
        Point _keyframeBottomBorderRight;

        Point _timeTopBorderLeft;
        Point _timeTopBorderRight;

        Color _buttonBackColor = Color.White;
        Color _lineColor = Color.Black;
        Brush _buttonBrush;
        Pen _linePen;
        Pen _dottedLinePen;

        Timer _animTimer;

        float _timeWidth = 0;
        int _oppKeyframeHeight = 0;
        int _panButtonWidth = 0;

        /// <summary>
        /// Creates a new animation control
        /// </summary>
        public AnimController()
        {
            DoubleBuffered = true;

            OnResize(EventArgs.Empty);

            _buttonBrush = new SolidBrush(_buttonBackColor);
            _linePen = new Pen(new SolidBrush(Color.Black));
            _dottedLinePen = new Pen(new SolidBrush(Color.Black));
            _dottedLinePen.DashStyle = DashStyle.Dot;

            _animTimer = new Timer(10);
            _animTimer.Elapsed += new ElapsedEventHandler(_animTimer_Elapsed);
            _animTimer.Start();
        }

        void _animTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_currentAnim != null)
                _currentAnim.Update(new TimeSpan(0,0,0,0, (int)_animTimer.Interval));
        }

        /// <summary>
        /// Called when this control has been resized
        /// </summary>
        /// <param name="eventargs">A blank event arguments</param>
        protected override void OnResize(EventArgs eventargs)
        {
            _oppKeyframeHeight = Height - Height / 4;
            _panButtonWidth = Height / 4;
            _timeWidth = Width - Height - Height / 2;

            _pauseButton = new Rectangle(Height / 2, 0, Height / 2, Height / 2);
            _playButton = new Rectangle(0, 0, Height / 2, Height / 2);

            _reverseButton = new Rectangle(0, Height / 2, Height / 2, Height / 2 - 1);
            _stopButton = new Rectangle(Height / 2, Height / 2, Height / 2, Height / 2 - 1);

            _panLeft = new Rectangle(Height, 0, _panButtonWidth, _oppKeyframeHeight);
            _panRight = new Rectangle(Width - _panButtonWidth - 1, 0, _panButtonWidth, _oppKeyframeHeight);

            _zoomIn = new Rectangle(Height, _oppKeyframeHeight, _panButtonWidth, _panButtonWidth);
            _zoomOut = new Rectangle(Width - _panButtonWidth - 1, _oppKeyframeHeight, _panButtonWidth, _panButtonWidth);

            _keyframeTopBorderLeft = new Point(Height + _panButtonWidth, _oppKeyframeHeight);
            _keyframeTopBorderRight = new Point(Right - 1 - _panButtonWidth, _oppKeyframeHeight);

            _timeTopBorderLeft = new Point(Height + _panButtonWidth, 0);
            _timeTopBorderRight = new Point(Right - 1, 0);

            _keyframeBottomBorderLeft = new Point(Height, Height - 1);
            _keyframeBottomBorderRight = new Point(Right - 1, Height - 1);

            base.OnResize(eventargs);
        }

        /// <summary>
        /// Paints this control to the screen
        /// </summary>
        /// <param name="e">The paint arguments</param>
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TranslateTransform(Height + Height / 4, 0);
            for (double t = _minTime; t < _maxTime; t += _shownTime / 10.0)
            {
                float x = (float)((t - _minTime) * _timeWidth / _shownTime);
                float lineDepth = (int)(t % 4.0) == 0 ? 10 : 20;
                e.Graphics.DrawString(Math.Round(t, 1).ToString(), Font, new SolidBrush(Color.Black), new PointF(x, 0));
                e.Graphics.DrawLine(_linePen, new PointF(x, 10), new PointF(x, _oppKeyframeHeight - lineDepth));
            }
            e.Graphics.ResetTransform();

            #region DrawButtons
            e.Graphics.FillRectangle(_buttonBrush, _playButton);
            e.Graphics.FillRectangle(_buttonBrush, _stopButton);
            e.Graphics.FillRectangle(_buttonBrush, _pauseButton);
            e.Graphics.FillRectangle(_buttonBrush, _reverseButton);
            e.Graphics.FillRectangle(_buttonBrush, _panLeft);
            e.Graphics.FillRectangle(_buttonBrush, _panRight);
            e.Graphics.FillRectangle(_buttonBrush, _zoomIn);
            e.Graphics.FillRectangle(_buttonBrush, _zoomOut);

            e.Graphics.DrawImage(AnimControllerFiles.PlayButton, _playButton);
            e.Graphics.DrawImage(AnimControllerFiles.StopButton, _stopButton);
            e.Graphics.DrawImage(AnimControllerFiles.PauseButton, _pauseButton);
            e.Graphics.DrawImage(AnimControllerFiles.ReverseButton, _reverseButton);
            e.Graphics.DrawImage(AnimControllerFiles.PanLeft, _panLeft);
            e.Graphics.DrawImage(AnimControllerFiles.PanRight, _panRight);
            e.Graphics.DrawImage(AnimControllerFiles.ZoomIn, _zoomIn);
            e.Graphics.DrawImage(AnimControllerFiles.ZoomOut, _zoomOut);

            e.Graphics.DrawRectangle(_linePen, _playButton);
            e.Graphics.DrawRectangle(_linePen, _stopButton);
            e.Graphics.DrawRectangle(_linePen, _pauseButton);
            e.Graphics.DrawRectangle(_linePen, _reverseButton);
            e.Graphics.DrawRectangle(_linePen, _panLeft);
            e.Graphics.DrawRectangle(_linePen, _panRight);
            e.Graphics.DrawRectangle(_linePen, _zoomIn);
            e.Graphics.DrawRectangle(_linePen, _zoomOut);
            #endregion

            e.Graphics.DrawLine(_linePen, _keyframeTopBorderLeft, _keyframeTopBorderRight);
            e.Graphics.DrawLine(_linePen, _keyframeBottomBorderLeft, _keyframeBottomBorderRight);
            e.Graphics.DrawLine(_linePen, _timeTopBorderLeft, _timeTopBorderRight);
            e.Graphics.DrawLine(_linePen, _timeTopBorderRight, _keyframeBottomBorderRight);
        }

        /// <summary>
        /// Occurs when the mouse is pressed on this control
        /// </summary>
        /// <param name="e">The mouse event arguments</param>
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (_panLeft.Contains(e.Location))
                _minTime -= 0.1;
            if (_panRight.Contains(e.Location))
                _minTime += 0.1;

            if (_zoomIn.Contains(e.Location))
                _shownTime -= 0.1;
            if (_zoomOut.Contains(e.Location))
                _shownTime += 0.1;

            if (_currentAnim != null)
            {
                if (_playButton.Contains(e.Location))
                    _currentAnim.Play();
                if (_stopButton.Contains(e.Location))
                    _currentAnim.Reset();
                if (_pauseButton.Contains(e.Location))
                    _currentAnim.Pause();
                if (_reverseButton.Contains(e.Location))
                {
                    _currentAnim.IsReversed = !_currentAnim.IsReversed;
                    _currentAnim.Play();
                }
            }

            _shownTime = _shownTime < 0.1 ? 0.1 : _shownTime;

            _minTime = _minTime < 0 ? 0 : _minTime;
            _maxTime = _minTime + _shownTime;
            Invalidate();

            Capture = false;
        }
    }
}
