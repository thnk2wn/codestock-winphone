using System.ComponentModel;
using System.Windows.Navigation;
using CodeStock.App.ViewModels.ItemViewModels;
using Phone.Common.Extensions.Microsoft.Phone.Controls_;

namespace CodeStock.App.Pages
{
    public partial class RoomDetailsPage //: PhoneApplicationPage
    {
        public RoomDetailsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (DesignerProperties.IsInDesignTool)
            {
                return;
            }

            var room = this.NavigationContext.QueryString["Room"];

            this.ViewModel = new RoomItemViewModel { Name = room };

            this.LoadState(uxSessionList);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.SaveState(uxSessionList);
        }

        private RoomItemViewModel ViewModel
        {
            get { return this.DataContext as RoomItemViewModel; }
            set
            {
                this.DataContext = value;
            }
        }

    }
}