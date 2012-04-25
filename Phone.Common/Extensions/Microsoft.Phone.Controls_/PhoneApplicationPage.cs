using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using System.Collections.Generic;
using System.Diagnostics;
using Phone.Common.Extensions.System.Windows_;

namespace Phone.Common.Extensions.Microsoft.Phone.Controls_
{
    public static class TombstoneHelper
    {
        #region Panorama

        public static void SaveState(this PhoneApplicationPage page, Panorama panorama)
        {
            page.State[panorama.Name + "_SelectedIndex"] = panorama.SelectedIndex;
        }

        public static void LoadState(this PhoneApplicationPage page, Panorama panorama)
        {
            int index = TryGetValue<int>(page.State, panorama.Name + "_SelectedIndex", 0);
            if (index > 0)
            {
                ScheduleOnNextRender(() => { panorama.DefaultItem = panorama.Items[index]; });
            }
        }

        #endregion

        #region Pivot

        public static void SaveState(this PhoneApplicationPage page, Pivot pivot)
        {
            page.State[pivot.Name + "_SelectedIndex"] = pivot.SelectedIndex;
        }

        public static void LoadState(this PhoneApplicationPage page, Pivot pivot)
        {
            int index = TryGetValue<int>(page.State, pivot.Name + "_SelectedIndex", 0);
            if (index > 0)
            {
                ScheduleOnNextRender(() => { pivot.SelectedIndex = index; });
            }
        }

        #endregion

        #region ListBox (ScrollViewer)

        public static void SaveState(this PhoneApplicationPage page, ListBox listBox)
        {
            page.State[listBox.Name + "_VerticalOffset"] = GetScrollViewer(listBox).VerticalOffset;
        }

        public static void LoadState(this PhoneApplicationPage page, ListBox listBox)
        {
            var offset = TryGetValue<double>(page.State, listBox.Name + "_VerticalOffset", 0);
            if (offset > 0)
            {
                ScheduleOnNextRender(() => GetScrollViewer(listBox).ScrollToVerticalOffset(offset));
            }
        }

        #endregion

        #region ListPicker
        public static void SaveState(this PhoneApplicationPage page, ListPicker listPicker)
        {
            page.State[listPicker.Name + "_SelectedIndex"] = listPicker.SelectedIndex;
        }

        public static void LoadState(this PhoneApplicationPage page, ListPicker listPicker)
        {
            var index = TryGetValue<int>(page.State, listPicker.Name + "_SelectedIndex", 1);
            ScheduleOnNextRender(() => { listPicker.SelectedIndex = index; });
            
        }
        #endregion

        #region LongListSelector

        public static void SaveState(this PhoneApplicationPage page, LongListSelector control)
        {
            /*
            var firstItem = control.GetItemsInView().FirstOrDefault();
            page.State[control.Name + "_FirstItem"] = firstItem;
            */
        }

        public static void LoadState(this PhoneApplicationPage page, LongListSelector control)
        {
            /*
            var firstItem = TryGetValue<object>(page.State, control.Name + "_FirstItem", 0);
            if (firstItem != null)
            {
                control.ScrollTo(firstItem);
                //ScheduleOnNextRender(() => GetScrollViewer(control).ScrollToVerticalOffset(offset));
            }
            */
        }

        #endregion

        #region TextBox

        public static void SaveState(this PhoneApplicationPage page, TextBox textBox)
        {
            page.State[textBox.Name + "_Text"] = textBox.Text;
            page.State[textBox.Name + "_SelectionStart"] = textBox.SelectionStart;
            page.State[textBox.Name + "_SelectionLength"] = textBox.SelectionLength;
        }

        public static void LoadState(this PhoneApplicationPage page, TextBox textBox)
        {
            textBox.Text = TryGetValue<string>(page.State, textBox.Name + "_Text", textBox.Text);
            textBox.SelectionStart = TryGetValue<int>(page.State, textBox.Name + "_SelectionStart", 0);
            textBox.SelectionLength = TryGetValue<int>(page.State, textBox.Name + "_SelectionLength", 0);
        }

        #endregion

        #region CheckBox

        public static void SaveState(this PhoneApplicationPage page, CheckBox checkBox)
        {
            page.State[checkBox.Name + "_IsChecked"] = checkBox.IsChecked;
        }

        public static void LoadState(this PhoneApplicationPage page, CheckBox checkBox)
        {
            checkBox.IsChecked = TryGetValue<bool?>(page.State, checkBox.Name + "_IsChecked", checkBox.IsChecked);
        }

        #endregion

        #region Slider

        public static void SaveState(this PhoneApplicationPage page, Slider slider)
        {
            page.State[slider.Name + "_Value"] = slider.Value;
        }

        public static void LoadState(this PhoneApplicationPage page, Slider slider)
        {
            slider.Value = TryGetValue<double>(page.State, slider.Name + "_Value", slider.Value);
        }

        #endregion

        #region RadioButton

        public static void SaveState(this PhoneApplicationPage page, RadioButton radioButton)
        {
            page.State[radioButton.Name + "_IsChecked"] = radioButton.IsChecked;
        }

        public static void LoadState(this PhoneApplicationPage page, RadioButton radioButton)
        {
            radioButton.IsChecked = TryGetValue<bool?>(page.State, radioButton.Name + "_IsChecked", radioButton.IsChecked);
        }

        #endregion

        private static T TryGetValue<T>(IDictionary<string, object> state, string name, T defaultValue)
        {
            if (state.ContainsKey(name))
            {
                if (state[name] != null)
                {
                    return (T)state[name];
                }
            }
            return defaultValue;
        }

        private static ScrollViewer GetScrollViewer(DependencyObject parent)
        {
            return parent.GetScrollViewer();
        }

        static List<Action> workItems;

        public static void ScheduleOnNextRender(Action action)
        {
            if (workItems == null)
            {
                workItems = new List<Action>();
                CompositionTarget.Rendering += DoWorkOnRender;
            }

            workItems.Add(action);
        }

        static void DoWorkOnRender(object sender, EventArgs args)
        {
            CompositionTarget.Rendering -= DoWorkOnRender;

            List<Action> work = workItems;
            workItems = null;

            foreach (Action action in work)
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {

                    if (Debugger.IsAttached)
                        Debugger.Break();

                    Debug.WriteLine("Exception while doing work for " + action.Method.Name + ". " + ex.Message);
                }
            }
        }

    }
}
