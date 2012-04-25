using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Collections.ObjectModel;

namespace Phone.Common.Controls
{

    /// <summary>
    /// textblock like control that will take a text string and will automaticly parse for urls / phone number / email addresses / twitter hash tags
    /// and create the correct corresponding phone tasks and hyper links for them.  Also can be wired to custom commands as opposed to phone tasks
    ///<contrib:SmartTextBlock ContentMargin="0,0,5,0" Text="my test of it catching a phone number : 410-327-9787 or (410)327-9787 or an email foo@mail.com or a web address http://www.cynergysystems.com or a hash #awesome or user @geekpunk">
    ///     <contrib:SmartTextBlock.CustomSearches>
    ///         <contrib:SmartTextBlockCustomSearch Regex="phone" >
    ///             <contrib:SmartTextBlockCustomSearch.ItemTemplate>
    ///                 <DataTemplate>
    ///                     <StackPanel Orientation="Horizontal">
    ///                         <TextBlock Text="***"/>
    ///                         <TextBlock Text="{Binding}"/>
    ///                     </StackPanel>
    ///                 </DataTemplate>
    ///             </contrib:SmartTextBlockCustomSearch.ItemTemplate>
    ///         </contrib:SmartTextBlockCustomSearch>
    ///     </contrib:SmartTextBlock.CustomSearches>
    /// </contrib:SmartTextBlock>
    ///                 
    /// </summary>
    public class SmartTextBlock : Grid
    {
        #region fields
        /// <summary>
        /// loaded to know if the control is loaded to parse and create the text
        /// </summary>
        private bool _loaded;

        #region action regex's
        /// <summary>
        /// regex for finding twitter tags for use in browser tasks
        /// </summary>
        private static Regex _twitterActionTagRegex = new Regex(@"( |^)[#|@].+?( |$)");

        /// <summary>
        /// regex for finding browser tasks
        /// </summary>
        private static Regex _browserActionRegex = new Regex(@"((https?://)?([-\w]+\.[-\w\.]+)+\w(:\d+)?(/([-\w/_\.]*(\?\S+)?)?)*)");

        /// <summary>
        /// regex for finding dialer tasks US
        /// </summary>
        private static Regex _dialerUSActionkRegex = new Regex(@"^\(?(?<AreaCode>[2-9]\d{2})(\)?)(-|.|\s)?(?<Prefix>[1-9]\d{2})(-|.|\s)?(?<Suffix>\d{4})$");
        private static Regex _dialerUKActionkRegex = new Regex(@"^(((\+44\s?\d{4}|\(?0\d{4}\)?)\s?\d{3}\s?\d{3})|((\+44\s?\d{3}|\(?0\d{3}\)?)\s?\d{3}\s?\d{4})|((\+44\s?\d{2}|\(?0\d{2}\)?)\s?\d{4}\s?\d{4}))(\s?\#(\d{4}|\d{3}))?$");

        /// <summary>
        /// regex for finding mail tasks
        /// </summary>
        private static Regex _mailActionkRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");

        #endregion

        #endregion

        #region inits
        public SmartTextBlock()
        {
            Loaded += new RoutedEventHandler(SmartTextBlock_Loaded);
        }

        void SmartTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            // on load set the value and parse the text
            _loaded = true;
            ParseAndCreate();
        }
        #endregion

        #region properties
        #region Text (DependencyProperty)

