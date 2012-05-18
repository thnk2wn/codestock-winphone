using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace Phone.Common.Navigation
{
    public class NavigationService : INavigationService
    {
        private PhoneApplicationFrame _mainFrame;

        public event NavigatingCancelEventHandler Navigating;

        public void NavigateTo(Uri pageUri)
        {
            if (EnsureMainFrame())
            {
                _mainFrame.Navigate(pageUri);
            }
        }

        public void NavigateTo(Uri uri, object navData)
        {
            _navData = navData;

            if (EnsureMainFrame())
            {
                _mainFrame.Navigate(uri);
            }
        }

        private object _navData;

        public void GoBack()
        {
            if (EnsureMainFrame()
                && _mainFrame.CanGoBack)
            {
                _mainFrame.GoBack();
            }
        }

        public void ExitBack()
        {
            if (_mainFrame.CanGoBack)
            {
                while (_mainFrame.RemoveBackEntry() != null)
                {
                    _mainFrame.RemoveBackEntry();
                }
            }
        }

        private bool EnsureMainFrame()
        {
            if (_mainFrame != null)
            {
                return true;
            }

            _mainFrame = Application.Current.RootVisual as PhoneApplicationFrame;

            if (_mainFrame != null)
            {
                // Could be null if the app runs inside a design tool
                _mainFrame.Navigating += (s, e) =>
                {
                    if (Navigating != null)
                    {
                        Navigating(s, e);
                    }
                };

                _mainFrame.Navigated += (s, e) =>
                {
                    if (_navData != null)
                    {
                        var page = _mainFrame.Content as PhoneApplicationPage;

                        if (null != page)
                        {
                            var vm = page.DataContext as INavigableTarget;

                            if (null == vm)
                                throw new NullReferenceException(string.Format("Page {0} DataContext must be INavigableTarget", page.GetType().Name));

                            vm.NavigatedTo(_navData);
                            _navData = null;
                        }
                    }
                };

                return true;
            }

            return false;
        }
    }
}
