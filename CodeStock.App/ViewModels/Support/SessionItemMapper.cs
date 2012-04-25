using System;
using CodeStock.App.ViewModels.ItemViewModels;
using CodeStock.Data.Model;

namespace CodeStock.App.ViewModels.Support
{
    public class SessionItemMapper
    {
        public static void FillViewModel(ISessionItemViewModel vm, Session model)
        {
            vm.Title = model.Title;
            vm.TrackArea = string.Format("{0}: {1}", model.Track, model.Area);
            vm.Level = (model.LevelGeneral == model.LevelSpecific)
                             ? model.LevelSpecific
                             : string.Format("{0} / {1}", model.LevelGeneral, model.LevelSpecific);
            vm.Abstract = model.Abstract;
            vm.SessionId = model.SessionId;
            vm.Technology = model.Technology;

            // time zone adjustments are handled at the model level:
            vm.StartTime = model.StartTime;
            vm.EndTime = model.EndTime;
            
            if (vm.StartTime != DateTime.MinValue && vm.EndTime != DateTime.MinValue)
            {
                // i.e. Fri 06/03 11:30 AM - 12:30 PM
                vm.TimeText = string.Format("{0:ddd MM/dd h:mm tt} - {1:h:mm tt}", vm.StartTime, vm.EndTime);
            }
            vm.SpeakerId = model.SpeakerId;
            vm.Room = model.Room;
            vm.RoomText = !string.IsNullOrEmpty(vm.Room) ? string.Format("Room {0}", vm.Room) : vm.Room;

            if (!string.IsNullOrEmpty(vm.TimeText) && !string.IsNullOrEmpty(vm.Room))
                vm.TimeAndRoom = string.Format("{0}  Room {1}", vm.TimeText, vm.Room);

            vm.Url = model.Url;

            if (model.VoteRank != null)
            {
                vm.VoteRank = ("NONE" == model.VoteRank) ? "None" 
                    : model.VoteRank.StartsWith("TOP") ? "Top " + model.VoteRank.Substring(3) : model.VoteRank;
            }

            //vm.Speaker = new SpeakerItemViewModel(model.Speaker);
        }
    }
}
