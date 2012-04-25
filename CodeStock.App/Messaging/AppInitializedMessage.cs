using GalaSoft.MvvmLight.Messaging;

namespace CodeStock.App.Messaging
{
    public class AppInitializedMessage : NotificationMessage
    {
        public AppInitializedMessage() : base("Application initialized")
        {
            return;
        }
    }
}
