using System;
using System.Collections.Generic;
using CodeStock.Data;
using CodeStock.Data.ServiceAccess;

namespace CodeStock.App.Design.Services
{
    public abstract class DesignServiceBase<T> : IService<T>
    {
        #region IClient<T> Members

        public virtual void Load()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Data {get; protected set;}
        
        public Action<CompletedEventArgs> AfterCompleted {get; set;}

        public TimeSpan CacheDuration {get; set;}

        public bool IsDataExpired()
        {
            return false;
        }

        #endregion IClient<T> Members

        protected virtual void OnAfterCompleted()
        {
            if (null != AfterCompleted)
                AfterCompleted(new CompletedEventArgs());
        }

        public CompletedEventArgs CompletedInfo { get; set; }
    }
}
