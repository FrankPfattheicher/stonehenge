namespace IctBaden.Stonehenge.Services
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    public class PassiveTimer : IComparable
    {
        private bool locked;
        private long lockedTimer;
        private long timer;

        private static long Now
        {
            get
            {
                return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            }
        }

        public PassiveTimer()
        {
            locked = false;
            Stop();
        }
        public PassiveTimer(long timeMilliseconds)
        {
            locked = false;
            Start(timeMilliseconds);
        }
        public PassiveTimer(TimeSpan period)
            : this((long)period.TotalMilliseconds)
        {
        }

        public void Lock()
        {
            locked = true;
            lockedTimer = DateTime.Now.Ticks;
        }
        public void Unlock()
        {
            locked = false;
        }

        public static implicit operator Int64(PassiveTimer me)
        {
            return me.timer;
        }

        public void Start(TimeSpan time)
        {
            Start((long)time.TotalMilliseconds);
        }
        public void Start(long timeMilliseconds)
        {
            timer = Now + timeMilliseconds;
        }
        public void StartInterval(long timeMilliseconds)
        {
            timer += timeMilliseconds;
        }

        public void Stop()
        {
            timer = 0;
        }

        public bool Running
        {
            get
            {
                return timer != 0;
            }
        }
        public bool Timeout
        {
            get
            {
                if (!Running)
                    return false;
                var t = locked ? lockedTimer : Now;
                return t >= timer;
            }
        }
        public TimeSpan Remaining
        {
            get
            {
                var t = locked ? lockedTimer : Now;
                var remaining = timer - t;
                return TimeSpan.FromMilliseconds(remaining);
            }
        }
        public Int64 RemainingMilliseconds
        {
            get
            {
                var t = locked ? lockedTimer : Now;
                var remaining = timer - t;
                return remaining;
            }
        }

        public DateTime TimeoutTimeStamp
        {
            get
            {
                return DateTime.Now + Remaining;
            }
        }

        public Timer SetCallback(TimerCallback callback, object state)
        {
            var T = Math.Max(0, RemainingMilliseconds);
            Debug.Print("Timer SetCallback {0}", T);
            return new Timer(callback, state, (uint)T, System.Threading.Timeout.Infinite);
        }


        #region IComparable Members

        public int CompareTo(object obj)
        {
            var otherTimer = (PassiveTimer)obj;
            var t1 = locked ? lockedTimer : timer;
            var t2 = otherTimer.locked ? otherTimer.lockedTimer : otherTimer.timer;
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
