using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CodeStock.App.Core;
using CodeStock.App.Messaging;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Phone.Common.Diagnostics.Logging;

namespace CodeStock.App.ViewModels
{
    public class MainViewModel : AppViewModelBase
    {
        public MainViewModel(AppSettings settings)
            : base(registerCoreData: true)
        {
            _settings = settings;

            if (!IsInDesignMode)
            {
                this.IsBusy = true;
                this.BusyText = "Initializing app...";
                LogInstance.LogDebug("MainViewModel ctor");
                Messenger.Default.Register<AppInitializedMessage>(this, AppInitialized);
            }

            SetBackground();
        }

        private readonly AppSettings _settings;

        public ICommand ChildViewModelChangedCommand
        {
            get { return new RelayCommand<AppViewModelBase>(ChildViewModelChanged); }
        }

        private AppViewModelBase CurrentViewModel { get; set; }

        private void ChildViewModelChanged(AppViewModelBase viewModel)
        {
            LogInstance.LogDebug("Child view model changed to {0}", viewModel.GetType().Name);

            // in case a control hasn't set its datasource yet it can be this one
            // and we'd end up with stack overflow with activate on main calling activate on main
            if (viewModel == this) return;

            if (this.CurrentViewModel == viewModel) return;

            if (null != this.CurrentViewModel)
                this.CurrentViewModel.Deactivate();

            this.CurrentViewModel = viewModel;

            // it'd probably be cleaner to use messaging here but we'd have to filter out so only the desired view model
            // gets the message and more work and it's more work that seems unncessary here.
            // call custom activate event and if ViewModel overrides it may do custom work to load now that we've switched panorma items
            this.CurrentViewModel.Activate();
        }

        protected override void OnActivated()
        {
            if (IsInDesignMode) return;

            SetBackground();

            if (null != this.CurrentViewModel)
                this.CurrentViewModel.Activate();
        }

        private void SetBackground()
        {
            if (!string.IsNullOrEmpty(_settings.BackgroundImageUrl))
                this.BackgroundImage = new BitmapImage(new Uri(_settings.BackgroundImageUrl, UriKind.Relative));
            else
                this.BackgroundImage = null;
        }

        private void AppInitialized(AppInitializedMessage msg)
        {
            this.SetBusy(false);
        }

        private ImageSource _backgroundImage;

        public ImageSource BackgroundImage
        {
            get { return _backgroundImage; }

            set
            {
                if (_backgroundImage != value)
                {
                    _backgroundImage = value;
                    RaisePropertyChanged(() => BackgroundImage);
                }
            }
        }
    }
}