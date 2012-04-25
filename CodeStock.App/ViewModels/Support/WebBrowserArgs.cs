using Microsoft.Phone.Controls;

namespace CodeStock.App.ViewModels.Support
{
    public class WebBrowserArgs
    {
        public WebBrowserArgs(string webBrowserUrl, SupportedPageOrientation? supportedOrientation = null)
        {
            this.WebBrowserUrl = webBrowserUrl;
            this.SupportedOrientation = supportedOrientation;
        }

        public SupportedPageOrientation? SupportedOrientation { get; set; }

        public string WebBrowserUrl { get; set; }
    }
}
