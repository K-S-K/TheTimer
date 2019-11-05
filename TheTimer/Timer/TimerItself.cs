using System;
using System.Threading;
using System.Diagnostics;

namespace TheTimer.Timer
{
    public delegate void TimerStateChangedDelegate(TimerState stOld, TimerState stNew);
    public class TimerItself
    {
        #region -> Events
        public event Action<TimeSpan> TimeChanged;
        public event TimerStateChangedDelegate StateChanged;
        #endregion


        #region -> Data
        private Stopwatch _swOuter;
        private Stopwatch _swInner;
        private Stopwatch _swSteps;

        private TimeSpan _inervalMeasure;
        private TimeSpan _inervalNotify;
        private TimeSpan _timeToWait;

        private TimerState _state;
        #endregion


        #region -> Properties
        public TimerState State
        {
            get { return _state; }
            private set
            {
                TimerState stOld = _state;
                TimerState stNew = value;

                if (stNew == stOld)
                {
                    return;
                }

                _state = value;

                StateChanged?.Invoke(stOld, stNew);
            }
        }

        public TimeSpan TimeToWait
        {
            get { return _timeToWait; }
            set
            {
                if (_timeToWait == value) { return; }

                if (State == TimerState.Complete)
                {
                    _swOuter.Reset();
                }

                _timeToWait = value;

                try { TimeChanged?.Invoke(TimeElapsed); } catch {; }
            }
        }

        public TimeSpan TimeElapsed
        {
            get
            {
                TimeSpan t = CalcTimeToWait();

                return t;
            }
        }

        private TimeSpan CalcTimeToWait()
        {
            TimeSpan t = TimeToWait - _swOuter.Elapsed;
            if (t.TotalMilliseconds < 0)
            {
                t = TimeSpan.Zero;
            }

            return t;
        }
        #endregion


        #region -> Methods
        public void Start()
        {
            if (_timeToWait == TimeSpan.Zero)
            {
                return;
            }

            if (State == TimerState.Stopped)
            {
                _swOuter.Restart();
            }
            else
            {
                _swOuter.Start();
            }

            State = TimerState.Running;
        }

        public void Restart()
        {
            _swOuter.Restart();

            State = TimerState.Running;
        }

        public void Pause()
        {
            _swOuter.Stop();
            State = TimerState.Paused;
        }

        public void Stop()
        {
            _swOuter.Reset();
            State = TimerState.Stopped;
            try { TimeChanged?.Invoke(TimeElapsed); } catch {; }
        }

        private void Complete()
        {
            _swOuter.Stop();
            State = TimerState.Complete;
        }
        #endregion


        #region -> Implementation
        private void ThrCount()
        {
            _swInner.Restart();
            _swSteps.Restart();

            while (true)
            {
                Thread.Sleep(_inervalMeasure);

                if (_swInner.Elapsed < _inervalMeasure) { continue; }

                _swInner.Restart();

                if (_swSteps.Elapsed >= _inervalNotify)
                {
                    if (_swOuter.IsRunning)
                    {
                        try { TimeChanged?.Invoke(TimeElapsed); } catch {; }

                        TimeSpan t = CalcTimeToWait();
                        if (t == TimeSpan.Zero)
                        {
                            Complete();
                        }
                    }

                    _swSteps.Restart();
                }
            }
        }
        #endregion


        #region -> Ctor
        public TimerItself()
        {
            _swOuter = new Stopwatch();
            _swInner = new Stopwatch();
            _swSteps = new Stopwatch();
            _inervalNotify = TimeSpan.FromSeconds(1);
            _inervalMeasure = TimeSpan.FromMilliseconds(100);

            new Thread(delegate () { ThrCount(); })
            { IsBackground = true, Name = "TimerItself" }.Start();
        }
        #endregion
    }
}
