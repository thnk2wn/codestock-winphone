using System.Windows.Data;
using System.Windows.Navigation;
using CodeStock.App.ViewModels;
using Microsoft.Phone.Controls;
using Phone.Common.IO;
using Phone.Common.Windows;

namespace CodeStock.App.Pages
{
    public partial class WebBrowserPage 
    {

        public WebBrowserPage()
        {
            InitializeComponent();
            webBrowser.Navigated += webBrowser_Navigated;
            webBrowser.Navigating += webBrowser_Navigating;
        }

        private void webBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            // most ugly: twitter change mobile auth which causes wp7 webbrowser issues: http://forums.create.msdn.com/forums/t/81946.aspx
            if (e.Uri.AbsoluteUri.StartsWith("http://twitter.com/share"))
                webBrowser.IsScriptEnabled = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // for some reason in mango this no longer worked binding in xaml.
            this.SupportedOrientations = this.ViewModel.SupportedOrientation;

            this.SetBinding(VSM.StateNameProperty, new Binding("CurrentStateName"));

            if (null == this.ViewModel.InitialSource)
                this.ViewModel.Restore(new TransientDataStorage());
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            this.ViewModel.Backup(new TransientDataStorage());
        }

        private void webBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            this.ViewModel.CurrentSource = webBrowser.Source;

            // most ugly: twitter change mobile auth which causes wp7 webbrowser issues: http://forums.create.msdn.com/forums/t/81946.aspx
            if (webBrowser.Source.AbsoluteUri.StartsWith("http://twitter.com/intent/session?return_to"))
                webBrowser.IsScriptEnabled = true;
        }

        private WebBrowserViewModel ViewModel
        {
            get { return this.DataContext as WebBrowserViewModel; }
        }
    }
}