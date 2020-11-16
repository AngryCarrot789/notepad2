﻿using Microsoft.Win32;
using Notepad2.Applications;
using Notepad2.CClipboard;
using Notepad2.FileChangeWatcher;
using Notepad2.FileExplorer;
using Notepad2.Finding.NotepadItemFinding;
using Notepad2.Finding.TextFinding;
using Notepad2.History;
using Notepad2.InformationStuff;
using Notepad2.Interfaces;
using Notepad2.Notepad;
using Notepad2.Preferences;
using Notepad2.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;


namespace Notepad2.ViewModels
{
    public class NotepadViewModel : BaseViewModel
    {
        #region Private Fields

        private TextDocumentViewModel _notepad;
        private NotepadItemViewModel _selectedNotepadItem;
        private FindReplaceViewModel _find;
        private Visibility _findViewVisible;
        private int _selectedIndex;
        private int _line;
        private int _column;
        private bool _notepadAvaliable;
        private bool _topTabsExpanded;
        private bool _leftTabsExpanded;
        private string _toFindItemText;
        private bool _fileWatcherEnabled;

        #endregion

        #region Public Properties

        /// <summary>
        /// Holds a list of <see cref="NotepadItemViewModel"/>, for use in a ListBox
        /// (the one on the left side of the program)
        /// <para>
        /// Call this, "the list on the left"
        /// </para>
        /// </summary>
        public ObservableCollection<NotepadItemViewModel> NotepadItems { get; set; }

        public Visibility FindViewVisibility
        {
            get => _findViewVisible;
            set => RaisePropertyChanged(ref _findViewVisible, value);
        }

        /// <summary>
        /// The selected index of the currently selected <see cref="NotepadItemViewModel"/>
        /// </summary>
        public int SelectedIndex
        {
            get => _selectedIndex;
            set => RaisePropertyChanged(ref _selectedIndex, value);
        }

        /// <summary>
        /// The line which the caret is at
        /// </summary>
        public int Line
        {
            get => _line;
            set => RaisePropertyChanged(ref _line, value);
        }

        /// <summary>
        /// The position of the caret within the current line
        /// </summary>
        public int Column
        {
            get => _column;
            set => RaisePropertyChanged(ref _column, value);
        }

        /// <summary>
        /// The currently selected <see cref="NotepadItemViewModel"/>
        /// </summary>
        public NotepadItemViewModel SelectedNotepadItem
        {
            get => _selectedNotepadItem;
            set => RaisePropertyChanged(ref _selectedNotepadItem, value, () =>
            {
                if (value?.Notepad != null)
                {
                    Notepad = value.Notepad;
                    UpdateAndOpenSelectedNotepad();
                }
            });
        }

        /// <summary>
        /// Says whether a notepad is avaliable (aka, if there's any 
        /// <see cref="NotepadListItem"/>s in the list on the left)
        /// </summary>
        public bool NotepadAvaliable
        {
            get => _notepadAvaliable;
            set => RaisePropertyChanged(ref _notepadAvaliable, value);
        }

        /// <summary>
        /// Says whether the Top Notepad Item Tabs panel is expanded or not
        /// </summary>
        public bool TopTabsExpanded
        {
            get => _topTabsExpanded;
            set => RaisePropertyChanged(ref _topTabsExpanded, value, AnimateTopNotepadsList);
        }

        private void AnimateTopNotepadsList()
        {
            View?.ShowOrHideTopNotepadsList(TopTabsExpanded);
        }

        /// <summary>
        /// Says whether the Left Notepad Item Tabs panel is expanded or not
        /// </summary>
        public bool LeftTabsExpanded
        {
            get => _leftTabsExpanded;
            set => RaisePropertyChanged(ref _leftTabsExpanded, value, AnimateLeftNotepadsList);
        }

        private void AnimateLeftNotepadsList()
        {
            View?.ShowOrHideNotepadsList(LeftTabsExpanded);
        }

        /// <summary>
        /// Text used for finding Notepad Items
        /// </summary>
        public string ToFindItemText
        {
            get => _toFindItemText;
            set => RaisePropertyChanged(ref _toFindItemText, value);
        }

        public bool FileWatcherEnabled
        {
            get => _fileWatcherEnabled;
            set => RaisePropertyChanged(ref _fileWatcherEnabled, value, UpdateFileWatcher);
        }

        private void UpdateFileWatcher(bool fileWatcherEnabled)
        {
            GlobalPreferences.ENABLE_FILE_WATCHER = fileWatcherEnabled;
        }

        /// <summary>
        /// The currently selected <see cref="TextDocumentViewModel"/>
        /// </summary>
        public TextDocumentViewModel Notepad
        {
            get => _notepad;
            set => RaisePropertyChanged(ref _notepad, value);
        }

