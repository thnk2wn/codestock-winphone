using System;
using System.Text;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Info;
using Microsoft.Phone.Tasks;
using Phone.Common.Diagnostics.Logging;

namespace CodeStock.App.ViewModels
{
    public class DiagnosticViewModel : AppViewModelBase
    {
        public DiagnosticViewModel(ILogManager logManager)
        {
            _logManager = logManager;

            LoadData();
        }

        private readonly ILogManager _logManager;

        private void LoadData()
        {
            if (IsInDesignMode)
            {
                DesignTimeLoad();
                return;
            }


            this.LogText = _logManager.LogBuffer();

#if DEBUG
            this.MemoryText = string.Format("Memory Usage: {0:##.0} MB  ({1:##.0} Peak)",
                GetMemoryUsageInMb(), GetPeakMemoryUsageInMb());
#endif
            this.HasMemoryInfo = !string.IsNullOrEmpty(this.MemoryText);
        }

        private void DesignTimeLoad()
        {
            var sb = new StringBuilder();
            sb.AppendLine("2012-04-29 16:43:02.1850 - Core data loaded; processing");
            sb.AppendLine("2012-04-29 16:43:02.1900 - Processing sessions");
            sb.AppendLine("2012-04-29 16:43:04.0070 - Sessions processed");
            sb.AppendLine("2012-04-29 16:43:04.0150 - Processing speakers");
            sb.AppendLine("2012-04-29 16:43:04.4220 - Speakers processed");
            sb.AppendLine("2012-04-29 16:43:04.4320 - Backing up core data to transient storage");
            sb.AppendLine("2012-04-29 16:43:04.4520 - Core data backup complete");
            sb.AppendLine("2012-04-29 16:43:04.4820 - Core data post processing finished");
            this.LogText = sb.ToString();

            this.MemoryText = string.Format("Memory Usage: {0:##.0} MB  ({1:##.0} Peak)",
               52.1, 55.2);
            this.HasMemoryInfo = true;

        }

        private string _logText;

        public string LogText
        {
            get { return _logText; }
            set
            {
                if (_logText != value)
                {
                    _logText = value;
                    RaisePropertyChanged(() => LogText);
                }
            }
        }

        public bool HasMemoryInfo { get; set; }

        public ICommand EmailCommand
        {
            get { return new RelayCommand(EmailLog); }
        }

        private void EmailLog()
        {
            var emailTask = new EmailComposeTask
            {
                Body = this.LogText,
                Subject = "CodeStock Diagnostic Log",
                To = string.Empty
            };
            emailTask.Show();
        }

        private string _memoryText;

        public string MemoryText
        {
            get { return _memoryText; }
            set
            {
                if (_memoryText != value)
                {
                    _memoryText = value;
                    RaisePropertyChanged(() => MemoryText);
                }
            }
        }

#if DEBUG
        private static double GetMemoryUsageInMb()
        {
            try
            {
                var memoryString = DeviceExtendedProperties.GetValue("ApplicationCurrentMemoryUsage");
                var memoryUsageInMb = Convert.ToInt32(memoryString) / (1024 * 1024.0);
                return memoryUsageInMb;
            }
            catch
            {
            }
            return 0;
        }

        private static double GetPeakMemoryUsageInMb()
        {
            try
            {
                var memoryString = DeviceExtendedProperties.GetValue("ApplicationPeakMemoryUsage");
                var memoryUsageInMb = Convert.ToInt32(memoryString) / (1024 * 1024.0);
                return memoryUsageInMb;
            }
            catch
            {
            }
            return 0;
        }
#endif
    }
}
