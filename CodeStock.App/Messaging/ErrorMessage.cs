using System;
using GalaSoft.MvvmLight.Messaging;

namespace CodeStock.App.Messaging
{
    public class ErrorMessage : MessageBase
    {
        public Exception Error { get; set; }

        public string FriendlyError { get; set; }

        public string Title { get; set; }
    }
}
