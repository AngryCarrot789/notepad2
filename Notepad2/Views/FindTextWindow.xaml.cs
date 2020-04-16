using System;
using System.Windows;
using System.Windows.Input;

namespace Notepad2.Views
{
    /// <summary>
    /// Interaction logic for FindTextWindow.xaml
    /// </summary>
    public partial class FindTextWindow : Window
    {
        public Action<string, bool, bool> FindNext { get; set; }

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
            FindNext?.Invoke(FindText.Text, matchCaseCBx.IsChecked ?? false, directionCBx.IsChecked ?? false);
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
