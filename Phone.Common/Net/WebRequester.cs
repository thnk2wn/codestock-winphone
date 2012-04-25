using System;
using System.IO;
using System.Net;
using Phone.Common.Threading;

namespace Phone.Common.Net
{
    public class WebRequester : IWebRequester
    {
        //[Inject]
        //public INetworkStatusProvider NetworkStatusProvider { get; set; }

        private static void InvokeOnCompleted(Action<Stream> onCompleted, Stream stream)
        {
            DispatchUtil.SafeDispatch(() => onCompleted(stream));
        }

        private static void InvokeOnFailure(Action<RequestFailure> onFailure, RequestFailure failure)
        {
            DispatchUtil.SafeDispatch(() => onFailure(failure));
        }

        public string Url { get; private set; }
        public TimeSpan Timeout { get; private set; }

        private const string TimeoutReason = "Timeout";

        public void Request(string url, TimeSpan timeout, Action<Stream> onCompleted, Action<RequestFailure> onFailure)
        {
            this.Url = url;
            this.Timeout = timeout;

            //if (!NetworkStatusProvider.IsConnected)
            //{
            //    OnTimeoutOrTechFail();
            //    return;
            //}

            var requestState = new RequestState();


            var request = (HttpWebRequest)HttpWebRequest.Create(url);

            request.Method = "GET";

            DispatchTimerUtil.OnWithDispatcher(timeout, () =>
            {
                if (!request.HaveResponse && !requestState.WasFailureOrCompleteInvoked)
                {
                    request.Abort();
                    requestState.WasFailureInvoked = true;
                    InvokeOnFailure(onFailure, CreateFailure(TimeoutReason));
                }
            });

            var async = request.BeginGetResponse(result =>
            {
                if (!request.HaveResponse)
                {
                    if (!requestState.WasFailureOrCompleteInvoked)
                        InvokeOnFailure(onFailure, CreateFailure("No response"));
                    requestState.WasFailureInvoked = true;
                }
                else
                {
                    try
                    {
                        var response = request.EndGetResponse(result);

                        var responseStream = response.GetResponseStream();
                        if (responseStream.CanRead)
                        {
                            if (!requestState.WasFailureOrCompleteInvoked)
                                InvokeOnCompleted(onCompleted, responseStream);
                            requestState.WasCompletedInvoked = true;
                        }
                        else
                        {
                            if (!requestState.WasFailureOrCompleteInvoked)
                                InvokeOnFailure(onFailure, CreateFailure("Cannot read"));
                            requestState.WasFailureInvoked = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!requestState.WasFailureOrCompleteInvoked)
                            InvokeOnFailure(onFailure, CreateFailure("Error", ex));
                        requestState.WasFailureInvoked = true;
                    }
                }
            }, null);

            //DispatchTimerUtil.OnWithDispatcher(timeout, () =>
            //{
            //    if (!request.HaveResponse && !requestState.WasFailureOrCompleteInvoked)
            //    {
            //        request.Abort();
            //        requestState.WasFailureInvoked = true;
            //        InvokeOnFailure(onFailure, CreateFailure("Timeout"));
            //    }
            //});
        }

        private RequestFailure CreateFailure(string reason, Exception error = null)
        {
            var fail = new RequestFailure { Reason = reason, Error = error, Timeout = this.Timeout, Url = this.Url };

            var webEx = error as WebException;

            Uri uri;
            var validUri = Uri.TryCreate(fail.Url, UriKind.RelativeOrAbsolute, out uri);

            if (null != webEx)
            {
                fail.StatusText = webEx.Status.ToString();
                var resp = webEx.Response as HttpWebResponse;

                if (null != resp)
                {
                    fail.StatusCodeText = resp.StatusCode.ToString();

                    if (resp.StatusCode == HttpStatusCode.NotFound)
                    {
                        fail.NotFound = true;
                        fail.IsCommon = true;

                        if (validUri)
                        {
                            fail.FriendlyMessage = string.Format(
                                "Received a Not Found response from host {0}. Verify your connectivity and host availability and try again later.",
                                uri.Host);
                        }
                    }

                }
            }
            else
            {
                if (reason == TimeoutReason)
                {
                    fail.IsCommon = true;

                    if (validUri)
                    {
                        fail.FriendlyMessage = string.Format(
                                "Timeout waiting for host {0}. Verify your connectivity and host availability and try again later. Timeout is '{1}'",
                                uri.Host, this.Timeout);
                    }
                }
            }

            return fail;
        }
        
    }

    internal class RequestState
    {
        public bool WasFailureOrCompleteInvoked
        {
            get { return WasCompletedInvoked || WasFailureInvoked; }
        }

        public bool WasFailureInvoked { get; set; }
        
        public bool WasCompletedInvoked { get; set; }
    }

    
}
