using Notepad2.Notepad;
using Notepad2.ViewModels;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shell;
using static Notepad2.App;

namespace Notepad2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //hannez u r a google
        //(ignore this if you aren't hannez)
        public bool DarkThemeEnabled { get; set; }
        public App CurrentApp;
        public MainViewModel ViewModel { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainViewModel();
            this.DataContext = ViewModel;
            ViewModel.NewNotepad();
        }

        public MainWindow(string filePath)
        {
            InitializeComponent();
            ViewModel = new MainViewModel();
            this.DataContext = ViewModel;
            ViewModel.OpenNotepadFileFromPath(filePath);
        }

        public MainWindow(NotepadListItem fileItem)
        {
            InitializeComponent();
            ViewModel = new MainViewModel();
            this.DataContext = ViewModel;
            ViewModel.AddNotepadItem(fileItem);
        }

        public void LoadSettings()
        {
            this.Top = Properties.Settings.Default.Top;
            this.Left = Properties.Settings.Default.Left;
            this.Height = Properties.Settings.Default.Height;
            this.Width = Properties.Settings.Default.Width;
            this.DarkThemeEnabled = Properties.Settings.Default.DarkTheme;

            if (DarkThemeEnabled)
                SetTheme(Theme.Dark);
            else
                SetTheme(Theme.Light);
        }

        public void SetTheme(Theme theme)
        {
            if (CurrentApp != null) CurrentApp.SetTheme(theme);
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

            Properties.Settings.Default.DarkTheme = this.DarkThemeEnabled;
            if (!this.ViewModel.CheckNotepadNull())
            {
                Properties.Settings.Default.DefaultFont = this.ViewModel.Notepad.DocumentFormat.Family.ToString();
                Properties.Settings.Default.DefaultFontSize = this.ViewModel.Notepad.DocumentFormat.Size;
            }
            Properties.Settings.Default.Save();

            ViewModel.Shutdown();
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

        private void SetTheme(object sender, RoutedEventArgs e)
        {
            switch (int.Parse(((MenuItem)sender).Uid))
            {
                //dark
                case 0: SetTheme(Theme.Dark); DarkThemeEnabled = true; break;
                //light
                case 1: SetTheme(Theme.Light); DarkThemeEnabled = false; break;
            }

        }

        public void CloseWindow()
        {
            //WindowStyle = WindowStyle.SingleBorderWindow;
            this.Close();
        }

        public void MaximizeRestore()
        {
            //WindowChrome chrome = WindowChrome.GetWindowChrome(this);
            //chrome.ResizeBorderThickness = new Thickness(10, 10, 10, 10);
            if (this.WindowState == WindowState.Maximized)
            {
                //WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Normal;
                //WindowStyle = WindowStyle.None;
            }
            else if (WindowState == WindowState.Normal)
            {
                //WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Maximized;
                //WindowStyle = WindowStyle.None;
            }
        }

        public void Minimize()
        {
            //WindowStyle = WindowStyle.SingleBorderWindow;
            this.WindowState = WindowState.Minimized;
        }
    }
}
