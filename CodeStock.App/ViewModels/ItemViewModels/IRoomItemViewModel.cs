using System.Collections.ObjectModel;

namespace CodeStock.App.ViewModels.ItemViewModels
{
    public interface IRoomItemViewModel
    {
        string Name { get; set; }
        string Title { get; set; }
        ObservableCollection<SessionItemViewModel> Sessions { get; set; }
    }
}
