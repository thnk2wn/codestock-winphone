using Newtonsoft.Json;

namespace CodeStock.Data.ServiceAccess
{
    public class UserIdLookupService : WebServiceBase, IUserIdLookupService
    {
        private const string URL = "http://codestock.org/api/v2.0.svc/GetUserIDJson?Email={0}";

        public void Lookup(string email)
        {
            this.IsBusy = true;
            var url = string.Format(URL, email);
            MakeRequest(url);
        }

        protected override void AfterRequestCompleted(string result)
        {
            var root = JsonUtility.Deserialize<UserIdLookupResult>(result);
            this.UserId = (0 == root.d) ? (int?)null : root.d;
            OnAfterCompleted(new CompletedEventArgs());
        }
        

        public int? UserId { get; private set; }

        public bool NotFound { get { return !this.UserId.HasValue; } }

        public bool IsBusy { get; private set; }

        protected override void OnAfterCompleted(CompletedEventArgs e)
        {
            IsBusy = false;
            base.OnAfterCompleted(e);
        }
    }

    public class UserIdLookupResult
    {
        public int d { get; set; }
        //public object meta { get; set; }
    }

    
}
