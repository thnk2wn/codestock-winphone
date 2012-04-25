using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CodeStock.App.Design.Services;
using CodeStock.App.ViewModels.ItemViewModels;
using CodeStock.App.ViewModels.Support;
using CodeStock.Data;
using Phone.Common.Extensions.System.Collections.Generic_;

namespace CodeStock.App.Design.ViewModels
{
    public class DesignCoreData : ICoreData
    {
        public DesignCoreData()
        {
            EnsureLoaded();
        }

        #region ICoreData Members

        public List<SessionItemViewModel> Sessions {get; set;}
        

        public List<SpeakerItemViewModel> Speakers {get; set;}
       

        public void LoadData()
        {
            var sessionSvc = new DesignSessionService();
            sessionSvc.AfterCompleted = (e) =>
            {
                this.Sessions = new List<SessionItemViewModel>();
                //throw new Exception("count is : " + sessionSvc.Data.Count());
                sessionSvc.Data.ForEach(s => this.Sessions.Add(new SessionItemViewModel(s)));
            };
            sessionSvc.Load();

            var speakerSvc = new DesignSpeakersService();
            speakerSvc.AfterCompleted = (e) =>
            {
                this.Speakers = new List<SpeakerItemViewModel>();
                speakerSvc.Data.ForEach(sp => this.Speakers.Add(new SpeakerItemViewModel(sp)));
            };
            speakerSvc.Load();
        }

        public void EnsureLoaded()
        {
            if (!this.IsLoaded)
                LoadData();
        }

        public bool IsLoaded {get; set;}

        public bool SessionsBound { get; set; }

        public bool SpeakersBound { get; set; }

        public ObservableCollection<SessionItemViewModel> SessionsForSpeaker(int speakerId)
        {
            return this.Sessions.Take(2).ToObservableCollection();
        }

        public CompletedEventArgs SessionCompletedInfo { get {return null;}  }
        public CompletedEventArgs SpeakerCompletedInfo { get { return null; } }

        public bool IsUnavailableFromError { get { return false; } }

        #endregion
    }
}
