namespace IctBaden.Stonehenge2.Core
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    public class PassiveTimer : IComparable
    {
        private bool _locked;
        private long _lockedTimer;
        private long _timer;

        private static long Now => DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

      public PassiveTimer()
        {
            _locked = false;
            Stop();
        }
        public PassiveTimer(long timeMilliseconds)
        {
            _locked = false;
            Start(timeMilliseconds);
        }
        public PassiveTimer(TimeSpan period)
            : this((long)period.TotalMilliseconds)
        {
        }

        public void Lock()
        {
            _locked = true;
            _lockedTimer = DateTime.Now.Ticks;
        }
        public void Unlock()
        {
            _locked = false;
        }

        public static implicit operator long(PassiveTimer me)
        {
            return me._timer;
        }

        public void Start(TimeSpan time)
        {
            Start((long)time.TotalMilliseconds);
        }
        public void Start(long timeMilliseconds)
        {
            _timer = Now + timeMilliseconds;
        }
        public void StartInterval(long timeMilliseconds)
        {
            _timer += timeMilliseconds;
        }

        public void Stop()
        {
            _timer = 0;
        }

        public bool Running => _timer != 0;

      public bool Timeout
        {
            get
            {
                if (!Running)
                    return false;
                var t = _locked ? _lockedTimer : Now;
                return t >= _timer;
            }
        }
        public TimeSpan Remaining
        {
            get
            {
                var t = _locked ? _lockedTimer : Now;
                var remaining = _timer - t;
                return TimeSpan.FromMilliseconds(remaining);
            }
        }
        public long RemainingMilliseconds
        {
            get
            {
                var t = _locked ? _lockedTimer : Now;
                var remaining = _timer - t;
                return remaining;
            }
        }

        public DateTime TimeoutTimeStamp => DateTime.Now + Remaining;

      public Timer SetCallback(TimerCallback callback, object state)
        {
            var T = Math.Max(0, RemainingMilliseconds);
          Debug.Print($"Timer SetCallback {T}");
            return new Timer(callback, state, (uint)T, System.Threading.Timeout.Infinite);
        }


        #region IComparable Members

        public int CompareTo(object obj)
        {
            var otherTimer = (PassiveTimer)obj;
            var t1 = _locked ? _lockedTimer : _timer;
            var t2 = otherTimer._locked ? otherTimer._lockedTimer : otherTimer._timer;
            return ((t1 - t2) < 0) ? -1 : 1;
        }

        #endregion

        public static bool operator <(PassiveTimer t1, PassiveTimer t2)
        {
            return t1.CompareTo(t2) < 0;
        }

        public static bool operator >(PassiveTimer t1, PassiveTimer t2)
        {
            return t1.CompareTo(t2) > 0;
        }
    }
}
