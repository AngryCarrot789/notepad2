using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace Notepad2.Views
{
    /// <summary>
    /// Interaction logic for FindTextWindow.xaml
    /// </summary>
    public partial class FindTextWindow : Window
    {
        public Action<string, bool, bool> FindNext { get; set; }

        public List<int> FoundTextIndexes;
        public int CurrentFindIndex;
        public bool HasAlreadySearched;
        public string FindingText;

        public FindTextWindow()
        {
            InitializeComponent();
        }

        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            Find();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        public void Find()
        {
            if (FindingText != FindText.Text)
            {
                HasAlreadySearched = false;
                CurrentFindIndex = 0;
            }
            FindNext?.Invoke(FindText.Text, matchCaseCBx.IsChecked ?? false, directionCBx.IsChecked ?? false);
            FindingText = FindText.Text;
        }

        public void SetPreviewText(string text)
        {
            PreviewText.Text = text;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape: this.Hide(); break;

                case Key.Enter: Find(); break;
            }
        }
    }
}
