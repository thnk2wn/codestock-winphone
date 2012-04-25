using System.Net;

namespace Phone.Common.Net
{
    public class HtmlDecoder : IHtmlDecoder
    {
        public string Decode(string htmlString)
        {
            return HttpUtility.HtmlDecode(htmlString);
        }

        //public bool TrimEnd { get; set; }
    }
}
