using System;
using CodeStock.Data;
using CodeStock.Data.ServiceAccess;

namespace CodeStock.App.Design.Services
{
    public class DesignUserIdLookupService : IUserIdLookupService
    {

        #region IUserIdLookupService Members

        public int? UserId {get; private set;}
        

        public bool NotFound
        {
            get { return false; }
        }

        public void Lookup(string email)
        {
            this.UserId = 500;
        }

        public Action<CompletedEventArgs> AfterCompleted {get; set;}
        

        public bool IsBusy
        {
            get { return false; }
        }

        #endregion
    }
}
