using System;
using System.Windows;

namespace Phone.Common.Threading
{
    public static class DispatchUtil
    {
        public static void SafeDispatch(Action action)
        {
            if (Deployment.Current.Dispatcher.CheckAccess())
            {
                // do it now on this thread 
                action.Invoke();
            }
            else
            {
                // do it on the UI thread 
                Deployment.Current.Dispatcher.BeginInvoke(action);
            }
        }
    }
}
