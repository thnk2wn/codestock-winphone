namespace CodeStock.App.ViewModels.ItemViewModels
{
    public class SearchItemViewModel : AppViewModelBase
    {
        private string _line1;

        public string Line1
        {
            get { return _line1; }

            set
            {
                if (_line1 != value)
                {
                    _line1 = value;
                    RaisePropertyChanged(() => Line1);
                }
            }
        }

        private string _line2;

        public string Line2
        {
            get { return _line2; }

            set
            {
                if (_line2 != value)
                {
                    _line2 = value;
                    RaisePropertyChanged(() => Line2);
                }
            }
        }

        private string _line3;

        public string Line3
        {
            get { return _line3; }

            set
            {
                if (_line3 != value)
                {
                    _line3 = value;
                    RaisePropertyChanged(() => Line3);
                }
            }
        }

        private object _viewModel;

        public object ViewModel
        {
            get { return _viewModel; }

            set
            {
                if (_viewModel != value)
                {
                    _viewModel = value;
                    RaisePropertyChanged(() => ViewModel);
                }
            }
        }

        private string _photoUrl;

        public string PhotoUrl
        {
            get { return _photoUrl; }

            set
            {
                if (_photoUrl != value)
                {
                    _photoUrl = value;
                    RaisePropertyChanged(() => PhotoUrl);
                }
            }
        }

        public static SearchItemViewModel Create(ISessionItemViewModel vm)
        {
            var i = new SearchItemViewModel
            {
                Line1 = vm.Title,
                Line2 = vm.TimeAndRoom,
                Line3 = string.Empty,
                PhotoUrl = vm.Speaker.PhotoUrl,
                ViewModel = vm
            };
            return i;
        }

        public static SearchItemViewModel Create(ISpeakerItemViewModel vm)
        {
            var i = new SearchItemViewModel
            {
                Line1 = vm.Name,
                Line2 = vm.TwitterId,
                Line3 = vm.Website,
                PhotoUrl = vm.PhotoUrl,
                ViewModel = vm
            };
            return i;
        }
    }
}
