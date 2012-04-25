using System.Windows;

namespace Phone.Common.Windows
{
    public interface IMessageBoxService
    {
        void ShowOKDispatch(string text, string header);
        bool ShowOkCancel(string text, string header);
    }
}
