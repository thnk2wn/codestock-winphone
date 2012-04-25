using System.Collections.ObjectModel;

namespace CodeStock.App.ViewModels.ItemViewModels
{
    public interface ISpeakerItemViewModel
    {
        string Bio { get; set; }
        string Company { get; set; }
        string Name { get; set; }
        string PhotoUrl { get; set; }
        string TwitterId { get; set; }
        string TwitterUrl { get; set; }
        string Website { get; set; }
        int SpeakerId { get; set; }
        string Url { get; set; }

        ObservableCollection<SessionItemViewModel> Sessions { get; set; } 
    }
}