        /// <summary>
        /// A ViewModel for the Finding Panel for finding text within the selected Notepad
        /// </summary>
        public FindReplaceViewModel Find
        {
            get => _find;
            set => RaisePropertyChanged(ref _find, value, UpdateFindContext);
        }

        private void UpdateFindContext()
        {

        }

        public InformationViewModel InformationView { get; set; }

        //unexpected communism
        /// <summary>
        /// A ViewModel used for binding to the clipboard
        /// </summary>
        public ClipboardViewModel OurClipboard { get; }

        /// <summary>
        /// A ViewModel for dealing with the history of recently closed files.
        /// </summary>
        public HistoryViewModel History { get; set; }

        public ItemSearchResultsViewMode ItemSearcher { get; }

        /// <summary>
        /// An interface for providing a few functions to be executed on the notepad window,
        /// without having an instance to the window. So this is MVVM friendly i think
        /// </summary>
        public IMainView View { get; set; }

        #endregion

        #region Commands

        public ICommand NewCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand OpenDirectoryCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand SaveAsCommand { get; }
        public ICommand SaveAllCommand { get; }
        public ICommand CloseSelectedNotepadCommand { get; }
        public ICommand CloseAllNotepadsCommand { get; }
        public ICommand FocusAndRenameFileCommand { get; }
        public ICommand MoveItemCommand { get; }
        public ICommand PrintFileCommand { get; }
        public ICommand ShowFindWindowCommand { get; }
        public ICommand FindNotepadItemCommand { get; }
        public ICommand RefreshFileContentsCommand { get; }
        public ICommand AutoShowTopTabsCommand { get; }
        public ICommand AutoShowLeftTabsCommand { get; }
        public ICommand ShowHelpCommand { get; }

        public ICommand NewWindowCommand { get; }
        public ICommand OpenSelectedInNewWindowCommand { get; }
        public ICommand ReopenLastWindowCommand { get; }
        public ICommand CloseWindowCommand { get; }
        public ICommand CloseViewWithCheckCommand { get; }
        public ICommand CloseAllWindowsCommand { get; }

        public ICommand ShowWindowManagerCommand { get; }
        public ICommand ShowPreferencesViewCommand { get; }

        public ICommand SortListCommand { get; }

        #endregion

        #region Constructor

        public NotepadViewModel() : this(null) { }

        public NotepadViewModel(IMainView view)
        {
            View = view;

            History = new HistoryViewModel();
            OurClipboard = new ClipboardViewModel();
            InformationView = new InformationViewModel();
            NotepadItems = new ObservableCollection<NotepadItemViewModel>();
            ItemSearcher = new ItemSearchResultsViewMode();

            SetupInformationHook();
            History.OpenFileCallback = ReopenNotepadFromHistory;
            ItemSearcher.SelectItemCallback = SelectItem;

            NewCommand = new Command(AddDefaultNotepad);
            OpenCommand = new Command(OpenNotepadFromFileExplorer);
            OpenDirectoryCommand = new Command(OpenNotepadsFromDirectoryExplorer);
            SaveCommand = new Command(SaveSelectedNotepad);
            SaveAsCommand = new Command(SaveSelectedNotepadAs);
            SaveAllCommand = new Command(SaveAllNotepadDocuments);
            CloseSelectedNotepadCommand = new Command(CloseSelectedNotepad);
            CloseAllNotepadsCommand = new Command(CloseAllNotepads);
            FocusAndRenameFileCommand = new Command(FocusAndRenameSelectedFile);
            MoveItemCommand = new CommandParam<string>(MoveItem);
            PrintFileCommand = new Command(PrintFile);
            ShowFindWindowCommand = new Command(ShowFindView);
            FindNotepadItemCommand = new Command(FindNotepadItem);
            RefreshFileContentsCommand = new Command(RefreshAllFilesContents);
            AutoShowTopTabsCommand = new Command(AutoShowTopTabs);
            AutoShowLeftTabsCommand = new Command(AutoShowLeftTabs);

            NewWindowCommand = new Command(NewView);
            OpenSelectedInNewWindowCommand = new Command(OpenSelectedNotepadInAnotherWindow);
            ReopenLastWindowCommand = new Command(ReopenLastView);
            CloseWindowCommand = new Command(CloseView);
            CloseViewWithCheckCommand = new Command(CloseViewWithFindViewCheck);
            CloseAllWindowsCommand = new Command(ShutdownApplication);

            ShowWindowManagerCommand = new Command(ThisApplication.ShowWindowManager);
            ShowPreferencesViewCommand = new Command(ThisApplication.ShowPreferencesWindow);
            ShowHelpCommand = new Command(ThisApplication.ShowHelp);

            SortListCommand = new CommandParam<string>(SortItems);

            ShowHideFindView(false);

            FileWatcherEnabled = true;

            Information.Show("Notepad Window loaded", InfoTypes.Information);
        }

