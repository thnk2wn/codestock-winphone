using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Navigation;
using CodeStock.App.ViewModels.ItemViewModels;
using CodeStock.App.ViewModels.Support;
using Phone.Common.Diagnostics.Logging;
using Phone.Common.Extensions.System.Collections.Generic_;
using Phone.Common.IO;
using Phone.Common.IOC;

namespace CodeStock.App.Pages
{
    public partial class SpeakerDetailsPage //: PhoneApplicationPage
    {
        public SpeakerDetailsPage()
        {
            InitializeComponent();
            this.LayoutUpdated += SpeakerDetailsPage_LayoutUpdated;
        }

        void SpeakerDetailsPage_LayoutUpdated(object sender, EventArgs e)
        {
            if (!this.ScrollOffset.HasValue) return;

            uxScrollViewer.ScrollToVerticalOffset(this.ScrollOffset.Value);
            this.ScrollOffset = null;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (DesignerProperties.IsInDesignTool) return;

            var speakerIdArg = this.NavigationContext.QueryString["SpeakerId"];
            var speakerId = Convert.ToInt32(speakerIdArg);

            var store = new TransientDataStorage();
            var vm = store.Restore(() => ViewModel);

            if (null != vm && vm.SpeakerId == speakerId)
            {
                this.ViewModel = vm;
                this.ScrollOffset = store.Restore<double>(ScrollPos);
                LogInstance.LogInfo("Restored from transient storage: {0}", vm);
                return;
            }

            var coreData = IoC.Get<ICoreData>();
            var speakers = coreData.Speakers;
            this.ViewModel = speakers.Where(i => i.SpeakerId == speakerId).FirstOrDefault();
        }

        private double? ScrollOffset { get; set; }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            var store = new TransientDataStorage();
            store.Backup(() => ViewModel, this.ViewModel);
            store.Backup(ScrollPos, uxScrollViewer.VerticalOffset);
            LogInstance.LogInfo("Backed up to transient storage: {0}", this.ViewModel);
        }

        private const string ScrollPos = "SpeakerDetails.ScrollPos";

        private void SetSessionsForSpeaker()
        {
            // since Sessions for a speaker does not get serialized we need to set here on an app reactivation
            var coreData = IoC.Get<ICoreData>();
            var sessions = coreData.Sessions.Where(s => s.SpeakerId == this.ViewModel.SpeakerId).ToList();
            this.ViewModel.Sessions = sessions.ToObservableCollection();
        }

        private SpeakerItemViewModel ViewModel
        {
            get { return this.DataContext as SpeakerItemViewModel; }
            set
            {
                this.DataContext = value;

                if (null != value && (value.Sessions == null || !value.Sessions.Any()))
                    SetSessionsForSpeaker();
            }
        }
    }
}