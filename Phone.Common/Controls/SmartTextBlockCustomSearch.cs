using System.Windows;
using System.Text.RegularExpressions;

namespace Phone.Common.Controls
{
    /// <summary>
    /// custom search object to contain regex and item template to be used for creating custom search and replace w/ in the 
    /// smart textblock
    /// </summary>
    public class SmartTextBlockCustomSearch : DependencyObject
    {


        #region Regex (DependencyProperty)
        /// <summary>
        /// regex string for this custom search
        /// </summary>
        public string Regex
        {
            get { return (string)GetValue(RegexProperty); }
            set { SetValue(RegexProperty, value); }
        }
        public static readonly DependencyProperty RegexProperty =
            DependencyProperty.Register("Regex", typeof(string), typeof(SmartTextBlockCustomSearch),
              new PropertyMetadata(string.Empty));

        #endregion


        #region ItemTemplate (DependencyProperty)

        /// <summary>
        /// data template to use for the output of this custom search
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(SmartTextBlockCustomSearch),
              new PropertyMetadata(null));

        #endregion

        /// <summary>
        /// regex object for the given regex string
        /// </summary>
        /// <returns></returns>
        public Regex GetRegexObject()
        {
            return new Regex(this.Regex);
        }

    }
}