        #endregion

        #region Sorting

        /// <summary>
        /// Sorts all of the notepad documents/items in a certain way
        /// </summary>
        /// <param name="sortBy">fn to sort by file name, or fs to sort by the file size</param>
        public void SortItems(string sortBy)
        {
            switch (sortBy.ToString())
            {
                case "fn":
                    {
                        List<NotepadItemViewModel> sortedNames = NotepadItems.OrderBy(a => a.Notepad.Document.FileName).ToList();
                        int index = SelectedIndex;
                        NotepadItems.Clear();
                        foreach (NotepadItemViewModel item in sortedNames)
                        {
                            NotepadItems.Add(item);
                        }
                        SelectedIndex = index;
                    }
                    break;
                case "fs":
                    {
                        List<NotepadItemViewModel> sortedSizes = NotepadItems.OrderBy(a => -a.Notepad.Document.FileSizeBytes).ToList();
                        int index = SelectedIndex;
                        NotepadItems.Clear();
                        foreach (NotepadItemViewModel item in sortedSizes)
                        {
                            NotepadItems.Add(item);
                        }
                        SelectedIndex = index;
                    }
                    break;
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Returns whether the currently selected 
        /// <see cref="TextDocumentViewModel"/> is null
        /// </summary>
        /// <returns>a bool. xdddd</returns>
        public bool IsNotepadNull()
        {
            return Notepad == null || Notepad.Document == null || Notepad.DocumentFormat == null;
        }

        /// <summary>
        /// Prints (on A4 Paper) a page to your printer. Shows a dialog to specify settings (untested)
        /// </summary>
        public void PrintFile()
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument flowDocument = new FlowDocument
                {
                    PagePadding = new Thickness(50)
                };

                flowDocument.Blocks.Add(new Paragraph(new Run(Notepad.Document.Text)));
                printDialog.PrintDocument(((IDocumentPaginatorSource)flowDocument).DocumentPaginator, "Printed from SharpPad");
            }
        }

        /// <summary>
        /// Returns whether any Notepad Items have been edited or not
        /// </summary>
        /// <returns></returns>
        public bool HasAnyNotepadMadeChanges()
        {
            foreach (NotepadItemViewModel nli in NotepadItems)
            {
                if (nli != null && nli.Notepad.HasMadeChanges)
                    return true;
            }
            return false;
        }

        private void RefreshAllFilesContents()
        {
            foreach (NotepadItemViewModel nli in NotepadItems)
            {
                nli.RefreshContents();
            }
        }

        private void AutoShowTopTabs()
        {
            TopTabsExpanded = !TopTabsExpanded;
        }

        private void AutoShowLeftTabs()
        {
            LeftTabsExpanded = !LeftTabsExpanded;
        }

        #endregion

        // Notepad Functions

        #region Adding Notepads

        /// <summary>
        /// Adds a new default Notepad Item
        /// </summary>
        public void AddDefaultNotepad()
        {
            AddNotepadItem(CreateNotepadItem(CreateDefaultStyleNotepadDocument("", $"new{NotepadItems.Count}.txt", "")), true);
        }

        /// <summary>
        /// Adds a <see cref="TextDocumentViewModel"/> to the list on the left.
        /// </summary>
        /// <param name="nli">A <see cref="TextDocumentViewModel"/> to be added</param>
        public void AddNotepadItem(NotepadItemViewModel nli, bool selectItem = false, bool hasSavedStatus = false)
        {
            if (nli != null)
            {
                TextDocumentViewModel doc = nli.Notepad;
                if (doc == null)
                    return;

                doc.HasMadeChanges = hasSavedStatus;
                doc.Watcher.StartWatching();
                NotepadItems.Add(nli);
                if (selectItem)
                    SelectedNotepadItem = nli;
                Information.Show($"Added FileItem: {doc.Document.FileName}", InfoTypes.FileIO);
            }
        }

        /// <summary>
        /// Adds a normal notepad item, and then selects it 
        /// (for simplicity and quick access... maybe)
        /// </summary>
        public void AddStartupItem()
        {
            AddNotepadItem(CreateNotepadItem(CreateDefaultStyleNotepadDocument("", $"new{NotepadItems.Count}.txt", "")), true);
        }

        public void AddNotepadFromViewModel(NotepadItemViewModel notepad, bool selectItem = true)
        {
            SetupNotepadItemCallbacks(notepad);
            AddNotepadItem(notepad);
            if (selectItem)
                SelectedNotepadItem = notepad;
        }

        #endregion

