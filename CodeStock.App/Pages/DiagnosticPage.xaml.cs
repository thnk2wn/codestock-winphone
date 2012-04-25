using System;
using CodeStock.App.ViewModels;

namespace CodeStock.App.Pages
{
    public partial class DiagnosticPage //: PhoneApplicationPage
    {
        public DiagnosticPage()
        {
            InitializeComponent();
        }

        private void EmailHandler(object sender, EventArgs e)
        {
            this.ViewModel.EmailCommand.Execute(null);
        }

        private DiagnosticViewModel ViewModel
        {
            get { return this.DataContext as DiagnosticViewModel; }
        }
    }
}