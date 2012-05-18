using System;
using System.Windows.Navigation;

namespace Phone.Common.Navigation
{
    public interface INavigationService
    {
        event NavigatingCancelEventHandler Navigating;
        void NavigateTo(Uri uri);
        void NavigateTo(Uri uri, object navData);
        void GoBack();
        void ExitBack();
    }
}
