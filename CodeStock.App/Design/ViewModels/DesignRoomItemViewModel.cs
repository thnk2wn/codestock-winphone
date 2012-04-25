using System.Collections.ObjectModel;
using System.Linq;
using CodeStock.App.Design.Services;
using CodeStock.App.ViewModels.ItemViewModels;
using Phone.Common.Extensions.System.Collections.Generic_;

namespace CodeStock.App.Design.ViewModels
{
    public class DesignRoomItemViewModel : IRoomItemViewModel
    {
        public DesignRoomItemViewModel()
        {
            this.Name = "406";
            this.Title = "Room " + this.Name;
            var svc = new DesignSessionService();
            svc.Load();
            var vms = new ObservableCollection<SessionItemViewModel>();
            svc.Data.Where(s=> s.Room == this.Name).ForEach(s => vms.Add(new SessionItemViewModel(s)));
            this.Sessions = vms;
        }
        #region IRoomItemViewModel Members

        public string Name {get; set;}
        public string Title { get; set; }
        

        public ObservableCollection<SessionItemViewModel> Sessions {get; set;}
        

        #endregion
    }
}
