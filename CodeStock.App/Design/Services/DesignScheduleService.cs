using System;
using System.Collections.Generic;
using CodeStock.Data;
using CodeStock.Data.ServiceAccess;

namespace CodeStock.App.Design.Services
{
    public class DesignScheduleService : IScheduleService
    {

        #region IScheduleService Members

        public void Load(int scheduleId)
        {
            this.SessionIds = new List<int> { 253, 35, 206, 60, 85, 237, 235 };
        }

        public IEnumerable<int> SessionIds {get; private set;}
        

        public Action<CompletedEventArgs> AfterCompleted {get; set;}

        public TimeSpan CacheDuration { get; set; }
        
        public void ClearCache()
        {
            return;
        }

        #endregion
    }
}
