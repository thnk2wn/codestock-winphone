using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Microsoft.Phone.Shell;

namespace Phone.Common.Extensions.Microsoft.Phone.Shell_
{
    public static class IApplicationBarExtensions
    {
        public static IEnumerable<IApplicationBarIconButton> GetButtons(this IApplicationBar bar)
        {
            return bar.Buttons.OfType<IApplicationBarIconButton>();
        }

        public static IEnumerable<IApplicationBarMenuItem> GetMenuItems(this IApplicationBar bar)
        {
            return bar.MenuItems.OfType<IApplicationBarMenuItem>();
        }

        public static bool RemoveButton(this IApplicationBar bar, Func<IApplicationBarIconButton, bool> condition)
        {
            var btns = GetButtons(bar);
            if (null == btns || !btns.Any()) return false;

            var btn = btns.Where(condition).FirstOrDefault();
            if (null == btn) return false;

            bar.Buttons.Remove(btn);
            return true;
        }

        public static bool RemoveMenuItem(this IApplicationBar bar, Func<IApplicationBarMenuItem, bool> condition)
        {
            var menuItems = GetMenuItems(bar);
            if (null == menuItems || !menuItems.Any()) return false;

            var menu = menuItems.Where(condition).FirstOrDefault();
            if (null == menu) return false;

            bar.MenuItems.Remove(menu);
            return true;
        }

        public static void RemoveAllItems(this IApplicationBar bar)
        {
            bar.Buttons.Clear();
            bar.MenuItems.Clear();
        }

        public static IApplicationBarIconButton InsertButton(this IApplicationBar bar, int index, string imageUrl, string text, ICommand cmd)
        {
            var btn = new ApplicationBarIconButton { IconUri = new Uri(imageUrl, UriKind.Relative), Text = text };
            btn.Click += (s, e) => cmd.Execute(null);
            bar.Buttons.Insert(index, btn);
            return btn;
        }

        public static IApplicationBarMenuItem InsertMenuItem(this IApplicationBar bar, int index, string text, ICommand cmd)
        {
            var menu = new ApplicationBarMenuItem { Text = text };
            menu.Click += (s, e) => cmd.Execute(null);
            bar.MenuItems.Insert(index, menu);
            return menu;
        }


    }
}