        #region Closing Notepads

        /// <summary>
        /// if the given <see cref="NotepadItemViewModel"/> is inside 
        /// the list on the left, this will remove it.
        /// </summary>
        /// <param name="nli">The <see cref="NotepadItemViewModel"/> to be removed</param>
        public void CloseNotepadItem(NotepadItemViewModel nli)
        {
            CloseNotepadItem(nli, true);
        }

        /// <summary>
        /// if the given <see cref="NotepadItemViewModel"/> is inside 
        /// the list on the left, this will remove it.
        /// </summary>
        /// <param name="nli">The <see cref="NotepadItemViewModel"/> to be removed</param>
        public void CloseNotepadItem(NotepadItemViewModel nli, bool checkHasSavedFile = true)
        {
            if (nli != null)
            {
                TextDocumentViewModel doc = nli.Notepad;
                if (doc == null)
                    return;

                // dont want to do this anymore. press Ctrl + R to reopen a file that 
                // might have been accidentally closed
                //if (checkHasSavedFile && doc.HasMadeChanges)
                //{
                //    if (MessageBox.Show(
                //        "You have unsaved work. Do you want to save it/them?",
                //        "Unsaved Work",
                //        MessageBoxButton.YesNo,
                //        MessageBoxImage.Information,
                //        MessageBoxResult.No) == MessageBoxResult.Yes)
                //    {
                //        SaveNotepad(doc);
                //    }
                //}

                //AnimateAddCallback?.Invoke(nli, AnimationFlag.NotepadItemCLOSE);
                RemoveNotepadItem(nli);
                History.PushFile(nli);
                Information.Show($"Removed FileItem: [{doc.Document.FileName}]", InfoTypes.FileIO);
                SelectedIndex = 0;
                UpdateAndOpenSelectedNotepad();
            }
        }

        // NotepadListItems would've had a shutdown method for
        // getting rid of static handlers or other things...
        public void RemoveNotepadItem(NotepadItemViewModel nli)
        {
            //nli?.Shutdown();
            nli.Notepad.Watcher.FullyStopWatching();
            NotepadItems.Remove(nli);
        }

        /// <summary>
        /// If a Notepad Item is selected, this will close it.
        /// </summary>
        private void CloseSelectedNotepad()
        {
            if (SelectedNotepadItem != null)
            {
                CloseNotepadItem(SelectedNotepadItem);
            }
        }

        /// <summary>
        /// Closes every single Notepad Item inside the list on the left
        /// </summary>
        private void CloseAllNotepads()
        {
            RemoveAllNotepadItems();
            Notepad = null;
        }

        private void RemoveAllNotepadItems()
        {
            List<NotepadItemViewModel> items = NotepadItems.ToList();
            foreach (NotepadItemViewModel notepad in items)
            {
                RemoveNotepadItem(notepad);
                History.PushFile(notepad);
            }
            Information.Show($"Cleared {items.Count} Notepad Items", "Items");
        }

        #endregion

        #region Create

        /// <summary>
        /// Gets the font from the properties. this is automatically set as the last notepad 
        /// window's last notepad document's font when the application closes (i think lol)
        /// </summary>
        /// <returns></returns>
        public FontFamily GetDefaultFont()
        {
            return !Properties.Settings.Default.DefaultFont.IsEmpty()
                    ? new FontFamily(Properties.Settings.Default.DefaultFont)
                    : new FontFamily("Consolas");
        }

        /// <summary>
        /// Gets the font size from properties. this is automatically set as the last notepad 
        /// window's last notepad document's font when the application closes (i think lol)
        /// </summary>
        /// <returns></returns>
        public double GetDefaultFontSize()
        {
            double fSize = Properties.Settings.Default.DefaultFontSize;
            return fSize > 0 ? fSize : 14;
        }

        /// <summary>
        /// Creates and returns a completely default <see cref="TextDocumentViewModel"/>, with preset fonts,
        /// fontsizes, etc.
        /// </summary>
        /// <param name="text">The text the Notepad Item will have</param>
        /// <param name="itemName">The header/file name of the Notepad Item</param>
        /// <param name="itemPath">The file path of the Notepad Item</param>
        /// <returns></returns>
        public TextDocumentViewModel CreateDefaultStyleNotepadDocument(string text, string itemName, string itemPath)
        {
            FormatViewModel fm = new FormatViewModel()
            {
                Family = GetDefaultFont(),
                Size = GetDefaultFontSize(),
                IsWrapped = PreferencesG.WRAP_TEXT_BY_DEFAULT
            };

            return CreateNotepadDocument(text, itemName, itemPath, fm);
        }

