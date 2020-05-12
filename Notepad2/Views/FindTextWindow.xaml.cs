using Notepad2.Finding;
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
        //public List<FindResult> FoundText
        //{
        //    gwt 
        //}
        public int CurrentFindIndex;
        public bool HasAlreadySearched;
        public string FindingText { get => FindText.Text; set => FindText.Text = value; }

        public FindTextWindow()
        {
            InitializeComponent();
        }

        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            Find();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape: this.Hide(); break;
                case Key.Enter: Find(); break;
            }
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
            //FoundText = CharacterFinder.FindTextOccurrences()
        }
    }
}
