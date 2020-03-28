using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace JiraTask.Controls
{
    public partial class SearchControl : UserControl
    {
        public SearchControl()
        {
            InitializeComponent();
        }

        public event EventHandler<SearchEventArgs> OnSearch;
        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            ExeccuteSearch(SearchedText);
        }

        private void TbxInput_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && e.Source is TextBox inpuTextBox)
            {
                ExeccuteSearch(inpuTextBox.Text);
            }
        }

        private void ExeccuteSearch(string text)
        {
            if (OnSearch != null)
            {
                var args = new SearchEventArgs();
                args.SearchText = text ?? string.Empty;
                OnSearch(this, args);
            }
        }

        public static readonly DependencyProperty SearchedTextProperty = DependencyProperty.Register(
            "SearchedText", typeof(string), typeof(SearchControl), new PropertyMetadata(default(string)));

        public string SearchedText
        {
            get { return (string)GetValue(SearchedTextProperty); }
            set { SetValue(SearchedTextProperty, value); }
        }
    }
    public class SearchEventArgs : EventArgs
    {
        public string SearchText { get; set; }
    }
}
