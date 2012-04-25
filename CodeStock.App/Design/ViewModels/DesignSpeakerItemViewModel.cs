using System.Collections.ObjectModel;
using System.Linq;
using CodeStock.App.Design.Services;
using CodeStock.App.ViewModels.ItemViewModels;
using CodeStock.App.ViewModels.Support;
using Phone.Common.IOC;

namespace CodeStock.App.Design.ViewModels
{
    public class DesignSpeakerItemViewModel : ISpeakerItemViewModel
    {
        public DesignSpeakerItemViewModel()
        {
            var svc = new DesignSpeakersService();
            svc.Load();

            var item = svc.Data.Skip(1).FirstOrDefault();

            SpeakerItemMapper.FillViewModel(this, item);
        }

        public string Bio { get; set; }
        public string Company { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public string TwitterId { get; set; }
        public string Website { get; set; }
        public int SpeakerId { get; set; }
        public string TwitterUrl { get; set; }
        public string Url { get; set; }

        private ObservableCollection<SessionItemViewModel> _sessions;

        public ObservableCollection<SessionItemViewModel> Sessions
        {
            get
            {
                if (null == _sessions)
                {
                    _sessions = IoC.Get<ICoreData>().SessionsForSpeaker(this.SpeakerId);
                }
                return _sessions;
            }

            set
            {
                _sessions = value;
            }
        }
    }
}
