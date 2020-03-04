using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Notepad2.Views
{
    /// <summary>
    /// Interaction logic for ErrorReporterWindow.xaml
    /// </summary>
    public partial class ErrorReporterWindow : Window
    {
        public ErrorReporterWindow(string title, string text)
        {
            InitializeComponent();
            this.Title = title;
            ErrorTextBox.Text = text;
        }

        //public ErrorReporterWindow(string text)
        //{
        //    InitializeComponent();
        //    ErrorTextBox.Text = text;
        //}

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape: this.Close(); break;
                case Key.Enter: this.Close(); break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
