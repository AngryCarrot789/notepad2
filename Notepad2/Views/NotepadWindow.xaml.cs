﻿using Notepad2.FileExplorer.ShellClasses;
using Notepad2.Finding;
using Notepad2.Notepad;
using Notepad2.Themes;
using Notepad2.Utilities;
using Notepad2.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Path = System.IO.Path;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;
using Notepad2.FileExplorer;
using Notepad2.Preferences;
using System.Threading.Tasks;
using System.Security.Policy;
using Notepad2.Applications;

namespace Notepad2.Views
{
    /// <summary>
    /// Interaction logic for NotepadWindow.xaml
    /// </summary>
    public partial class NotepadWindow : Window
    {
        private Point MouseDownPoint { get; set; }

        public bool CanSavePreferences { get; set; }

        public NotepadViewModel Notepad
        {
            get => this.DataContext as NotepadViewModel;
            set => this.DataContext = value;
        }

        public Action<NotepadWindow> WindowFocusedCallback { get; set; }
        public Action<NotepadWindow> WindowShownCallback { get; set; }
        public Action<NotepadWindow> WindowClosedCallback { get; set; }

        public NotepadWindow(NewWinPrefs prefs = NewWinPrefs.OpenBlankWindow)
        {
            BeforeInitComponents();
            InitializeComponent();
            InitWindow();
            if (prefs == NewWinPrefs.OpenDefaultNotepadAfterLaunch)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(10);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Notepad.AddStartupItem();
                    });
                });
            }
        }

        public NotepadWindow(string[] filePaths, NewWinPrefs prefs = NewWinPrefs.OpenFilesInParams)
        {
            BeforeInitComponents();
            InitializeComponent();
            InitWindow();
            try
            {
                foreach (string path in filePaths)
                {
                    if (path.IsFile())
                    {
                        Notepad.OpenNotepadFileFromPath(path, true);
                    }
                    if (path.IsDirectory())
                    {
                        MessageBoxResult a =
                            MessageBox.Show(
                                "Open all files in this directory?",
                                "Open entire directory",
                                MessageBoxButton.YesNo);
                        if (a == MessageBoxResult.Yes)
                        {
                            try
                            {
                                foreach (string file in Directory.GetFiles(path))
                                {
                                    Notepad.OpenNotepadFileFromPath(file, true);
                                }
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(
                                    $"Error opening all files in a directory:  {e.Message}",
                                    "Error opening a directory");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    $"Error opening files:  {e.Message}",
                    "Error opening files");
            }
        }

        public void AddStartupNotepad()
        {
            Notepad?.AddStartupItem();
        }

        public void BeforeInitComponents()
        {
            Loaded += NotepadWindow_Loaded;
        }

        private void NotepadWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WindowShownCallback?.Invoke(this);
        }

        public void InitWindow()
        {
            Notepad = new NotepadViewModel();
            Notepad.HightlightTextCallback = Hightlight;
            Notepad.AnimateAddCallback = this.AnimateControl;
            Notepad.FocusFindInputCallback = FocusFindInputBox;
            Notepad.ScrollItemIntoView = ScrollItemIntoView;
            InitialiseTreeFileExplorer();
        }

        public void LoadSettings(bool loadTheme = true, bool loadPosition = true)
        {
            PreferencesG.LoadFromProperties();
            if (loadPosition)
            {
                this.Top = Properties.Settings.Default.Top;
                this.Left = Properties.Settings.Default.Left;
            }
            this.Height = Properties.Settings.Default.Height;
            this.Width = Properties.Settings.Default.Width;
            showLineThing.IsChecked = Properties.Settings.Default.allowCaretLineOutline;
            if (loadTheme)
            {
                switch (Properties.Settings.Default.Theme)
                {
                    case 1: SetTheme(ThemeTypes.Light); break;
                    case 2: SetTheme(ThemeTypes.Dark); break;
                    case 3: SetTheme(ThemeTypes.ColourfulLight); break;
                    case 4: SetTheme(ThemeTypes.ColourfulDark); break;
                }
            }
        }

        public void SetTheme(ThemeTypes theme)
        {
            ThemesController.SetTheme(theme);
        }

        public void AnimateControl(NotepadListItem nli, AnimationFlag af)
        {
            switch (af)
            {
                case AnimationFlag.NotepadItemOPEN:
                    AnimationLib.OpacityControl(nli, 0, 1, GlobalPreferences.ANIMATION_SPEED_SEC);
                    AnimationLib.MoveToTargetX(nli, 0, -ActualWidth, GlobalPreferences.ANIMATION_SPEED_SEC);
                    break;

                //Cant really do this. Animations are async so i'd have to have a timed delay when
                //removing the item from the NotepadList, which would be... a bit complex.
                //If anyone wants to try and add that delay (using tasks maybe, have a go)
                //(and msg me or something with the code, and ill add it)
                case AnimationFlag.NotepadItemCLOSE:
                    //AnimationLib.OpacityControl(nli, 1, 0, AnimationSpeedSeconds);
                    //AnimationLib.MoveToTargetX(nli, -ActualWidth, 0, AnimationSpeedSeconds * 15);
                    //
                    //Task.Run(async () =>
                    //{
                    //    await Task.Delay(TimeSpan.FromSeconds(AnimationSpeedSeconds));
                    //    await Application.Current.Dispatcher.InvokeAsync(() => { Notepad.NotepadItems.Remove(nli); });
                    //});
                    break;
            }
        }

        public void NotepadListItemDropped(DragEventArgs e)
        {
            if (Notepad != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if (e.Data.GetData(DataFormats.FileDrop) is string[] droppedItemArray)
                {
                    foreach (string path in droppedItemArray)
                    {
                        string text = File.ReadAllText(path);
                        Notepad.AddNotepadItem(
                            Notepad.CreateDefaultStyleNotepadItem(
                                text,
                                Path.GetFileName(path),
                                path));
                    }
                }
            }
        }

        public void FocusFindInputBox(bool focusFind)
        {
            if (focusFind)
                findInputBox.Focus();
            else
                MainTextBox.Focus();
        }

        public void ScrollItemIntoView()
        {
            if (notepadLstBox.SelectedItem != null)
                notepadLstBox.ScrollIntoView(notepadLstBox.SelectedItem);
        }

        /// <summary>
        /// Hightlight text using a <see cref="FindResult"/>'s Start/length
        /// </summary>
        /// <param name="result"></param>
        public void Hightlight(FindResult result)
        {
            try
            {
                if (findList.ItemsSource is ObservableCollection<FindResultItem> items)
                {
                    if (findList.SelectedItem is FindResultItem fri)
                    {
                        int itemIndex = findList.SelectedIndex;
                        items.Remove(fri);
                        items.Insert(itemIndex, fri);
                        MainTextBox.HighlightSearchResult(result);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to highlight text: {e.Message}");
            }
        }

        private void InitialiseTreeFileExplorer()
        {
            try
            {
                DriveInfo[] drives = DriveInfo.GetDrives();
                DriveInfo.GetDrives().ToList().ForEach(drive =>
                {
                    FileSystemObjectInfo fileSystemObject = new FileSystemObjectInfo(drive);
                    fileSystemObject.BeforeExplore += FileSystemObject_BeforeExplore;
                    fileSystemObject.AfterExplore += FileSystemObject_AfterExplore;
                    treeView.Items.Add(fileSystemObject);
                });
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to initialise File Explorer. Error: {e.Message}");
            }
        }

        private void FileSystemObject_AfterExplore(object sender, System.EventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        private void FileSystemObject_BeforeExplore(object sender, System.EventArgs e)
        {
            Cursor = Cursors.Wait;
        }

        //could put in mainviewmodel but eh
        private void ChangeTheme(object sender, RoutedEventArgs e)
        {
            switch (int.Parse(((MenuItem)sender).Uid))
            {
                case 0: ThemesController.SetTheme(ThemeTypes.Light); break;
                case 1: ThemesController.SetTheme(ThemeTypes.ColourfulLight); break;
                case 2: ThemesController.SetTheme(ThemeTypes.Dark); break;
                case 3: ThemesController.SetTheme(ThemeTypes.ColourfulDark); break;
            }
            //idk what this actually does .-.
            e.Handled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Notepad.CheckHasMadeChanges())
            {
                MessageBoxResult mbr = MessageBox.Show(
                    "You have unsaved work. Do you want to save it/them?",
                    "Unsaved Work",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Information);

                if (mbr == MessageBoxResult.Yes)
                    Notepad.SaveAllNotepadItems();
                if (mbr == MessageBoxResult.Cancel)
                    e.Cancel = true;
                return;
            }
            if (CanSavePreferences)
            {
                try
                {
                    PreferencesG.SaveToProperties();
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
                    switch (ThemesController.CurrentTheme)
                    {
                        case ThemeTypes.Light: Properties.Settings.Default.Theme = 1; break;
                        case ThemeTypes.Dark: Properties.Settings.Default.Theme = 2; break;
                        case ThemeTypes.ColourfulLight: Properties.Settings.Default.Theme = 3; break;
                        case ThemeTypes.ColourfulDark: Properties.Settings.Default.Theme = 4; break;
                    }
                    if (!this.Notepad.CheckNotepadNull())
                    {
                        if (Notepad.Notepad.DocumentFormat != null)
                        {
                            Properties.Settings.Default.DefaultFont = this.Notepad.Notepad.DocumentFormat.Family.ToString();
                            Properties.Settings.Default.DefaultFontSize = this.Notepad.Notepad.DocumentFormat.Size;
                        }
                    }
                    Properties.Settings.Default.allowCaretLineOutline = showLineThing.IsChecked ?? true;
                    Properties.Settings.Default.Save();
                }
                catch { }
                Notepad?.ShutdownInformationHook();
            }

            WindowClosedCallback?.Invoke(this);
        }

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            NotepadListItemDropped(e);
        }


        private void TreeViewItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                MouseDownPoint = e.GetPosition(null);
        }

        private void TreeViewItem_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseDownPoint != e.GetPosition(null) && e.LeftButton == MouseButtonState.Pressed)
            {
                if (sender is Rectangle grip && grip.Parent is StackPanel fileItem)
                {
                    if (fileItem.DataContext is FileSystemObjectInfo file)
                    {
                        try
                        {
                            if (File.Exists(file.FileSystemInfo.FullName))
                            {
                                string[] path1 = new string[1] { file.FileSystemInfo.FullName };
                                DragDrop.DoDragDrop(
                                    this,
                                    new DataObject(
                                        DataFormats.FileDrop,
                                        path1),
                                    DragDropEffects.Copy);
                            }
                        }
                        catch { }
                    }
                }
            }
        }


        public void DrawRectangleAtCaret()
        {
            if (MainTextBox != null && showLineThing.IsChecked == true)
            {
                Rect p = MainTextBox.GetCaretLocation();
                Thickness t = new Thickness(0, p.Y - 1, 17, MainTextBox.ActualHeight - p.Bottom - 1); ;
                if (t.Top >= 0 && t.Bottom >= 0)
                {
                    aditionalSelection.Visibility = Visibility.Visible;
                    aditionalSelection.Margin = t;
                }
                else
                    aditionalSelection.Visibility = Visibility.Collapsed;
            }
            else
                aditionalSelection.Visibility = Visibility.Collapsed;
        }



        private void TextBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            DrawRectangleAtCaret();
            if (PreferencesG.CAN_ZOOM_EDITOR_CTRL_MWHEEL)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    int fontChange = e.Delta / 100;
                    if (Notepad.Notepad.DocumentFormat.Size > 1)
                        Notepad.Notepad.DocumentFormat.Size += fontChange;
                    if (Notepad.Notepad.DocumentFormat.Size == 1 && fontChange >= 1)
                        Notepad.Notepad.DocumentFormat.Size += fontChange;
                    e.Handled = true;
                }
            }
        }

        private void MainTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            DrawRectangleAtCaret();
        }

        private void MainTextBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawRectangleAtCaret();
        }

        private void ShowLineThing_Checked(object sender, RoutedEventArgs e)
        {
            DrawRectangleAtCaret();
        }

        private void ChangeResolutionClick(object sender, RoutedEventArgs e)
        {
            switch (int.Parse(((FrameworkElement)sender).Uid))
            {
                case 0: Width = 1024; Height = 576 + GlobalPreferences.WINDOW_TITLEBAR_HEIGHT; break;
                case 1: Width = 1152; Height = 648 + GlobalPreferences.WINDOW_TITLEBAR_HEIGHT; break;
                case 2: Width = 1280; Height = 720 + GlobalPreferences.WINDOW_TITLEBAR_HEIGHT; break;
                case 3: Width = 1706; Height = 960 + GlobalPreferences.WINDOW_TITLEBAR_HEIGHT; break;
                case 4: Width = 1096; Height = 664; break;
            }
        }

        // this is kinda useless tbh but oh well
        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Notepad.OurClipboard.ShowClipboardWindow();
        }

        private void OpenWindowPreviews(object sender, RoutedEventArgs e)
        {
            ThisApplication.ShowWindowPreviewsWindow();
        }
    }
}