        /// <summary>
        /// Creates a <see cref="TextDocumentViewModel"/> with the given text, name, path, text formats and preset callbacks
        /// </summary>
        /// <param name="text">The text the Notepad Item will have</param>
        /// <param name="fileName">The header/name of the Notepad Item</param>
        /// <param name="filePath">The file path of the Notepad Item</param>
        /// <param name="fm">The styles the Notepad Item will have</param>
        /// <returns></returns>
        public TextDocumentViewModel CreateNotepadDocument(string text, string fileName, string filePath, FormatViewModel fm)
        {
            TextDocumentViewModel fivm = new TextDocumentViewModel();
            fivm.Document.Text = text;
            fivm.Document.FileName = fileName;
            fivm.Document.FilePath = filePath;
            fivm.DocumentFormat = fm;

            fivm.FindResults.HighlightResultCallback = HighlightTextFromSearch;
            fivm.FindResults.ReplaceTextCallback = ReplaceText;
            fivm.FindResults.SetFindViewIsVisibleCallback = ShowHideFindView;
            fivm.HasMadeChanges = false;

            return fivm;
        }

        public NotepadItemViewModel CreateNotepadItem(TextDocumentViewModel doc)
        {
            NotepadItemViewModel item = new NotepadItemViewModel();
            item.Notepad = doc;
            SetupNotepadItemCallbacks(item);

            return item;
        }

        /// kettlesimulator made this xd
        /// <summary>
        /// Sets up the callback functions for a notepad document 
        /// (e.g, the ones to close the item or open in another window).
        /// </summary>
        /// <param name="nli"></param>
        public void SetupNotepadItemCallbacks(NotepadItemViewModel nli)
        {
            nli.RemoveNotepadCallback = CloseNotepadItem;
            nli.OpenInNewWindowCallback = OpenNotepadDocumentInNewView;
        }

        #endregion

        #region Other functions

        /// <summary>
        /// Sets the selected <see cref="NotepadItemViewModel"/> using 
        /// the selected <see cref="NotepadItemViewModel"/>, and also other stuff
        /// </summary>
        private void UpdateAndOpenSelectedNotepad()
        {
            if (SelectedNotepadItem != null)
            {
                OpenNotepadDocument(SelectedNotepadItem.Notepad);
                Find = SelectedNotepadItem.Notepad.FindResults;
            }
            UpdateNotepadAvaliability();
        }

        /// <summary>
        /// Updates stuff
        /// </summary>
        public void UpdateNotepadAvaliability()
        {
            NotepadAvaliable = NotepadItems.Count > 0;
            if (NotepadItems.Count == 0)
                Notepad = null;
        }

        public void FocusAndRenameSelectedFile()
        {
            if (NotepadAvaliable)
                SelectedNotepadItem?.HighlightFileName();
        }

        #endregion

        // File IO

        #region Opening

        /// <summary>
        /// Sets the View's document as the given one
        /// </summary>
        /// <param name="notepadItem"></param>
        public void OpenNotepadDocument(TextDocumentViewModel notepadItem)
        {
            if (notepadItem != null)
            {
                Notepad = notepadItem;
            }
        }

        /// <summary>
        /// Opens a notepad file from file explorer
        /// </summary>
        public void OpenNotepadFromFileExplorer()
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Title = "Select Files to open",
                    Multiselect = true
                };

