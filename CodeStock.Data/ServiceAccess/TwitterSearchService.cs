using CodeStock.Data.Model;
using Newtonsoft.Json;

namespace CodeStock.Data.ServiceAccess
{
    public class TwitterSearchService : WebServiceBase, ITwitterSearchService
    {
        private const string UrlFormat = "http://search.twitter.com/search.json?rpp=100&q={0}";

        public void Search(string query)
        {
            //TODO: consider seeing if query starts with @ and if so do a more detailed different call to user_timeline service such as:
            // http://api.twitter.com/1/statuses/user_timeline.json?screen_name=@thnk2wn
            var searchUrl = string.Format(UrlFormat, SafeUrlArg(query));
            MakeRequest(searchUrl);
        }

        public TwitterSearchResult Result { get; private set; }

        protected override void AfterRequestCompleted(string result)
        {
            RaiseCompleteAfter( ()=> this.Result = ParseJson(result));
        }

        private static TwitterSearchResult ParseJson(string json)
        {
            var result = JsonUtility.Deserialize<TwitterSearchResult>(json);
            return result;
        }

        /*
        private void LoadDataFromResource()
        {
            var res = new ResourceHelper();
            const string resourceFile = "TwitterCodeStockSearch.json";
            var info = res.GetResource("CodeStockData", resourceFile);

            if (null == info)
                throw new NullReferenceException(string.Format("Failed to find resource '{0}'", resourceFile));

            string json;

            using (var reader = new StreamReader(info.Stream))
            {
                json = reader.ReadToEnd();
            }

            this.Result = ParseJson(json);
            OnAfterCompleted(new CompletedEventArgs());
        }
        */
    }
}
