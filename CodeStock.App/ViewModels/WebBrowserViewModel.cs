using System;
using System.Windows.Input;
using CodeStock.App.ViewModels.Support;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Controls;
using Phone.Common.IO;
using Phone.Common.Navigation;

namespace CodeStock.App.ViewModels
{
    public class WebBrowserViewModel : AppViewModelBase, INavigableTarget
    {
        public WebBrowserViewModel()
        {
            return;
        }

        public string twitterSuccessfulShareUrl = "http://twitter.com/share/complete";
        public string twitterLoginFailUri = "res://ieframe.dll/http_default.htm#http://twitter.com/share";


        private Uri _currentSource;
        public Uri CurrentSource
        {
            get { return _currentSource; }
            set
            {
                _currentSource = value;
                RaisePropertyChanged(() => CurrentSource);
                OnCurrentSourceChanged();
            }
        }

        private Uri _initialSource;
        public Uri InitialSource
        {
            get { return _initialSource; }
            set
            {
                _initialSource = value;
                RaisePropertyChanged(() => InitialSource);
            }
        }


        private void OnCurrentSourceChanged()
        {
            if (CurrentSource == null) return;

            if (CurrentSource.OriginalString.Contains(twitterSuccessfulShareUrl))
            {
                MessageBox("Posted your tweet is.", "Done");
                //MessengerInstance.Send(new NavigateBackMessage());
            }
            else if (CurrentSource.OriginalString == twitterLoginFailUri)
            {
                MessageBox("Wrong twitter user name or password.", "Login Failed");
                //MessengerInstance.Send(new NavigateBackMessage());
            }
        }

        private bool _isLoadingPage;
        public bool IsLoadingPage
        {
            get { return _isLoadingPage; }
            set
            {
                _isLoadingPage = value;
                RaisePropertyChangedUpdateState(() => IsLoadingPage, value);
            }
        }

        public ICommand LoadingPage
        {
            get { return new RelayCommand(() => IsLoadingPage = true); }
        }


        public ICommand LoadedPage
        {
            get { return new RelayCommand(() => IsLoadingPage = false); }
        }

        private SupportedPageOrientation _supportedOrientation = SupportedPageOrientation.PortraitOrLandscape;

        public SupportedPageOrientation SupportedOrientation
        {
            get { return _supportedOrientation; }

            set
            {
                if (_supportedOrientation != value)
                {
                    _supportedOrientation = value;
                    RaisePropertyChanged(() => SupportedOrientation);
                }
            }
        }


        #region INavigableTarget Members

        public void NavigatedTo(object data)
        {
            var url = string.Empty;

            if (data is string)
            {
                url = data.ToString();
            }
            else if (data is WebBrowserArgs)
            {
                var args = (WebBrowserArgs)data;
                url = args.WebBrowserUrl;

                if (null != args.SupportedOrientation)
                    this.SupportedOrientation = args.SupportedOrientation.Value;
            }

            var safeUrl = Uri.EscapeUriString(url);

            // clean up this twitter workaround hack later. without #hashtag gets removed
            if (safeUrl.StartsWith("http://twitter.com/share"))
                safeUrl = safeUrl.Replace("#", "%23");

            this.InitialSource = new Uri(safeUrl, UriKind.Absolute);
        }

        #endregion

        public void Backup(IDataStorage store)
        {
            store.Backup(() => this.InitialSource, this.InitialSource);
        }

        public void Restore(IDataStorage store)
        {
            var src = store.Restore(() => this.InitialSource, null);

            if (null != src)
                this.InitialSource = src;
        }
    }
}