using System;
using System.Runtime.Serialization;
using System.Threading;
using CodeStock.App.Messaging;
using CodeStock.App.ViewModels.ItemViewModels;
using CodeStock.App.ViewModels.Support;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Phone.Common.Diagnostics.Logging;
using Phone.Common.Extensions.System.Linq.Expressions_;
using Phone.Common.IOC;
using Phone.Common.Threading;
using Phone.Common.Windows;
using System.Linq.Expressions;

namespace CodeStock.App.ViewModels
{

    // [KnownType] is ugly BS but for the moment it is required. http://dotnet.dzone.com/news/wcf-tutorial-serializing-and
    [KnownType(typeof(SpeakerItemViewModel))]
    [KnownType(typeof(SessionItemViewModel))]
    public class AppViewModelBase : ViewModelBase
    {
        public AppViewModelBase() : this(false)
        {
            return;
        }

        protected AppViewModelBase(bool registerCoreData)
        {
            if (registerCoreData)
                RegisterCoreData();
        }

        protected void RegisterCoreData()
        {
            Messenger.Default.Register<CoreDataLoadedMessage>(this, AfterCoreDataLoaded);
        }

        protected void AfterCoreDataLoaded(CoreDataLoadedMessage msg)
        {
            SafeDispatch(() =>
            {
                var coreData = IoC.Get<ICoreData>();
                // this can be called again if a manual refresh etc.
                //Messenger.Default.Unregister<CoreDataLoadedMessage>(this, AfterCoreDataLoaded);
                OnCoreDataLoaded(coreData);
            });
        }

        protected ICoreData CoreData()
        {
            return IoC.Get<ICoreData>();
        }

        /// <summary>
        /// Performs specified action if core data is loaded / present. 
        /// Usually used in tombstoning scenarios
        /// </summary>
        /// <param name="action"></param>
        protected void WithCoreDataPresent(Action<ICoreData> action)
        {
            var cd = CoreData();
            if (cd.IsLoaded)
            {
                LogInstance.LogInfo("CoreData is loaded; performing action for {0}", this.GetType().Name);
                action(cd);
            }
            else
                LogInstance.LogDebug("CoreData is not yet loaded ({0})", this.GetType().Name);
        }

        protected virtual void OnCoreDataLoaded(ICoreData coreData)
        {
            return;
        }

        protected void SafeDispatch(Action action)
        {
            if (!IsInDesignMode)
                DispatchUtil.SafeDispatch(action);
            else
                action();
        }

        protected void QueueSafeDispatch(Action action)
        {
            if (IsInDesignMode)
            {
                action();
                return;
            }

            ThreadPool.QueueUserWorkItem(delegate
            {
                SafeDispatch(action);
            });
        }

        protected void SendErrorDialogMessage(Exception error, string friendlyError, string title)
        {
            SafeDispatch(() =>
            {
                var message = new ErrorMessage { Error = error, FriendlyError = friendlyError, Title = title };
                Messenger.Default.Send(message);
            });

        }


        protected void MessageBox(string message, string caption)
        {
            IoC.Get<IMessageBoxService>().ShowOKDispatch(message, caption);
        }

        protected bool MessageBoxOKCancel(string message, string caption)
        {
            return IoC.Get<IMessageBoxService>().ShowOkCancel(message, caption);
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    RaisePropertyChanged(() => IsBusy);
                    OnBusyChanged();
                }
            }
        }

        protected virtual void OnBusyChanged()
        {
            return;
        }

        public bool IsBusySafe()
        {
            var busy = false;
            SafeDispatch(() => busy = IsBusy);
            return busy;
        }

        private string _busyText;

        public string BusyText
        {
            get { return _busyText; }

            set
            {
                if (_busyText != value)
                {
                    _busyText = value;
                    RaisePropertyChanged(() => BusyText);
                }
            }
        }

        protected virtual void SetBusy(bool isBusy)
        {
            this.IsBusy = isBusy;

            if (!isBusy)
                this.BusyText = string.Empty;
        }

        protected virtual void SetBusy(bool isBusy, string busyText)
        {
            this.BusyText = busyText;
            //this.IsBusy = (string.IsNullOrEmpty(busyText));
            this.IsBusy = isBusy;
        }

        private bool _isActive;

        public bool IsActive
        {
            get { return _isActive; }

            private set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    RaisePropertyChanged(() => IsActive);
                }
            }
        }
        
        public void Activate()
        {
            this.IsActive = true;
            OnActivated();
        }

        public void Deactivate()
        {
            this.IsActive = false;
            OnDeactivated();
        }

        /// <summary>
        /// Method that can be overriden in derived ViewModel that is intended for use when the ViewModel 
        /// "becomes active again" - i.e. back navigation or switching to a panorma or pivot item; in this 
        /// case the ViewModel is not being created from scratched but it may need to refresh some data
        /// upon return to the View/ViewModel.
        /// </summary>
        protected virtual void OnActivated()
        {
            return;
        }

        protected virtual void OnDeactivated()
        {
            return;
        }

        // IsInDesignMode is in base

        public static DateTime Now()
        {
#if DEBUG
            return DateTime.Parse("06/15/2012 01:30 PM"); // testing
#else
            return DateTime.Now;
#endif
            //return DateTime.Now;
        }

        private string _state;
        public string CurrentStateName
        {
            get { return _state; }
            set
            {
                _state = value;
                this.RaisePropertyChanged(() => CurrentStateName);
            }
        }

        protected void RaisePropertyChangedUpdateState<T>(Expression<Func<T>> property, bool value)
        {
            var propName = property.GetMemberName();
            RaisePropertyChanged(propName);

            if (value)
            {
                CurrentStateName = propName;
            }
            else
            {
                CurrentStateName = "Not" + propName;
            }
        }
    }
}
