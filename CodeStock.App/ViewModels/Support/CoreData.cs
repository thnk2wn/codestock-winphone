using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using CodeStock.App.Messaging;
using CodeStock.App.ViewModels.ItemViewModels;
using CodeStock.Data;
using CodeStock.Data.ServiceAccess;
using GalaSoft.MvvmLight.Messaging;
using System.Linq;
using Phone.Common.Diagnostics.Logging;
using Phone.Common.Extensions.System.Collections.Generic_;
using Phone.Common.IO;
using Phone.Common.IOC;
using Phone.Common.Threading;

namespace CodeStock.App.ViewModels.Support
{
    public class CoreData : ICoreData
    {
        // don't make this a singleton here; IOC module will use .InSingletonScope()
        // should never be here in DesignTime, different ICoreData

        public CoreData()
        {
            LogInstance.LogDebug("CoreData ctor()");

            RestoreFromTransientStorage();
        }

        public List<SessionItemViewModel> Sessions { get; set; }

        public List<SpeakerItemViewModel> Speakers { get; set; }

        public void LoadData()
        {
            QueueLoadData();
        }

        public void EnsureLoaded()
        {
            if (IsLoaded)
                return;

            QueueLoadData();
        }

        public bool IsLoaded {get; private set;}
        
        private void QueueLoadData()
        {
            if (null != this.SessionsWorker && this.SessionsWorker.IsBusy) return;
            if (null != this.SpeakersWorker && this.SpeakersWorker.IsBusy) return;

            LogInstance.LogInfo("Loading core data");

            this.SessionsWorker = new BackgroundWorker();
            this.SessionsWorker.DoWork += DoSessionsBackgroundWork;
            this.SessionsWorker.RunWorkerCompleted += SessionsBackgroundWorkDone;
            this.SessionsWorker.RunWorkerAsync();

            this.SpeakersWorker = new BackgroundWorker();
            this.SpeakersWorker.DoWork += DoSpeakersBackgroundWork;
            this.SpeakersWorker.RunWorkerCompleted += SpeakersBackgroundWorkDone;
            this.SpeakersWorker.RunWorkerAsync();
        }

        private void SpeakersBackgroundWorkDone(object sender, RunWorkerCompletedEventArgs e)
        {
            if (null != e.Error)
                throw e.Error;

            _speakersService = (ISpeakersService)e.Result;
            AfterServiceOperationCompleted();
        }

        private static void DoSpeakersBackgroundWork(object sender, DoWorkEventArgs e)
        {
            LogInstance.LogInfo("Getting speaker data");
            var client = IoC.Get<ISpeakersService>();
            var done = false;
            client.AfterCompleted = (cde) => done = true;
            client.Load();

            while (!done)
            {
                Thread.Sleep(100);
            }

            e.Result = client;
        }

        private static void DoSessionsBackgroundWork(object sender, DoWorkEventArgs e)
        {
            LogInstance.LogInfo("Getting session data");
            var client = IoC.Get<ISessionsService>();
            var done = false;
            client.AfterCompleted = (cde) => done = true;
            client.Load();

            while (!done)
            {
                Thread.Sleep(100);
            }

            e.Result = client;
        }

        private void SessionsBackgroundWorkDone(object sender, RunWorkerCompletedEventArgs e)
        {
            if (null != e.Error)
                throw e.Error;

            _sessionsService = (ISessionsService)e.Result;
            AfterServiceOperationCompleted();
        }

        private BackgroundWorker SessionsWorker { get; set; }
        private BackgroundWorker SpeakersWorker { get; set; }

        //private ICoreDataService _service;
        private ISessionsService _sessionsService;
        private ISpeakersService _speakersService;

        public CompletedEventArgs SessionCompletedInfo
        {
            get { return (null == _sessionsService) ? null : _sessionsService.CompletedInfo; }
        }

        public CompletedEventArgs SpeakerCompletedInfo
        {
            get { return  (null == _speakersService) ? null : _speakersService.CompletedInfo; }
        }
        
        private void AfterServiceOperationCompleted()
        {
            if (null == _sessionsService || null == _speakersService) return;

            // data loaded. not post processed yet but need indicator here
            this.IsLoaded = true;

            ThreadPool.QueueUserWorkItem(delegate
            {
                PostProcessThreadScope();
            });

            if (null != this.SessionsWorker)
            {
                this.SessionsWorker.DoWork -= DoSessionsBackgroundWork;
                this.SessionsWorker.RunWorkerCompleted -= SessionsBackgroundWorkDone;
                this.SessionsWorker = null;
            }

            if (null != this.SpeakersWorker)
            {
                this.SpeakersWorker.DoWork -= DoSpeakersBackgroundWork;
                this.SpeakersWorker.RunWorkerCompleted -= SpeakersBackgroundWorkDone;
                this.SpeakersWorker = null;
            }
        }

        public bool IsUnavailableFromError
        {
            get
            {
                if ( null != SessionCompletedInfo && null != SessionCompletedInfo.Error && (null == this.Sessions || !this.Sessions.Any()) )
                    return true;

                if ( null != SpeakerCompletedInfo && null != SpeakerCompletedInfo.Error && (null == this.Speakers || !this.Speakers.Any()) )
                    return true;

                return false;
            }
        }

        private void PostProcessThreadScope()
        {
            LogInstance.LogInfo("Core data loaded; processing");

            var merger = new CoreDataMerger();
            merger.Merge(_sessionsService, _speakersService);

            ThreadPool.QueueUserWorkItem(delegate
            {
                DispatchUtil.SafeDispatch(PostProcessSessionsThreadScope);
            });

            ThreadPool.QueueUserWorkItem(delegate
            {
                DispatchUtil.SafeDispatch(PostProcessSpeakersThreadScope);
            });
        }

