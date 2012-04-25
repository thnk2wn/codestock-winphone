using System;

namespace Phone.Common.Diagnostics
{
    /// <summary>
    /// Alternative to System.Diagnostics.Stopwatch - the reason is that in WP7 that class is in Microsoft.Phone.dll which
    /// is usually not referenced in non-UI projects. Rather than having to add in that reference this is a less accurate
    /// but still fine simple timer. We can't copy out Stopwatch source b/c it relies on SafeNativePerformanceMethods and other
    /// </summary>
    public class CodeTimer
    {
        private DateTime _startTime;
        private DateTime _stopTime;
        private bool _running;

        public static CodeTimer StartNew()
        {
            var timer = new CodeTimer();
            timer.Start();
            return timer;
        }

        public void Start()
        {
            this._startTime = DateTime.Now;
            this._running = true;
        }

        public double Stop()
        {
            this._stopTime = DateTime.Now;
            this._running = false;
            return this.TotalSeconds;
        }

        public TimeSpan Elapsed
        {
            get
            {
                var interval = (_running) ? (DateTime.Now - _startTime) : _stopTime - _startTime;
                return interval;   
            }
        }

        public double TotalSeconds
        {
            get
            {
                return Elapsed.TotalSeconds;
            }
        }
    }
}
