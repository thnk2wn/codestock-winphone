using CodeStock.App.ViewModels.ItemViewModels;
using CodeStock.Data.Model;

namespace CodeStock.App.ViewModels.Support
{
    public class SpeakerItemMapper
    {
        public static void FillViewModel(ISpeakerItemViewModel vm, Speaker model)
        {
            if (null == model) return;

            // consider using AutoMapper
            vm.Bio = (!string.IsNullOrEmpty(model.Bio)) ? model.Bio.TrimEnd() : model.Bio;
            vm.Company = model.Company;
            vm.Name = model.Name;
            vm.PhotoUrl = !string.IsNullOrEmpty(model.PhotoUrl) ? model.PhotoUrl : SpeakerItemViewModel.DefaultImage;
            vm.SpeakerId = model.SpeakerId;
            vm.TwitterId = !string.IsNullOrEmpty(model.TwitterId) ? !model.TwitterId.StartsWith("@") ? "@" + model.TwitterId : model.TwitterId : string.Empty;
            vm.Website = ("http://" == model.Website) ? null : model.Website;

            vm.TwitterUrl = (!string.IsNullOrEmpty(vm.TwitterId)) ? string.Format("http://mobile.twitter.com/{0}", vm.TwitterId) : null;

            vm.Url = model.Url;

            // this block causing runtime crashes, poss stack overflow / circular ref...
            //var sessions = new ObservableCollection<SessionItemViewModel>();
            //if (null != model.Sessions)
            //{
            //    model.Sessions.ForEach(s => sessions.Add(new SessionItemViewModel(s)));
            //}
            //vm.Sessions = sessions;
        }
    }
}
