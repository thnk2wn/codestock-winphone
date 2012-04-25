using System;
using Phone.Common.Net;

namespace CodeStock.Data
{
    public class CompletedEventArgs : EventArgs
    {
        public CompletedEventArgs(Exception error, RequestFailure requestFailure = null) : this()
        {
            this.Error = error;
            this.RequestFailure = requestFailure;
        }

        public CompletedEventArgs()
        {
            return;
        }

        public Exception Error { get; private set; }

        public RequestFailure RequestFailure { get; private set; }
    }
}
