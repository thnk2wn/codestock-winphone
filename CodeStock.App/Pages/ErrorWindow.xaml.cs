using System;
using System.Text;
using Phone.Common.Diagnostics.Logging;
using Phone.Common.IOC;
using Phone.Common.Tasks;

namespace CodeStock.App.Pages
{
    public partial class ErrorWindow //: PhoneApplicationPage
    {
        public ErrorWindow()
        {
            InitializeComponent();

            uxCloseButton.Click += (s, e) =>
                                       {
                                           this.DialogResult = true;
                                           this.Close();
                                       };

            uxSendButton.Click += (s, e) => IoC.Get<ITaskInvoker>().SendEmail(
                @"thnk2wn2@gmail.com", "CodeStock Error", BuildErrorReportBody());

        }

        public ErrorWindow(Exception error, string friendlyError) : this()
        {
            this.Error = error;
            this.FriendlyError = friendlyError;

            ProcessError();
        }

        public Exception Error { get; private set; }
        public string FriendlyError { get; private set; }
        
        private void ProcessError()
        {
            SetErrorDisplayText();
        }

        private string BuildErrorReportBody()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Please describe what happened: ");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("Technical Error Details");
            sb.AppendLine(new string('=', 20));
            
            // we could get phone diagnostic info here but that can throw security errors and requires elevated trust, cert check etc.
            var technicalText = this.Error.ToString();
            technicalText += Environment.NewLine + Environment.NewLine; //+ diagnosticInfo;

            sb.AppendLine(technicalText);

            var logMgr = IoC.TryGet<ILogManager>();

            if (null != logMgr)
            {
                sb.AppendLine();
                sb.AppendLine(logMgr.LogBuffer());
            }


            return sb.ToString();
        }

        private void SetErrorDisplayText()
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(FriendlyError))
                sb.AppendLine(FriendlyError + Environment.NewLine);
           
//#if DEBUG
            sb.AppendLine(this.Error.ToString());
            // make room for the large exception dump. if release it's autosized height
            this.Height = 550;
//#endif
        
            uxErrorTextBlock.Text = sb.ToString();
        }
    }
}