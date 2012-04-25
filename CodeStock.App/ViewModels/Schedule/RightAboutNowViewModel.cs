using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeStock.App.ViewModels.Schedule
{
    public class RightAboutNowViewModel : ScheduleChildViewModel
    {
        public override void Load()
        {
            // don't set busy; parent will handle
            this.NotFoundText = "No sessions found in progress or starting shortly.";
            if (null == this.AllSessions) return;

            var aboutNow = this.AllSessions.Where(s => (s.HasStarted && !s.HasEnded) ||
                (!s.HasStarted && s.StartTime.AddMinutes(-15) <= Now()));

            var ids = new List<int>();
            ids.AddRange(aboutNow.Select(s => s.SessionId));
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
