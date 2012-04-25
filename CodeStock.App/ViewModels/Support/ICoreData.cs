using System.Collections.Generic;
using System.Collections.ObjectModel;
using CodeStock.App.ViewModels.ItemViewModels;
using CodeStock.Data;

namespace CodeStock.App.ViewModels.Support
{
    public interface ICoreData
    {
        List<SessionItemViewModel> Sessions { get; set; }

        List<SpeakerItemViewModel> Speakers { get; set; }

        void LoadData();

        void EnsureLoaded();

        bool IsLoaded {get;}

        bool SessionsBound { get; set; }

        bool SpeakersBound { get; set; }

        ObservableCollection<SessionItemViewModel> SessionsForSpeaker(int speakerId);

        CompletedEventArgs SessionCompletedInfo { get; }
        CompletedEventArgs SpeakerCompletedInfo { get; }

        bool IsUnavailableFromError { get; }
    }
}
