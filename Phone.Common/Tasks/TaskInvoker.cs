using Microsoft.Phone.Tasks;

namespace Phone.Common.Tasks
{
    public class TaskInvoker : ITaskInvoker
    {
        public void OpenWebBrowser(string url)
        {
            new WebBrowserTask()
            {
                URL = url
            }.Show();
        }

        public void SendEmail(string subject, string body)
        {
            new EmailComposeTask()
            {
                Subject = subject,
                Body = body
            }.Show();
        }

        public void SendEmail(string toEmailAdress, string subject, string body)
        {
            new EmailComposeTask()
            {
                To = toEmailAdress,
                Subject = subject,
                Body = body
            }.Show();
        }
    }
}
