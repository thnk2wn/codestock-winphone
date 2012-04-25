using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeStock.App.ViewModels.Schedule
{
    public class UpcomingViewModel : ScheduleChildViewModel
    {
        public override void Load()
        {
            // don't set busy; parent will suffice
            this.NotFoundText = "Looks like it is time for the after party.";
            if (null == this.AllSessions) return;

            var soon = this.AllSessions.Where(s => !s.HasStarted);

            var ids = new List<int>();
            ids.AddRange(soon.Select(s => s.SessionId));
            SetResult(ids);
        }

        public override bool IsRefreshNeeded
        {
            get
            {
                var needed = (null == this.LastLoadTime || this.LastLoadTime.Value.AddMinutes(2) < DateTime.Now);
                return needed;
            }
        }
    }
}
