using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using CodeStock.App.ViewModels.Support;
using GalaSoft.MvvmLight.Command;
using Phone.Common.IOC;
using Phone.Common.Navigation;

namespace CodeStock.App.ViewModels.ItemViewModels
{
    public class RoomItemViewModel : AppViewModelBase, IRoomItemViewModel
    {
        public RoomItemViewModel() 
        {
            return;
        }
        
        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged(() => Name);

                    this.Title = !string.IsNullOrEmpty(value) ? "Room " + value : value;
                }
            }
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    RaisePropertyChanged(() => Title);
                }
            }
        }


        private ObservableCollection<SessionItemViewModel> _sessions;

        public ObservableCollection<SessionItemViewModel> Sessions
        {
            get
            {
                // this lazy load style is needed to prevent circular reference stack overflow type errors loading data
                if (null == _sessions || !_sessions.Any())
                {
                    _sessions = new ObservableCollection<SessionItemViewModel>();

                    var sessions = IoC.Get<ICoreData>().Sessions.Where(
                        s => s.Room == this.Name).OrderBy(s=> s.StartTime).ToList();
                    sessions.ForEach(s => _sessions.Add(s));

                    RaisePropertyChanged(() => Sessions);
                }
                return _sessions;
            }
            set
            {
                if (_sessions != value)
                {
                    _sessions = value;
                    RaisePropertyChanged(() => Sessions);
                }
            }
        }

        public ICommand SessionSelectedCommand
        {
            get { return new RelayCommand<SelectionChangedEventArgs>(SessionSelected); }
        }

        private void SessionSelected(SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;

            var session = (SessionItemViewModel)e.AddedItems[0];
            var navigationService = IoC.Get<INavigationService>();
            navigationService.NavigateTo(Uris.Session(session));
            return;
        }

    }
}