                if (ofd.ShowDialog() == true)
                {
                    int i;
                    for (i = 0; i < ofd.FileNames.Length; i++)
                    {
                        string paths = ofd.FileNames[i];
                        OpenNotepadFromPath(paths, true);
                    }
                    Information.Show($"Opened {i} files", InfoTypes.FileIO);
                }
            }
            catch (Exception e) { Information.Show(e.Message, "Error while opening file from f.explorer"); }
        }

        /// <summary>
        /// Opens every notepad file in a folder
        /// </summary>
        public void OpenNotepadsFromDirectoryExplorer()
        {
            System.Windows.Forms.FolderBrowserDialog ofd =
                new System.Windows.Forms.FolderBrowserDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                int i;
                if (ofd.SelectedPath.IsDirectory())
                {
                    try
                    {
                        string[] files = Directory.GetFiles(ofd.SelectedPath);
                        for (i = 0; i < files.Length; i++)
                        {
                            string file = files[i];
                            OpenNotepadFromPath(file);
                        }
                        Information.Show($"Opened {i} files", InfoTypes.FileIO);
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

        /// <summary>
        /// Reads the text from a file path and creates and adds a new notepad document
        /// </summary>
        /// <param name="path"></param>
        /// <param name="selectFile"></param>
        public void OpenNotepadFromPath(string path, bool selectFile = false, bool hasMadeChanges = false, bool clearPath = false)
        {
            try
            {
                if (File.Exists(path))
                {
                    FileInfo fInfo = new FileInfo(path);
                    if (fInfo.Length > GlobalPreferences.MAX_FILE_SIZE)
                    {
                        Information.Show("File size too big (75+ MB): Cannot open because a file this big would be extremely laggy");
                        return;
                    }

                    string text = NotepadActions.ReadFile(path);
                    NotepadItemViewModel item = CreateNotepadItem(CreateDefaultStyleNotepadDocument(text, Path.GetFileName(path), clearPath ? "" : path));
                    TextDocumentViewModel doc = item.Notepad;

                    AddNotepadItem(item, hasSavedStatus: hasMadeChanges);

                    doc.Document.IsReadOnly = fInfo.IsReadOnly;

                    Information.Show($"Opened file: [{path}]", InfoTypes.FileIO);
                    if (selectFile && doc != null)
                        SelectedNotepadItem = item;
                }
            }
            catch (Exception e) { Information.Show($"Error while opening file from path: {e.Message}", "File Open Error"); }
        }

        public void ReopenNotepadFromHistory(NotepadItemViewModel notepad)
        {
            if (notepad != null)
            {
                Information.Show($"Reopening [{notepad.Notepad?.Document?.FileName}] from history", "History");
                AddNotepadFromViewModel(notepad);
            }
        }

        #endregion

        #region Saving

        public void SaveNotepad(TextDocumentViewModel fivm, bool forceCreateFileAnyway = false)
        {
            try
            {
                if (forceCreateFileAnyway)
                {
                    try
                    {
                        Information.Show($"Force saving file in [{fivm.Document.FilePath}]", InfoTypes.FileIO);
                        if (SaveFile(fivm.Document.FilePath, fivm.Document.Text, fivm.Document.IsReadOnly))
                        {
                            fivm.HasMadeChanges = false;
                        }
                    }
                    catch (Exception exception)
                    {
                        Information.Show($"Error when saving file from non-existant but valid path: {exception.Message}", InfoTypes.FileIO);
                    }
                }
                else if (File.Exists(fivm.Document.FilePath))
                {
                    // Check if the path is the same but if the name has changed
                    string oldFileName = Path.GetFileName(fivm.Document.FilePath);
                    string extension = Path.GetExtension(fivm.Document.FilePath);
                    string folderName = Path.GetDirectoryName(fivm.Document.FilePath);
                    string newFileName =
                        Path.HasExtension(Path.Combine(folderName, fivm.Document.FileName))
                        ? fivm.Document.FileName
                        : fivm.Document.FileName + extension;
                    string newFilePath = Path.Combine(folderName, newFileName);
                    // if the name is different, then ask to override the file with a new name
                    if (fivm.Document.FilePath != newFilePath)
                    {
                        if (MessageBox.Show($"You are about to overwrite {oldFileName} " +
                            $"with {newFileName}, do you want to continue?", "Overrite file?",
                            MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            if (SaveFile(fivm.Document.FilePath, fivm.Document.Text, fivm.Document.IsReadOnly))
                            {
                                fivm.HasMadeChanges = false;
                            }

                            File.Move(fivm.Document.FilePath, newFilePath);
                            fivm.Document.FileName = newFileName;
                            fivm.Document.FilePath = newFilePath;

                            Information.Show($"Renamed [{oldFileName}] to [{newFileName}] successfully, and saved", "File IO");
                        }
                    }
                    else
                    {
                        if (SaveFile(fivm.Document.FilePath, fivm.Document.Text, fivm.Document.IsReadOnly))
                        {
                            fivm.HasMadeChanges = false;
                        }

                        fivm.Document.FileName = newFileName;
                        fivm.Document.FilePath = newFilePath;
                    }
                }
                else
                {
                    SaveNotepadAs(fivm);
                }
            }
            catch (Exception e)
            {
                Information.Show($"Error while saving a (manual) notepad item: {e.Message}", "FileIO Error");
            }
        }

        public void SaveNotepadAs(TextDocumentViewModel fivm)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter =
                        "Plain Text (.txt)|*.txt|" +
                        "Text..? ish (.text)|*.text|" +
                        "C# File (.cs)|*.cs|" +
                        "C File (.c)|*.c|" +
                        "C++ File (.cpp)|*.cpp|" +
                        "C/C++ Header File (.h)|*.h|" +
                        "XAML File (.xaml)|*.xaml|" +
                        "XML File (.xml)|*.xml|" +
                        "HTM File (.htm)|*.htm|" +
                        "HTML File (.html)|*.html|" +
                        "CSS File (.css)|*.css|" +
                        "JS File (.js)|*.js|" +
                        "EXE File (.exe)|*.exe|" +
                        "All files|*.*";
                sfd.Title = "Select Files to save";
                sfd.FileName = fivm.Document.FileName;
                sfd.FilterIndex = 1;
                sfd.RestoreDirectory = true;

                if (sfd.ShowDialog() == true)
                {
                    string newFilePath = sfd.FileName;
                    if (SaveFile(newFilePath, fivm.Document.Text, fivm.Document.IsReadOnly))
                    {
                        fivm.HasMadeChanges = false;
                    }

                    fivm.Document.FileName = Path.GetFileName(newFilePath);
                    fivm.Document.FilePath = newFilePath;
                }
            }
            catch (Exception e) { Information.Show(e.Message, "Error while saving (manual) notepaditem as..."); }
        }

        public void SaveSelectedNotepad()
        {
            if (NotepadAvaliable)
            {
                Information.Show($"Attempted to save [{Notepad.Document.FileName}]", InfoTypes.FileIO);
                try
                {
                    if (!IsNotepadNull())
                    {
                        if (Notepad.Document.FilePath.IsFile())
                        {
                            SaveNotepad(Notepad);
                        }
                        else
                        {
                            if (Notepad.Document.FilePath.CanNonExistantButPossiblePathExist())
                            {
                                if (MessageBox.Show(
                                    "File doesn't exist, but the notepad's path is valid. " +
                                    "Create and save this notepad to the path?",
                                    "File Doesn't exist, but could exist",
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Question,
                                    MessageBoxResult.Yes) == MessageBoxResult.Yes)
                                {
                                    try
                                    {
                                        SaveNotepad(Notepad, true);
                                    }
                                    catch (Exception exception)
                                    {
                                        Information.Show($"Error when saving file from non-existant but valid path: {exception.Message}", InfoTypes.FileIO);
                                    }
                                }
                            }
                            else
                            {
                                SaveNotepadAs(Notepad);
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    Information.Show(exception.Message, "Error while saving currently selected notepad");
                }
                UpdateAndOpenSelectedNotepad();
            }
            else
            {
                Information.Show($"Failed to save notepad: none exist", InfoTypes.FileIO);
            }
        }

        public void SaveSelectedNotepadAs()
        {
            try
            {
                if (!IsNotepadNull() && NotepadAvaliable)
                    SaveNotepadAs(Notepad);
            }
            catch (Exception e)
            {
                Information.Show(e.Message, "Error while saving currently selected notepad as...");
            }
        }

        public void SaveAllNotepadDocuments()
        {
            if (NotepadAvaliable)
            {
                foreach (NotepadItemViewModel nli in NotepadItems)
                {
                    if (nli?.Notepad != null)
                        SaveNotepad(nli.Notepad);
                }
            }
        }

        /// <summary>
        /// Saves the text to the given path
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <param name="text">The text to be saved to the file</param>
        /// <returns></returns>
        public bool SaveFile(string path, string text, bool isReadOnly = false)
        {
            try
            {
                // Check if the file is read only
                if (path.IsFile())
                {
                    FileInfo fInfo = new FileInfo(path);
                    if (!fInfo.IsReadOnly)
                    {
                        NotepadActions.SaveFile(path, text, isReadOnly);
                        Information.Show($"Successfully saved [{path}]", InfoTypes.FileIO);
                        return true;
                    }
                    else
                    {
                        Information.Show($"File [{path}] is read only. Cannot save.", "FileIO Error");
                        return false;
                    }
                }
                else
                {
                    NotepadActions.SaveFile(path, text, isReadOnly);
                    Information.Show($"Successfully saved [{path}]", InfoTypes.FileIO);
                    return true;
                }
            }
            catch (Exception e)
            {
                Information.Show($"Error while saving text to file: {e.Message}", "FileIO Error");
                return false;
            }
        }

        #endregion

        // Other

        #region Finding

        private void ShowHideFindView(bool show)
        {
            if (show)
            {
                FindViewVisibility = Visibility.Visible;
                View.FocusFindInput();
            }

            else
            {
                FindViewVisibility = Visibility.Collapsed;
            }
        }

        public void ShowFindView()
        {
            ShowHideFindView(true);
        }

        public void HighlightText(FindResult result, bool focusTextEditor = true)
        {
            View.HighlightFindResult(result, focusTextEditor);
        }

        private void HighlightTextFromSearch(FindResult result, bool focusTextEditor = true)
        {
            HighlightText(result, focusTextEditor);
        }

        private void ReplaceText(FindResult result, string textToReplaceWith)
        {
            View.ReplaceEditorText(result, textToReplaceWith);
        }

        // Finding Items

        private void FindNotepadItem()
        {
            ItemSearcher.Search(NotepadItems, ToFindItemText);
            View.ShowItemsSearcherWindow();
        }

        public void SelectItem(NotepadItemViewModel doc)
        {
            if (doc != null && NotepadItems.Contains(doc))
            {
                SelectedNotepadItem = doc;
            }
        }

        #endregion

        #region Moving Items

        public void MoveItem(bool upOrDown)
        {
            if (SelectedNotepadItem != null)
            {
                if (upOrDown)
                    MoveSelectedItemUp();
                else
                    MoveSelectedItemDown();
            }
        }

        public void MoveItem(string direction)
        {
            if (SelectedNotepadItem != null)
            {
                if (direction.ToString() == "up")
                    MoveSelectedItemUp();

                if (direction.ToString() == "down")
                    MoveSelectedItemDown();
            }
        }

        public void MoveSelectedItemUp()
        {
            if (SelectedIndex > 0)
            {
                MoveControl(SelectedIndex, SelectedIndex - 1);
            }
        }

        public void MoveSelectedItemDown()
        {
            if (SelectedIndex + 1 < NotepadItems.Count)
            {
                MoveControl(SelectedIndex, SelectedIndex + 1);
            }
        }

        public void MoveControl(int oldIndex, int newIndex)
        {
            if (NotepadItems.Count > 0 &&
                oldIndex >= 0 &&
                oldIndex < NotepadItems.Count &&
                newIndex >= 0 &&
                newIndex < NotepadItems.Count)
            {
                NotepadItemViewModel item = NotepadItems[oldIndex];
                NotepadItems.Remove(item);
                NotepadItems.Insert(newIndex, item);
                SelectedIndex = newIndex;
            }
        }

        #endregion

        #region InfoStatusErrors

        private void Information_InformationAdded(InformationModel e)
        {
            InformationView.AddInformation(e);
        }

        // ther

        #endregion

        #region Views/Windows

        private void NewView()
        {
            ThisApplication.OpenNewWindow();
        }

        public void ReopenLastView()
        {
            if (PreferencesG.CAN_REOPEN_WIN_WITH_CTRL_SHIFT_T)
                ThisApplication.ReopenLastWindow();
        }

        public void CloseView()
        {
            if (PreferencesG.CAN_CLOSE_WIN_WITH_CTRL_W)
            {
                Shutdown();
                ThisApplication.CloseWindowFromDataContext(this);
            }
        }

        public void CloseViewWithFindViewCheck()
        {
            if (FindViewVisibility == Visibility.Visible)
            {
                ShowHideFindView(false);
            }
            else if (MessageBox.Show("Close Window?", "Close", MessageBoxButton.OKCancel, MessageBoxImage.Information, MessageBoxResult.OK) == MessageBoxResult.OK)
            {
                CloseView();
            }
        }

        public void ShutdownApplication()
        {
            if (PreferencesG.CAN_CLOSE_WIN_WITH_CTRL_W)
                ThisApplication.ShutdownApplication();
        }

        /// <summary>
        /// Opens a notepad document in a new window
        /// </summary>
        /// <param name="nli"></param>
        public void OpenNotepadDocumentInNewView(NotepadItemViewModel nli)
        {
            if (nli != null)
            {
                TextDocumentViewModel doc = nli.Notepad;

                if (doc == null)
                    return;

                if (File.Exists(doc.Document.FilePath))
                {
                    ThisApplication.OpenFileInNewWindow(doc.Document.FilePath, false, false);
                    Information.Show($"Opened [{doc.Document.FileName}] in another window", InfoTypes.Information);
                    CloseNotepadItem(nli, false);
                }
                else
                {
                    string tempFilePath = Path.Combine(Path.GetTempPath(), doc.Document.FileName);
                    File.WriteAllText(tempFilePath, doc.Document.Text);
                    ThisApplication.OpenFileInNewWindow(tempFilePath, true, false);
                    Information.Show($"Opened [{doc.Document.FileName}] in another window", InfoTypes.Information);
                    CloseNotepadItem(nli, false);
                    File.Delete(tempFilePath);
                }
            }
        }

        public void OpenSelectedNotepadInAnotherWindow()
        {
            if (SelectedNotepadItem != null)
            {
                OpenNotepadDocumentInNewView(SelectedNotepadItem);
            }
        }

        #endregion

        #region Hooks and Shutting down

        public void SetupInformationHook()
        {
            Information.InformationAdded += Information_InformationAdded;
        }

        /// <summary>
        /// Unattach the static event from the information thingy. i think
        /// this stops a memory leak occouring.
        /// </summary>
        public void ShutdownInformationHook() //idec this method name is bad xd
        {
            Information.InformationAdded -= Information_InformationAdded;
        }

        public void Shutdown()
        {
            ShutdownInformationHook();
            OurClipboard.ShutdownUpdaterHook();
        }

        #endregion
    }
}

// eee over 1000 lines xd