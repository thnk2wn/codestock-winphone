namespace Phone.Common.Tasks
{
    public interface ITaskInvoker
    {
        void OpenWebBrowser(string Url);
        void SendEmail(string toEmailAdress, string subject, string body);
        void SendEmail(string subject, string body);
    }
}
