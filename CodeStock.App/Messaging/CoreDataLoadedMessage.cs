using CodeStock.Data;
using GalaSoft.MvvmLight.Messaging;

namespace CodeStock.App.Messaging
{
    public class CoreDataLoadedMessage : NotificationMessage 
    {
        public CoreDataLoadedMessage(object sender, CompletedEventArgs sessionArg, CompletedEventArgs speakerArgs) : base(sender, "CoreDataLoaded")
        {
            this.SessionCompletedInfo = sessionArg;
            this.SpeakerCompletedInfo = speakerArgs;
        }

        public CoreDataLoadedMessage() : this(null, null, null)
        {
            return;
        }

        public CompletedEventArgs SessionCompletedInfo { get; private set; }
        public CompletedEventArgs SpeakerCompletedInfo { get; private set; }
    }
}
