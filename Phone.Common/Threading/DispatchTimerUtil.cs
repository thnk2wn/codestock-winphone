using System;
using System.Windows;
using System.Windows.Threading;

namespace Phone.Common.Threading
{
    public class DispatchTimerUtil
    {
        public static void On(TimeSpan ts, Action action)
        {
            var t = new DispatcherTimer() { Interval = ts };
            t.Tick += (s, args) =>
            {
                action();
                t.Stop();
            };
            t.Start();

        }


        public static void OnWithDispatcher(TimeSpan ts, Action action)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                var t = new DispatcherTimer() { Interval = ts };
                t.Tick += (s, args) =>
                {
                    action();
                    t.Stop();
                };
                t.Start();
            });
        }
    }
}
