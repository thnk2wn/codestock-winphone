using System;
using CodeStock.App.ViewModels.ItemViewModels;

namespace CodeStock.App.ViewModels.Support
{
    public class Uris
    {
        public static Uri Session(SessionItemViewModel session)
        {
            var uri = new Uri(string.Format("/Pages/SessionDetailsPage.xaml?SessionId={0}", session.SessionId), UriKind.Relative);
            return uri;
        }

        public static Uri Speaker(ISpeakerItemViewModel speaker)
        {
            var uri = new Uri(string.Format("/Pages/SpeakerDetailsPage.xaml?SpeakerId={0}", speaker.SpeakerId), UriKind.Relative);
            return uri;
        }

        public static Uri Room(RoomItemViewModel room)
        {
            var uri = new Uri(string.Format("/Pages/RoomDetailsPage.xaml?Room={0}", room.Name), UriKind.Relative);
            return uri;
        }

        public static Uri Tweet(TweetItemViewModel tweet)
        {
            var uri = new Uri(string.Format( "/Pages/TweetDetailsPage.xaml?TweetId={0}", tweet.TweetId), UriKind.Relative);
            return uri;
        }

        public static Uri WebBrowser()
        {
            var uri = new Uri("/Pages/WebBrowserPage.xaml", UriKind.Relative);
            return uri;
        }

        public static Uri Diagnostics()
        {
            var uri = new Uri("/Pages/DiagnosticPage.xaml", UriKind.Relative);
            return uri;
        }

        public static Uri Settings()
        {
            return new Uri("/Pages/SettingsPage.xaml", UriKind.Relative);
        }
    }
}
