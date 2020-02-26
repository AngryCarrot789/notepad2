using Notepad2.ViewModels;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Animation;

namespace Notepad2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainViewModel();
            this.DataContext = ViewModel;
            ViewModel.NewNotepad();
            this.Top = Properties.Settings.Default.Top;
            this.Left = Properties.Settings.Default.Left;
            this.Height = Properties.Settings.Default.Height;
            this.Width = Properties.Settings.Default.Width;

        }

        public MainWindow(string filePath)
        {
            InitializeComponent();
            ViewModel = new MainViewModel();
            this.DataContext = ViewModel;

            this.Top = Properties.Settings.Default.Top;
            this.Left = Properties.Settings.Default.Left;
            this.Height = Properties.Settings.Default.Height;
            this.Width = Properties.Settings.Default.Width;

            ViewModel.OpenNotepadFileFromPath(filePath);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                // Use the RestoreBounds as the current values will be 0, 0 and the size of the screen
                Properties.Settings.Default.Top = RestoreBounds.Top;
                Properties.Settings.Default.Left = RestoreBounds.Left;
                Properties.Settings.Default.Height = RestoreBounds.Height;
                Properties.Settings.Default.Width = RestoreBounds.Width;
            }
            else
            {
                Properties.Settings.Default.Top = this.Top;
                Properties.Settings.Default.Left = this.Left;
                Properties.Settings.Default.Height = this.Height;
                Properties.Settings.Default.Width = this.Width;
            }

            Properties.Settings.Default.Save();
        }
        private bool panelShowing;
        private void ShowFontDialogPanel(object sender, RoutedEventArgs e)
        {
            if (!panelShowing)
            {
                DoubleAnimation da = new DoubleAnimation(0, 320, TimeSpan.FromMilliseconds(200));
                da.AccelerationRatio = 0.3;
                da.DecelerationRatio = 0.7;
                FontDialogPanel.BeginAnimation(WidthProperty, da);
                panelShowing = true;
            }

            else
            {
                DoubleAnimation da = new DoubleAnimation(320, 0, TimeSpan.FromMilliseconds(200));
                da.AccelerationRatio = 0.3;
                da.DecelerationRatio = 0.7;
                FontDialogPanel.BeginAnimation(WidthProperty, da);
                panelShowing = false;
            }
        }

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                if (ViewModel != null)
                {
                    foreach (string path in files)
                    {
                        string text = File.ReadAllText(path);
                        ViewModel.AddNotepadItem(
                            ViewModel.CreateDefaultStyleNotepadItem(
                                text,
                                Path.GetFileName(path),
                                path,
                                (double)text.Length / 1000.0));
                    }
                }
            }
        }
    }
}
