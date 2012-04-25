using System;

namespace Phone.Common.Net
{
    public class RequestFailure
    {
        public Exception Error { get; internal set; }
        public string Reason { get; internal set; }
        public string Url { get; internal set; }
        public TimeSpan Timeout { get; internal set; }
        public bool NotFound { get; internal set; }
        public string StatusText { get; internal set; }
        public string StatusCodeText { get; internal set; }
        public bool IsCommon { get; internal set; }
        public string FriendlyMessage { get; internal set; }
    }
}