        private void PostProcessSessionsThreadScope()
        {
            LogInstance.LogDebug("Processing sessions");
            var sessions = new List<SessionItemViewModel>();
            var data = _sessionsService.Data;
            if (null != data)
            {
                data.ForEach(s => sessions.Add(new SessionItemViewModel(s)));
            }
            this.Sessions = sessions;
            LogInstance.LogDebug("Sessions processed");
            CheckDone();
        }

        private void PostProcessSpeakersThreadScope()
        {
            LogInstance.LogDebug("Processing speakers");
            var speakers = new List<SpeakerItemViewModel>();
            var data = _speakersService.Data;
            if (null != data)
            {
                data.ForEach(sp => speakers.Add(new SpeakerItemViewModel(sp)));
            }
            LogInstance.LogDebug("Speakers processed");

            // speakers for each session already set
            this.Speakers = speakers;
            CheckDone();
        }

        private void CheckDone()
        {
            if (!IsLoaded) return;
            Messenger.Default.Send(new CoreDataLoadedMessage(this, _sessionsService.CompletedInfo, _speakersService.CompletedInfo));
            LogInstance.LogInfo("Core data post processing finished");

            DispatchTimerUtil.OnWithDispatcher(TimeSpan.FromSeconds(1), BackupToTransientStorage);
        }

        private void EnsureCache()
        {
            // can be null in a tombstone scenario
            if (null != _sessionsService && null != _speakersService)
            {
                this.CacheWorker = new BackgroundWorker();
                this.CacheWorker.DoWork += DoCacheBackgroundWork;
                this.CacheWorker.RunWorkerCompleted += CacheBackgroundWorkComplete;
                this.CacheWorker.RunWorkerAsync();

                //ThreadPool.QueueUserWorkItem(delegate
                //{
                //    _sessionsService.EnsureCached();
                //    _speakersService.EnsureCached();
                //});
            }
        }

        private void DoCacheBackgroundWork(object sender, DoWorkEventArgs e)
        {
            _sessionsService.EnsureCached();
            _speakersService.EnsureCached();
        }

        private void CacheBackgroundWorkComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            this.CacheWorker.DoWork -= DoCacheBackgroundWork;
            this.CacheWorker.RunWorkerCompleted -= CacheBackgroundWorkComplete;
            this.CacheWorker = null;
        }

        private BackgroundWorker CacheWorker { get; set; }

        private void BackupToTransientStorage()
        {
            LogInstance.LogInfo("Backing up core data to transient storage");
            var storage = new TransientDataStorage();
            storage.Backup("CoreData.Sessions", this.Sessions);
            storage.Backup("CoreData.Speakers", this.Speakers);
            LogInstance.LogInfo("Core data backup complete");
        }

        private void RestoreFromTransientStorage()
        {
            LogInstance.LogInfo("Restoring from transient storage");
            var storage = new TransientDataStorage();
            this.Sessions = storage.Restore<List<SessionItemViewModel>>("CoreData.Sessions");
            this.Speakers = storage.Restore<List<SpeakerItemViewModel>>("CoreData.Speakers");

            LogInstance.LogInfo((null == this.Sessions) ? "Sessions not found in storage" : "Restored sessions from storage");
            LogInstance.LogInfo((null == this.Speakers) ? "Speakers not found in storage" : "Restored speakers from storage");

            if (null == this.Sessions)
                this.Sessions = new List<SessionItemViewModel>();

            if (null == this.Speakers)
                this.Speakers = new List<SpeakerItemViewModel>();

            this.IsLoaded = (this.Sessions.Any() && this.Speakers.Any());

            LogInstance.LogInfo("Restore complete. Data found? : {0}", this.IsLoaded);
        }

        private bool _sessionsBound;
        public bool SessionsBound
        {
            get { return _sessionsBound; }
            set
            {
                if (_sessionsBound != value)
                {
                    _sessionsBound = value;
                    CheckAppSpunUp();
                }
            }
        }

        private bool _speakersBound;
        public bool SpeakersBound
        {
            get { return _speakersBound; }
            set
            {
                if (_speakersBound != value)
                {
                    _speakersBound = value;
                    CheckAppSpunUp();
                }
            }
        }

        private bool _appSpunup;

        private void CheckAppSpunUp()
        {
            if (_appSpunup) return;

            if (this.SpeakersBound && this.SessionsBound)
            {
                _appSpunup = true;
                var app = IoC.Get<IApp>();

                if (app.IsNewApp)
                {
                    var ts = DateTime.Now - app.StartupTime;
                    LogInstance.LogInfo("Approx app spinup time: {0:##.000} seconds", ts.TotalSeconds);
                    // 10.572 initial no-cache load with one background worker

                    //DispatchTimerUtil.OnWithDispatcher(TimeSpan.FromSeconds(1), EnsureCache);
                    EnsureCache();
                }

                Messenger.Default.Send(new AppInitializedMessage());
            }
        }

        public ObservableCollection<SessionItemViewModel> SessionsForSpeaker(int speakerId)
        {
            var collection = new ObservableCollection<SessionItemViewModel>();

            if (null != _sessionsService && null != _sessionsService.Data)
            {
                _sessionsService.Data.OrderBy(s=> s.StartTime).Where(s => s.SpeakerId == speakerId)
                    .ForEach(s => collection.Add(new SessionItemViewModel(s)));
            }

            return collection;
        }
    }
}
