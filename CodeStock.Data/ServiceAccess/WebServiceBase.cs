using System;
using System.IO;
using Phone.Common.Diagnostics;
using Phone.Common.Diagnostics.Logging;
using Phone.Common.Extensions.System.IO_;
using Phone.Common.Net;

namespace CodeStock.Data.ServiceAccess
{
    public abstract class WebServiceBase
    {
        protected void MakeRequest(string url)
        {
            MakeRequest(url, TimeSpan.FromSeconds(25));

            // temp timeout testing:
            //MakeRequest(url, TimeSpan.FromMilliseconds(100));
        }

        private CodeTimer CallTimer { get; set; }
        private string Url {get; set;}

        protected void MakeRequest(string url, TimeSpan timeout)
        {
            this.Url = url;
            var webRequester = new WebRequester();
            LogInstance.LogInfo("Calling {0} with timeout of {1} seconds", url, timeout.TotalSeconds);
            this.CallTimer = CodeTimer.StartNew();
            webRequester.Request(url, timeout, LocalAfterRequestCompleted, LocalAfterRequestFailure);
        }

        protected string SafeUrlArg(string value)
        {
            return Uri.EscapeDataString(value);
        }

        private void LocalAfterRequestCompleted(Stream stream)
        {
            LogInstance.LogInfo("Call complete to {0} in {1:#.000} seconds", this.Url, CallTimer.Stop());
            this.ResponseText = stream.ReadToEnd().TrimEnd();
            AfterRequestCompleted(this.ResponseText);
        }

        private void LocalAfterRequestFailure(RequestFailure failure)
        {
            LogInstance.LogInfo("Call complete with failure to {0} in {1}", this.Url, CallTimer.Stop());
            AfterRequestFailure(failure);
        }

        protected abstract void AfterRequestCompleted(string result);
        

        protected virtual void AfterRequestFailure(RequestFailure failure)
        {
            var ex = new Exception(string.Format("Request failed with reason '{0}'; see inner exception", failure.Reason), failure.Error);
            LogInstance.LogError("{0}: Service call failure. Reason: {1}, Error: {2}", this.GetType().Name, failure.Reason, failure.Error);
            OnAfterCompleted(new CompletedEventArgs(ex, failure));
        }

        protected virtual void OnAfterCompleted(CompletedEventArgs e)
        {
            if (null != AfterCompleted)
            {
                AfterCompleted(e);
            }
        }

        protected void RaiseCompleteAfter(Action action)
        {
            action();
            OnAfterCompleted(new CompletedEventArgs());
        }

        protected string ResponseText { get; private set; }

        public Action<CompletedEventArgs> AfterCompleted { get; set; }
    }
}
