﻿using System.ComponentModel;
using System.Device.Location;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls.Maps;

namespace CodeStock.App.Controls
{
    public partial class MapControl //: UserControl
    {
        public MapControl()
        {
            InitializeComponent();
            SetBackground();
        }

        private void Map_ViewChangeEnd(object sender, MapEventArgs e)
        {
            var map = (Map)sender;

            Debug.WriteLine(string.Format("view changed. center location: {0}", map.Center));
        }

        private void SetBackground()
        {
            if (this.Parent != null || !DesignerProperties.IsInDesignTool)
                this.LayoutRoot.Background = new SolidColorBrush(Colors.Transparent);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SetBackground();
        }
    }
}