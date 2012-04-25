using System.Windows;
using Phone.Common.Threading;

namespace Phone.Common.Windows
{
    public class MessageBoxService : IMessageBoxService
    {
        #region IMessageBoxService Members

        public void ShowOKDispatch(string text, string header)
        {
            DispatchUtil.SafeDispatch(() => MessageBox.Show(text, header, MessageBoxButton.OK));
        }

        public bool ShowOkCancel(string text, string header)
        {
            return MessageBox.Show(text, header, MessageBoxButton.OKCancel) == MessageBoxResult.OK;
        }
        #endregion
    }
}
