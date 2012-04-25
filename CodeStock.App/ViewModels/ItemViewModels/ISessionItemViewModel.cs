using System;

namespace CodeStock.App.ViewModels.ItemViewModels
{
    public interface ISessionItemViewModel
    {
        string Title { get; set; }
        string TrackArea { get; set; }
        string Technology { get; set; }
        string Level { get; set; }
        string Abstract { get; set; }
        int SessionId { get; set; }
        string Room { get; set; }
        string RoomText { get; set; }
        string TimeText { get; set; }
        int SpeakerId { get; set; }
        DateTime StartTime { get; set; }
        DateTime EndTime { get; set; }
        string TimeAndRoom { get; set; }
        string Url { get; set; }
        string VoteRank { get; set; }

        ISpeakerItemViewModel Speaker { get; }
        string SpeakerName { get; }
    }
}
