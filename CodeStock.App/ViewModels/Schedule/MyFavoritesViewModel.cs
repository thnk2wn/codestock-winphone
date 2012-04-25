using CodeStock.Data.LocalAccess;
using Phone.Common.IOC;

namespace CodeStock.App.ViewModels.Schedule
{
    public class MyFavoritesViewModel : ScheduleChildViewModel
    {

        public override void Load()
        {
            this.NotFoundText = "No favorite sessions found. Navigate to a session to add it as a favorite.";
            // really this only gets constructed and loaded once then after that we are just refreshing the ids
            var favs = IoC.Get<FavoriteSessions>();
            SetResult(favs.SessionIds);
            SetBusy(false);
        }

        public override bool IsRefreshNeeded
        {
            get { return true; }
        }
    }
}
