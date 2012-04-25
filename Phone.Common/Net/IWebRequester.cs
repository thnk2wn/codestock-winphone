using System;
using System.IO;

namespace Phone.Common.Net
{
    public interface IWebRequester
    {
        void Request(string url, TimeSpan timeout, Action<Stream> onCompleted, Action<RequestFailure> onFailure);
    }
}
