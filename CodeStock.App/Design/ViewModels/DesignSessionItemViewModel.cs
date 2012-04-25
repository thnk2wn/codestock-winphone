using System;
using System.Linq;
using CodeStock.App.Design.Services;
using CodeStock.App.ViewModels.ItemViewModels;
using CodeStock.App.ViewModels.Support;

namespace CodeStock.App.Design.ViewModels
{
    public class DesignSessionItemViewModel : ISessionItemViewModel
    {
        public DesignSessionItemViewModel()
        {
            //ObjectFactory.Get<ISessionsService>()
            var svc = new DesignSessionService();
            svc.Load();

            // consider automapper for wp7 (not out of beta?)
            var item = svc.Data.FirstOrDefault();
            SessionItemMapper.FillViewModel(this, item);

            var spSvc = new DesignSpeakersService();
            spSvc.Load();
            var speakerModel = spSvc.Data.FirstOrDefault();
            this.Speaker = new SpeakerItemViewModel();
            this.SpeakerName = speakerModel.Name;
            SpeakerItemMapper.FillViewModel(this.Speaker, speakerModel);
        }

        public string Title { get; set; }
        public string TrackArea { get; set; }
        public string Technology { get; set; }
        public string Level { get; set; }
        public string Abstract { get; set; }
        public int SessionId { get; set; }
        public string Room { get; set; }
        public string RoomText { get; set; }
        public string TimeText { get; set; }
        public int SpeakerId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string TimeAndRoom { get; set; }
        public string Url { get; set; }
        public string VoteRank { get; set; }

        public ISpeakerItemViewModel Speaker { get; set; }
        public string SpeakerName { get; set; }
    }
}