        /// <summary>
        /// Gets or sets Text to show as content
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(SmartTextBlock), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnTextChanged)));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SmartTextBlock)d).OnTextChanged(e);
        }

        protected virtual void OnTextChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_loaded)
            {
                ParseAndCreate();
            }
        }

        #endregion

        #region ContentMarginMargin (DependencyProperty)
        /// <summary>
        /// margin to set between textblocks and links
        /// </summary>
        public Thickness ContentMargin
        {
            get { return (Thickness)GetValue(ContentMarginProperty); }
            set { SetValue(ContentMarginProperty, value); }
        }

        public static readonly DependencyProperty ContentMarginProperty =
            DependencyProperty.Register("ContentMargin", typeof(Thickness), typeof(SmartTextBlock), new PropertyMetadata(null));

        #endregion

        #region TextBlockStyle (DependencyProperty)
        /// <summary>
        /// style for textblocks
        /// </summary>
        public Style TextBlockStyle
        {
            get { return (Style)GetValue(TextBlockStyleProperty); }
            set { SetValue(TextBlockStyleProperty, value); }
        }

        public static readonly DependencyProperty TextBlockStyleProperty =
            DependencyProperty.Register("TextBlockStyle", typeof(Style), typeof(SmartTextBlock), new PropertyMetadata(null));

        #endregion

        #region HyperlinkButtonStyle (DependencyProperty)
        /// <summary>
        /// style for hyperlinks
        /// </summary>
        public Style HyperlinkButtonStyle
        {
            get { return (Style)GetValue(HyperlinkButtonStyleProperty); }
            set { SetValue(HyperlinkButtonStyleProperty, value); }
        }

        public static readonly DependencyProperty HyperlinkButtonStyleProperty =
            DependencyProperty.Register("HyperlinkButtonStyle", typeof(Style), typeof(SmartTextBlock), new PropertyMetadata(null));

        #endregion

        #region TwitterUserActionCommand (DependencyProperty)

        /// <summary>
        /// command to run when twitter user link is clicked
        /// </summary>
        public ICommand TwitterUserActionCommand
        {
            get { return (ICommand)GetValue(TwitterUserActionCommandProperty); }
            set { SetValue(TwitterUserActionCommandProperty, value); }
        }
        public static readonly DependencyProperty TwitterUserActionCommandProperty =
            DependencyProperty.Register("TwitterUserActionCommand", typeof(ICommand), typeof(SmartTextBlock),
              new PropertyMetadata(null));

        #endregion

        #region TwitterHashActionCommand (DependencyProperty)

        /// <summary>
        /// command to run when twitter hash link is clicked
        /// </summary>
        public ICommand TwitterHashActionCommand
        {
            get { return (ICommand)GetValue(TwitterHashActionCommandProperty); }
            set { SetValue(TwitterHashActionCommandProperty, value); }
        }
        public static readonly DependencyProperty TwitterHashActionCommandProperty =
            DependencyProperty.Register("TwitterHashActionCommand", typeof(ICommand), typeof(SmartTextBlock),
              new PropertyMetadata(null));

        #endregion


        #region BrowserActionCommand (DependencyProperty)

        /// <summary>
        /// command to run when browser link is clicked
        /// </summary>
        public ICommand BrowserActionCommand
        {
            get { return (ICommand)GetValue(BrowserActionCommandProperty); }
            set { SetValue(BrowserActionCommandProperty, value); }
        }
        public static readonly DependencyProperty BrowserActionCommandProperty =
            DependencyProperty.Register("BrowserActionCommand", typeof(ICommand), typeof(SmartTextBlock),
              new PropertyMetadata(null));

        #endregion

        #region DialerActionCommand (DependencyProperty)

        /// <summary>
        /// command to run when Dialer link is clicked
        /// </summary>
        public ICommand DialerActionCommand
        {
            get { return (ICommand)GetValue(DialerActionCommandProperty); }
            set { SetValue(DialerActionCommandProperty, value); }
        }
        public static readonly DependencyProperty DialerActionCommandProperty =
            DependencyProperty.Register("DialerActionCommand", typeof(ICommand), typeof(SmartTextBlock),
              new PropertyMetadata(null));

        #endregion

        #region MailActionCommand (DependencyProperty)

        /// <summary>
        /// command to run when Mail link is clicked
        /// </summary>
        public ICommand MailActionCommand
        {
            get { return (ICommand)GetValue(MailActionCommandProperty); }
            set { SetValue(MailActionCommandProperty, value); }
        }
        public static readonly DependencyProperty MailActionCommandProperty =
            DependencyProperty.Register("MailActionCommand", typeof(ICommand), typeof(SmartTextBlock),
              new PropertyMetadata(null));

        #endregion


        #region IsTwitterLinkEnabled (DependencyProperty)

        /// <summary>
        /// should the text be searched for twitter links
        /// </summary>
        public bool IsTwitterLinkEnabled
        {
            get { return (bool)GetValue(IsTwitterLinkEnabledProperty); }
            set { SetValue(IsTwitterLinkEnabledProperty, value); }
        }
        public static readonly DependencyProperty IsTwitterLinkEnabledProperty =
            DependencyProperty.Register("IsTwitterLinkEnabled", typeof(bool), typeof(SmartTextBlock),
              new PropertyMetadata(true));

        #endregion


        #region EmailRegex (DependencyProperty)

        /// <summary>
        /// custom regex for email address searching.  Not required, will default back to internal regex
        /// </summary>
        public string EmailRegex
        {
            get { return (string)GetValue(EmailRegexProperty); }
            set { SetValue(EmailRegexProperty, value); }
        }
        public static readonly DependencyProperty EmailRegexProperty =
            DependencyProperty.Register("EmailRegex", typeof(string), typeof(SmartTextBlock),
            new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnEmailRegexChanged)));

        private static void OnEmailRegexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SmartTextBlock)d).OnEmailRegexChanged(e);
        }

        protected virtual void OnEmailRegexChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && e.NewValue != string.Empty)
            {
                _mailActionkRegex = new Regex((string)e.NewValue);
            }
        }

        #endregion


        #region PhoneRegex (DependencyProperty)

        /// <summary>
        /// custom regex for phone number searching.  Not required, will default back to internal regex
        /// </summary>
        public string PhoneRegex
        {
            get { return (string)GetValue(PhoneRegexProperty); }
            set { SetValue(PhoneRegexProperty, value); }
        }
        public static readonly DependencyProperty PhoneRegexProperty =
            DependencyProperty.Register("PhoneRegex", typeof(string), typeof(SmartTextBlock),
            new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnPhoneRegexChanged)));

        private static void OnPhoneRegexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SmartTextBlock)d).OnPhoneRegexChanged(e);
        }

        protected virtual void OnPhoneRegexChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && e.NewValue != string.Empty)
            {
                _dialerUSActionkRegex = new Regex((string)e.NewValue);
            }
        }

        #endregion


        #region WebRegex (DependencyProperty)

        /// <summary>
        /// custom regex for url searching.  Not required, will default back to internal regex
        /// </summary>
        public string WebRegex
        {
            get { return (string)GetValue(WebRegexProperty); }
            set { SetValue(WebRegexProperty, value); }
        }
        public static readonly DependencyProperty WebRegexProperty =
            DependencyProperty.Register("WebRegex", typeof(string), typeof(SmartTextBlock),
            new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnWebRegexChanged)));

        private static void OnWebRegexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SmartTextBlock)d).OnWebRegexChanged(e);
        }

        protected virtual void OnWebRegexChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && e.NewValue != string.Empty)
            {
                _browserActionRegex = new Regex((string)e.NewValue);
            }

        }

        #endregion





        #region CustomSearches (DependencyProperty)

        /// <summary>
        /// A description of the property.
        /// </summary>
        public ObservableCollection<SmartTextBlockCustomSearch> CustomSearches
        {
            get { return (ObservableCollection<SmartTextBlockCustomSearch>)GetValue(CustomSearchesProperty); }
            set { SetValue(CustomSearchesProperty, value); }
        }
        public static readonly DependencyProperty CustomSearchesProperty =
            DependencyProperty.Register("CustomSearches", typeof(ObservableCollection<SmartTextBlockCustomSearch>), typeof(SmartTextBlock),
            new PropertyMetadata(new ObservableCollection<SmartTextBlockCustomSearch>(), new PropertyChangedCallback(OnCustomSearchesChanged)));

        private static void OnCustomSearchesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SmartTextBlock)d).OnCustomSearchesChanged(e);
        }

        protected virtual void OnCustomSearchesChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion


        #endregion

        #region events


        #region BrowserActionTouch

        /// <summary>
        /// a browser link was touched
        /// </summary>
        public event RoutedEventHandler BrowserActionTouch;

        private void RaiseBrowserActionTouch(string content)
        {
            if (BrowserActionTouch != null)
            {
                BrowserActionTouch(content, new RoutedEventArgs());
            }
        }

        #endregion


        #region DialActionTouch

        /// <summary>
        /// a dialer link was touched
        /// </summary>
        public event RoutedEventHandler DialActionTouch;

        private void RaiseDialActionTouch(string content)
        {
            if (DialActionTouch != null)
            {
                DialActionTouch(content, new RoutedEventArgs());
            }
        }

        #endregion


        #region EmailActionTouch

        /// <summary>
        /// An email link was touched
        /// </summary>
        public event RoutedEventHandler MainActionTouch;

        private void RaiseMailActionTouch(string content)
        {
            if (MainActionTouch != null)
            {
                MainActionTouch(content, new RoutedEventArgs());
            }
        }

        #endregion

        #region TwitterUserActionTouch

        /// <summary>
        /// a twitter user link was touched
        /// </summary>
        public event RoutedEventHandler TwitterUserActionTouch;

        private void RaiseTwitterUserActionTouch(string content)
        {
            if (TwitterUserActionTouch != null)
            {
                TwitterUserActionTouch(content, new RoutedEventArgs());
            }
        }

        #endregion

        #region TwitterHashActionTouch

        /// <summary>
        /// a twitter hash link was touched
        /// </summary>
        public event RoutedEventHandler TwitterHashActionTouch;

        private void RaiseTwitterHashActionTouch(string content)
        {
            if (TwitterHashActionTouch != null)
            {
                TwitterHashActionTouch(content, new RoutedEventArgs());
            }
        }

        #endregion

        #endregion

        #region methods
        /// <summary>
        /// on text changed the text is parsed and texblock and hyperlinkbuttons are created
        /// </summary>
        private void ParseAndCreate()
        {
            // clear contents
            Children.Clear();

            // dont render if empty
            if (String.IsNullOrEmpty(Text))
            {
                return;
            }

            // if there are any phone / mail / twitter / web links then contrinue to create
            if (TextContainsActions(Text, IsTwitterLinkEnabled, CustomSearches))
            {
                // create a wrap panel and loop over all words to add textblocks or hyperlinks
                WrapPanel panel = new WrapPanel();
                string[] values = Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int x = 0; x < values.Length; x++)
                {
                    UIElement element = null;


                    // first look for a custom search and replace
                    if (CustomSearches != null && CustomSearches.Count > 0)
                    {
                        foreach (SmartTextBlockCustomSearch search in CustomSearches)
                        {
                            if (search.GetRegexObject().IsMatch(values[x]))
                            {
                                element = CreateCustomItem(values[x], search);
                                break;
                            }
                        }
                    }

                    // otherwise searcg for other matches
                    if (element == null)
                    {
                        if (_mailActionkRegex.IsMatch(values[x]))
                        {
                            element = CreateMailButton(values[x]);
                        }
                        else if (_browserActionRegex.IsMatch(values[x]))
                        {
                            element = CreateBrowserButton(values[x], string.Empty, string.Empty);
                        }
                        else if (_dialerUKActionkRegex.IsMatch(values[x]) || _dialerUSActionkRegex.IsMatch(values[x]))
                        {
                            element = CreateDialerButton(values[x]);
                        }
                        else if (_twitterActionTagRegex.IsMatch(values[x]) && IsTwitterLinkEnabled)
                        {
                            // twitter items will be parsed for hashes and user tags, and will send to the mobile site by default
                            if (values[x].Substring(0, 1) == "#")
                            {
                                string uri = string.Format("http://mobile.twitter.com/searches?q={0}", values[x].Substring(1, values[x].Length - 1));
                                element = CreateTwitterButton(uri, true, values[x]);
                            }
                            else if (values[x].Substring(0, 1) == "@")
                            {
                                string uri = string.Format("http://mobile.twitter.com/{0}", values[x].Replace("@", string.Empty));
                                element = CreateTwitterButton(uri, false, values[x]);
                            }
                        }
                        else
                        {
                            // didnt match any regex so this must not be a link, instead create a textblock
                            element = CreateTextBlock(values[x]);
                        }
                    }

                    // add each child to the wrap panel
                    panel.Children.Add(element);
                }

                Children.Add(panel);
            }
            // other wise just add the 1 textblock
            else
            {
                Children.Add(CreateTextBlock(Text));
            }
        }

        /// <summary>
        /// set up the hyper link style and margin
        /// </summary>
        /// <param name="btn"></param>
        private void SetUpHyperLinkStyle(HyperlinkButton btn)
        {
            if (HyperlinkButtonStyle != null)
                btn.Style = HyperlinkButtonStyle;

            Thickness m = btn.Margin;

            // do some negative margins to deal w/ hyperlink paddings to make it feel
            // more inline
            btn.Margin = new Thickness(m.Left -10, m.Top,m.Right -3, m.Bottom);

        }

        /// <summary>
        /// set up the hyperlink content
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="text"></param>
        /// <param name="prefix"></param>
        /// <param name="formated"></param>
        private void SetUpHyperLinkContent(HyperlinkButton btn, string text, string prefix, string formated)
        {
            if (formated != string.Empty)
            {
                btn.Content = formated;
            }
            else
            {
                if (prefix != string.Empty)
                    btn.Content = string.Format("{0} {1}", text, prefix);
                else
                    btn.Content = text;
            }
        }

        /// <summary>
        ///  create the custom element using the data template of the item
        /// </summary>
        /// <param name="text"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        private FrameworkElement CreateCustomItem(string text, SmartTextBlockCustomSearch search)
        {
            if (search.ItemTemplate != null)
            {
                FrameworkElement obj = search.ItemTemplate.LoadContent() as FrameworkElement;
                obj.DataContext = text;
                return obj;
            }
            else return null;
        }

        /// <summary>
        /// create a phone dialer button, wire up task or command
        /// </summary>
        /// <param name="text">phone number</param>
        /// <returns></returns>
        private HyperlinkButton CreateDialerButton(string text)
        {
            HyperlinkButton btn = new HyperlinkButton();
            if (DialerActionCommand != null||DialActionTouch!=null)
            {
                btn.Click += delegate(object sender, RoutedEventArgs args)
                {
                    if (DialerActionCommand != null)
                    {
                        DialerActionCommand.Execute(text);
                    }

                    RaiseDialActionTouch(text);
                };
            }
            else
            {
                btn.Click += delegate(object sender, RoutedEventArgs args)
                {
                    MessageBox.Show("Dialing a phone number is not currently supported due to capability / security / certification implications.", "Sorry", MessageBoxButton.OK);
                    //PhoneCallTask task = new PhoneCallTask();
                    //task.PhoneNumber = text;
                    //task.Show();
                };
            }

            SetUpHyperLinkContent(btn, text, string.Empty, string.Empty);

            SetUpHyperLinkStyle(btn);

            return btn;
        }


        /// <summary>
        /// create a browser button, wire up task or command
        /// </summary>
        /// <param name="text">raw text, url</param>
        /// <param name="prefix">any thing to show ahead of the item like [uri:]</param>
        /// <param name="formated">the formated string for example @geekpunk</param>
        /// <returns></returns>
        private HyperlinkButton CreateBrowserButton(string text, string prefix, string formated)
        {
            HyperlinkButton btn = new HyperlinkButton();
            if (BrowserActionCommand != null || BrowserActionTouch!=null)
            {
                btn.Click += delegate(object sender, RoutedEventArgs args)
                {
                    if (BrowserActionCommand != null)
                    {
                        BrowserActionCommand.Execute(text);
                    }

                    RaiseBrowserActionTouch(text);
                };
            }
            else
            {
                btn.Click += delegate(object sender, RoutedEventArgs args)
                {
                    WebBrowserTask task = new WebBrowserTask();
                    task.URL = text;
                    task.Show();
                };
            }

            SetUpHyperLinkContent(btn, text, prefix, formated);

            SetUpHyperLinkStyle(btn);

            return btn;
        }

        /// <summary>
        /// create a twitter button, wire up task or command
        /// </summary>
        /// <param name="text">raw text, url</param>
        /// <param name="isHash">is this a # or an @</param>
        /// <param name="formated">the formated string for example @geekpunk</param>
        /// <returns></returns>
        private HyperlinkButton CreateTwitterButton(string text, bool isHash, string formated)
        {
            HyperlinkButton btn = new HyperlinkButton();
            if (TwitterHashActionCommand != null || TwitterHashActionTouch != null||TwitterUserActionCommand!=null||TwitterUserActionTouch!=null)
            {
                btn.Click += delegate(object sender, RoutedEventArgs args)
                {
                    if (isHash)
                    {
                        if (TwitterHashActionCommand != null)
                        {
                            TwitterHashActionCommand.Execute(text);
                        }

                        RaiseTwitterHashActionTouch(text);
                    }
                    else
                    {
                        if (TwitterUserActionCommand != null)
                        {
                            TwitterUserActionCommand.Execute(text);
                        }

                        RaiseTwitterUserActionTouch(text);

                    }
                };
            }
            else
            {
                btn.Click += delegate(object sender, RoutedEventArgs args)
                {
                    WebBrowserTask task = new WebBrowserTask();
                    task.URL = text;
                    task.Show();
                };
            }

            SetUpHyperLinkContent(btn, text, string.Empty, formated);

            SetUpHyperLinkStyle(btn);

            return btn;
        }

        /// <summary>
        /// create a email button, wire up task or command
        /// </summary>
        /// <param name="text">email@mail.com</param>
        /// <returns></returns>
        private HyperlinkButton CreateMailButton(string text)
        {
            HyperlinkButton btn = new HyperlinkButton();
            if (MailActionCommand != null|| MainActionTouch!=null)
            {
                btn.Click += delegate(object sender, RoutedEventArgs args)
                {
                    if (MailActionCommand != null)
                    {
                        MailActionCommand.Execute(text);
                    }
                
                    RaiseMailActionTouch(text);

                };
            }
            else
            {
                btn.Click += delegate(object sender, RoutedEventArgs args)
                {
                    EmailComposeTask task = new EmailComposeTask();
                    task.To = text;
                    task.Show();
                };
            }

            SetUpHyperLinkContent(btn, text, string.Empty, string.Empty);

            SetUpHyperLinkStyle(btn);

            return btn;
        }

        /// <summary>
        /// create a textblock and apply the style and content
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private TextBlock CreateTextBlock(string text)
        {
            TextBlock t = new TextBlock();
            if (ContentMargin != null)
                t.Margin = ContentMargin;
            if (TextBlockStyle != null)
                t.Style = TextBlockStyle;

            t.TextWrapping = TextWrapping.Wrap;
            t.Text = text;

            return t;

        }

        /// <summary>
        /// search the text to see if the contents contains items which should become task links
        /// </summary>
        /// <param name="text">full text</param>
        /// <param name="searchTwitter">should the text be searched for twitter # and @</param>
        /// <returns></returns>
        private static bool TextContainsActions(string text, bool searchTwitter, ObservableCollection<SmartTextBlockCustomSearch> customs)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }
            bool result = false;
            if (searchTwitter)
            {
                result = _twitterActionTagRegex.IsMatch(text);
            }

            if (!result)
            {
                result = _twitterActionTagRegex.IsMatch(text) || _mailActionkRegex.IsMatch(text) || _dialerUSActionkRegex.IsMatch(text) || _dialerUKActionkRegex.IsMatch(text) || _browserActionRegex.IsMatch(text);
            }

            if (!result)
            {
                if (customs != null)
                {
                    if (customs.Count > 0)
                    {
                        foreach (SmartTextBlockCustomSearch search in customs)
                        {
                            result = search.GetRegexObject().IsMatch(text);
                            if (result)
                                break;
                        }
                    }
                }
            }

            return result;
        }

        #endregion
    }
}
