using System.Windows.Input;
using CodeStock.Data;
using GalaSoft.MvvmLight.Command;

namespace CodeStock.App.ViewModels.ItemViewModels
{
    public class ErrorItemViewModel : AppViewModelBase
    {
        public ErrorItemViewModel(CompletedEventArgs e)
        {
            this.CompletedEventArgs = e;
        }

        public static ErrorItemViewModel FromCompletion(CompletedEventArgs e)
        {
            if (null == e || null == e.Error) return null;
            return new ErrorItemViewModel(e);
        }

        private CompletedEventArgs _completedEventArgs;

        public CompletedEventArgs CompletedEventArgs
        {
            get { return _completedEventArgs; }

            set
            {
                if (_completedEventArgs != value)
                {
                    _completedEventArgs = value;
                    RaisePropertyChanged(() => CompletedEventArgs);

                    this.HasError = (null != value && value.Error != null);
                }
            }
        }

        private bool _hasError;

        public bool HasError
        {
            get { return _hasError; }

            set
            {
                if (_hasError != value)
                {
                    _hasError = value;
                    RaisePropertyChanged(() => HasError);
                }
            }
        }

        public ICommand ShowErrorCommand
        {
            get { return new RelayCommand(ShowError); }
        }

        private void ShowError()
        {
            if (null == this.CompletedEventArgs || null == this.CompletedEventArgs.Error)
            {
                // shouldn't get here really
                MessageBox("No error to show", "Hmm");
                return;
            }

            var failure = this.CompletedEventArgs.RequestFailure;

            if (null != failure && failure.IsCommon && !string.IsNullOrEmpty(failure.FriendlyMessage))
            {
                MessageBox(failure.FriendlyMessage, "Sorry");
                return;
            }

            SendErrorDialogMessage(this.CompletedEventArgs.Error, null, "Sorry");
        }
    }
}
