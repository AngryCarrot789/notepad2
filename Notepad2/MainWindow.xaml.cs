using Notepad2.Notepad;
using Notepad2.Utilities;
using Notepad2.ViewModels;
using Notepad2.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        public bool IsDuplicatedWindow { get; set; }
        public bool DarkThemeEnabled { get; set; }
        public App CurrentApp;
        public MainViewModel ViewModel { get; set; }
        private double AnimationSpeedSeconds = 0.2;
        // Find Window
        public FindTextWindow FindWindow { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            InitWindow();
            ViewModel.NewNotepad();
        }

        public MainWindow(string filePath)
        {
            InitializeComponent();
            InitWindow();
            ViewModel.OpenNotepadFileFromPath(filePath);
        }

        public MainWindow(string filePath, bool enableSettingsSave)
        {
            InitializeComponent();
            InitWindow();
            ViewModel.OpenNotepadFileFromPath(filePath);
            IsDuplicatedWindow = enableSettingsSave;
            Title = "SharpPad";
        }

        public MainWindow(NotepadListItem fileItem, bool enableSettingsSave)
        {
            InitializeComponent();
            InitWindow();
            ViewModel.AddNotepadItem(fileItem);
            IsDuplicatedWindow = enableSettingsSave;
            Title = "SharpPad";
        }

        public void InitWindow()
        {
            NotepadActions.richTB = this.mainRTB;
            ViewModel = new MainViewModel();
            this.DataContext = ViewModel;
            ViewModel.MainWind = this;
            ViewModel.AnimateAddCallback = this.AnimateControl;

            FindWindow = new FindTextWindow();
            FindWindow.FindNext = this.FindText;

            KeydownManager.KeyDown += KeyPressedDown;
        }

        private void KeyPressedDown(Key key)
        {
            if (KeydownManager.Keydown(Key.LeftCtrl) && KeydownManager.Keydown(Key.F))
            {
                //WIP
                //FindWindow.Show();
            }
        }

        public void FindText(string textToBeFound)
        {
            MainTextBox.SelectedText = textToBeFound;
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

        public void AnimateControl(NotepadListItem nli, AnimationFlag af)
        {
            switch (af)
            {
                case AnimationFlag.NotepadItemOPEN:
                {
                    AnimationLib.OpacityControl(nli, 0, 1, AnimationSpeedSeconds);
                    AnimationLib.MoveToTargetX(nli, 0, -ActualWidth, AnimationSpeedSeconds);
                }
                break;

                //Cant really do this. It's async so i'd have to have a timed delay when
                //removing the item from the NotepadList, which would be... a bit complex.
                //If anyone wants to try and add that delay (using tasks maybe, have a go)
                //(and msg me or something with the code, and ill add it with your name in the code obviously ;) )
                case AnimationFlag.NotepadItemCLOSE:
                {
                    //AnimationLib.OpacityControl(nli, 1, 0, AnimationSpeedSeconds);
                    //AnimationLib.MoveToTargetX(nli, -ActualWidth, 0, AnimationSpeedSeconds * 15);
                    //
                    //Task.Run(async () =>
                    //{
                    //    await Task.Delay(TimeSpan.FromSeconds(AnimationSpeedSeconds));
                    //    await Application.Current.Dispatcher.InvokeAsync(() => { ViewModel.NotepadItems.Remove(nli); });
                    //});
                }
                break;
            }
        }

        public void SetTheme(Theme theme)
        {
            if (CurrentApp != null) CurrentApp.SetTheme(theme);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ViewModel.Shutdown())
            {
                MessageBoxResult mbr = MessageBox.Show(
                    "You have unsaved work. Do you want to save it/them?",
                    "Unsaved Work",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Information);

                if (mbr == MessageBoxResult.Yes)
                {
                    ViewModel.SaveAllNotepadItems();
                }
                if (mbr == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
                else if (mbr == MessageBoxResult.No)
                {
                    //nothin ;)
                }
            }
            if (!IsDuplicatedWindow)
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
            }
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
            if (ViewModel != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if (e.Data.GetData(DataFormats.FileDrop) is string[] droppedItemArray)
                {
                    foreach (string path in droppedItemArray)
                    {
                        string text = File.ReadAllText(path);

                        ViewModel.AddNotepadItem(
                            ViewModel.CreateDefaultStyleNotepadItem(
                                text,
                                Path.GetFileName(path),
                                path,
                                text.Length / 1000.0));
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
            this.Close();
        }

        public void MaximizeRestore()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                //Margin = new Thickness(0, 0, 0, 0);
            }
            else if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
                //WindowChrome Margin = new Thickness(8,8,8,8);
            }
        }

        public void Minimize()
        {
            this.WindowState = WindowState.Minimized;
        }

        private void TextBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (KeydownManager.Keydown(Key.LeftCtrl))
            {
                int fontChange = (e.Delta / 100);
                if (ViewModel.Notepad.DocumentFormat.Size > 1)
                {
                    ViewModel.Notepad.DocumentFormat.Size += fontChange;
                }
                if (ViewModel.Notepad.DocumentFormat.Size == 1 && fontChange >= 1)
                {
                    ViewModel.Notepad.DocumentFormat.Size += fontChange;
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            KeydownManager.SetKeyDown(e.Key);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            KeydownManager.SetKeyUp(e.Key);
        }
    }
}
