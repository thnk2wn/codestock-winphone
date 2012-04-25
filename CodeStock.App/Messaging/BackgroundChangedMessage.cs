using GalaSoft.MvvmLight.Messaging;

namespace CodeStock.App.Messaging
{
    public class BackgroundChangedMessage : NotificationMessage
    {
        public BackgroundChangedMessage() : base("Background imange changed")
        {
            return;
        }
    }
}
