using Microsoft.Win32;
using NamespaceHere;
using Notepad2.InformationStuff;
using Notepad2.Notepad;
using Notepad2.Utilities;
using Notepad2.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Notepad2.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Private Fields

        private NotepadViewModel _notepad;
        private int _selectedIndex;
        private bool _normalTextBoxSelected;
        private int _textEditorsSelectedIndex;

        #endregion

        #region Public Fields

        public ObservableCollection<NotepadListItem> NotepadItems { get; set; }
        public ObservableCollection<InformationListItem> InfoStatusErrorsItems { get; set; }
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                RaisePropertyChanged(ref _selectedIndex, value);
                UpdateNotepad();
            }
        }
        public NotepadListItem SelectedNotepadItem
        {
            get
            {
                try { return NotepadItems[SelectedIndex]; }
                catch { return null; }
            }
        }
        public FileItemViewModel SelectedNotepadViewModel
        {
            get
            {
                try { return NotepadItems[SelectedIndex].DataContext as FileItemViewModel; }
                catch { return null; }
            }
        }
        public bool TextBoxSelected
        {
            get => _normalTextBoxSelected;
            set
            {
                RaisePropertyChanged(ref _normalTextBoxSelected, value);
            }
        }
        public int TextEditorsSelectedIndex
        {
            get => _textEditorsSelectedIndex;
            set
            {
                RaisePropertyChanged(ref _textEditorsSelectedIndex, value);
                if (value <= 0)
                    TextBoxSelected = true;
                else if (value > 0)
                    TextBoxSelected = false;
            }
        }

        #endregion

        public ICommand NewCommand { get; set; }
        public ICommand OpenCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand SaveAsCommand { get; set; }
        public ICommand SaveAllCommand { get; set; }
        public ICommand CloseSelectedNotepadCommand { get; set; }
        public ICommand CloseAllNotepadsCommand { get; set; }
        public ICommand OpenInNewWindowCommand { get; set; }
        public ICommand PrintFileCommand { get; set; }
        public ICommand ShowFindWindowCommand { get; set; }
        public ICommand ClearInfoItemsCommand { get; set; }

        // The ViewModel for the Notepad. This contains a DocumentModel and FormatModel, for holding
        // Styles, Text, FilePath, etc.
        public NotepadViewModel Notepad
        {
            get => _notepad;
            set => RaisePropertyChanged(ref _notepad, value);
        }

        // ViewModel for the help window (idk why it needs a view model...)
        public HelpViewModel Help { get; set; }
        public Action<NotepadListItem, AnimationFlag> AnimateAddCallback { get; set; }

        public MainViewModel()
        {
            Notepad = new NotepadViewModel();
            Help = new HelpViewModel();
            NotepadItems = new ObservableCollection<NotepadListItem>();
            TextEditorsSelectedIndex = 0;
            SetupCommands();

            KeydownManager.KeyDown += KeydownManager_KeyDown;
            InfoStatusErrorsItems = new ObservableCollection<InformationListItem>();
            Information.AddErrorCallback = AddInfoStatusInfoMessage;
            Information.Show("Program loaded", InfoTypes.Information);
        }

        private void AddInfoStatusInfoMessage(InformationModel em)
        {
            InfoStatusErrorsItems.Insert(0, new InformationListItem(em));
        }

        private void KeydownManager_KeyDown(Key key)
        {
            if (KeydownManager.CtrlPressed)
            {
                switch (key)
                {
                    case Key.N: SaveCurrentNotepad(); break;
                    case Key.O: OpenNotepadFileFromFileExplorer(); break;
                    case Key.S: SaveCurrentNotepad(); break;
                    case Key.F: OpenFindWindow(); break;
                }
                if (KeydownManager.ShiftPressed)
                {
                    switch (key)
                    {
                        case Key.S: SaveAllNotepadItems(); break;
                    }
                }
            }
        }

        public void SetupCommands()
        {
            NewCommand = new Command(NewNotepad);
            OpenCommand = new Command(OpenNotepadFileFromFileExplorer);
            SaveCommand = new Command(SaveCurrentNotepad);
            SaveAsCommand = new Command(SaveCurrentNotepadAs);
            SaveAllCommand = new Command(SaveAllNotepadItems);
            CloseSelectedNotepadCommand = new Command(CloseSelectedNotepad);
            CloseAllNotepadsCommand = new Command(CloseAllNotepads);
            OpenInNewWindowCommand = new Command(OpenInNewWindow);
            PrintFileCommand = new Command(PrintFile);
            ShowFindWindowCommand = new Command(OpenFindWindow);
            ClearInfoItemsCommand = new Command(ClearInfoItems);
        }

        private void OpenInNewWindow()
        {
            MainWindow mWnd = new MainWindow(SelectedNotepadItem, false);
            mWnd.Show();
            mWnd.LoadSettings();
            CloseSelectedNotepad();
        }

        public void ClearInfoItems()
        {
            InfoStatusErrorsItems.Clear();
        }

        #region Helpers

        /// <summary>
        /// Returns true if null
        /// </summary>
        /// <returns>a bool. xdddd</returns>
        public bool CheckNotepadNull()
        {
            return Notepad == null || Notepad.Document == null || Notepad.DocumentFormat == null;
        }

        public void PrintFile()
        {
            FileItemViewModel file = SelectedNotepadViewModel;
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument flowDocument = new FlowDocument();
                flowDocument.PagePadding = new Thickness(50);
                flowDocument.Blocks.Add(new Paragraph(new Run(file.Document.Text)));

                printDialog.PrintDocument((((IDocumentPaginatorSource)flowDocument).DocumentPaginator), "Using Paginator");
            }
        }

        public bool Shutdown()
        {
            foreach (NotepadListItem nli in NotepadItems)
                if (nli != null && nli.DataContext is FileItemViewModel fivm)
                    if (fivm != null && fivm.HasMadeChanges)
                        return true;
            return false;
        }

        #endregion

        #region RichTextBox

        public void RichTextChanged(string text)
        {
            SelectedNotepadViewModel.Document.Text = text;
        }

        #endregion

        #region Notepad Functions

        #region New and Adding notepads

        public void NewNotepad()
        {
            AddNotepadItem(CreateDefaultStyleNotepadItem("", $"new{NotepadItems.Count()}.txt", null));
        }

        public void AddNotepadItem(NotepadListItem nli)
        {
            NotepadItems.Add(nli);
            Information.Show($"Added FileItem: {nli.Notepad.Document.FileName}", InfoTypes.FileIO);
            AnimateAddCallback?.Invoke(nli, AnimationFlag.NotepadItemOPEN);
            UpdateNotepad();
        }

        public void CloseNotepadItem(NotepadListItem nli)
        {
            //AnimateAddCallback?.Invoke(nli, AnimationFlag.NotepadItemCLOSE);
            NotepadItems.Remove(nli);
            Information.Show($"Removed FileItem: {nli.Notepad.Document.FileName}", InfoTypes.FileIO);
            UpdateNotepad();
        }

        private void CloseSelectedNotepad()
        {
            CloseNotepadItem(SelectedNotepadItem);
        }

        private void CloseAllNotepads()
        {
            Information.Show($"Cleared {NotepadItems.Count} NotepadItems", InfoTypes.FileIO);
            NotepadItems.Clear();
            Notepad = new NotepadViewModel();
        }

        #endregion

        #region Other functions

        /// <summary>
        /// Sets the main notepad view to the selected notepad item. sometimes even if one is selected, one isnt 'set'. this sets it.
        /// </summary>
        private void UpdateNotepad()
        {
            OpenNotepadItem(SelectedNotepadItem);
        }

        #endregion

        #region Create

        public NotepadListItem CreateDefaultStyleNotepadItem(string text, string itemName, string itemPath)
        {
            FontFamily font = string.IsNullOrEmpty(Properties.Settings.Default.DefaultFont)
                ? new FontFamily("Consolas")
                : new FontFamily(Properties.Settings.Default.DefaultFont);
            double fontSize = 16; //default fontsize

            if (Properties.Settings.Default.DefaultFontSize > 0)
                fontSize = Properties.Settings.Default.DefaultFontSize;
            else
                fontSize = 16; //default fontsize

            FormatModel fm = new FormatModel()
            {
                Family = font,
                Size = fontSize,
                IsWrapped = true,
                EnableSpellCheck = false
            };
            return CreateNotepadItem(text, itemName, itemPath, fm);
        }

        public NotepadListItem CreateNotepadItem(string text, string itemName, string itemPath, FormatModel fm)
        {
            NotepadListItem nli = new NotepadListItem();
            FileItemViewModel fivm = new FileItemViewModel();
            fivm.Document.Text = text;
            fivm.Document.FileName = itemName;
            fivm.Document.FilePath = itemPath;

            fivm.DocumentFormat = fm;

            fivm.HasMadeChanges = false;

            nli.DataContext = fivm;

            nli.OpenInFileExplorer = this.OpenInFileExplorer;
            nli.Open = this.OpenNotepadItem;
            nli.Close = this.CloseNotepadItem;
            //fivm.DocumentFormat.Size = fm.Size;

            return nli;
        }

        #endregion

        #region File IO

        #region Opening

        public void OpenInFileExplorer(NotepadListItem nli)
        {
            if (nli != null && nli.DataContext is FileItemViewModel fivm)
            {
                if (fivm != null)
                {
                    string folderPath = fivm.Document.FilePath;
                    if (File.Exists(folderPath))
                    {
                        ProcessStartInfo info = new ProcessStartInfo()
                        {
                            FileName = "explorer.exe",
                            Arguments = string.Format("/e, /select, \"{0}\"", folderPath)
                        };
                        Information.Show("Opening File Explorer at selected file location", InfoTypes.FileIO);
                        Process.Start(info);
                    }

                    else
                    {
                        Information.Show("FilePath Doesn't Exist", "FilePath null");
                    }
                }
            }
        }

        public void OpenNotepadItem(NotepadListItem notepadItem)
        {
            if (notepadItem != null)
            {
                FileItemViewModel fivm = notepadItem.DataContext as FileItemViewModel;
                Notepad.Document = fivm.Document;
                Notepad.DocumentFormat = fivm.DocumentFormat;
            }
            else
            {
                Notepad.Document = null;
                Notepad.DocumentFormat = null;
            }
        }

        public void OpenNotepadFileFromFileExplorer()
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Select Files to open";
                ofd.Multiselect = true;
                if (ofd.ShowDialog() == true)
                {
                    int i;
                    for (i = 0; i < ofd.FileNames.Length; i++)
                    {
                        string paths = ofd.FileNames[i];
                        OpenNotepadFileFromPath(paths);
                        try { SelectedNotepadViewModel.HasMadeChanges = false; } catch { }
                    }
                    Information.Show($"Opened {i} files", InfoTypes.FileIO);
                }
            }
            catch (Exception e) { Information.Show(e.Message, "Error while opening file from f.explorer"); }
        }

        public void OpenNotepadFileFromPath(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    string text = NotepadActions.ReadFile(path);
                    AddNotepadItem(
                        CreateDefaultStyleNotepadItem(
                           text,
                           Path.GetFileName(path),
                           path));
                    Information.Show($"Opened file: {path}", InfoTypes.FileIO);
                }
            }
            catch (Exception e) { Information.Show(e.Message, "Error while opening file from path"); }
        }

        #endregion

        #region Saving

        public void SaveNotepad(FileItemViewModel fivm)
        {
            try
            {
                if (File.Exists(fivm.Document.FilePath))
                {
                    string extension = Path.GetExtension(fivm.Document.FilePath);
                    string folderName = Path.GetDirectoryName(fivm.Document.FilePath);
                    string newFileName =
                        Path.HasExtension(Path.Combine(folderName, fivm.Document.FileName))
                        ? fivm.Document.FileName
                        : fivm.Document.FileName + extension;
                    string newFilePath = Path.Combine(folderName, newFileName);
                    if (fivm.Document.FilePath != newFilePath)
                    {
                        if (MessageBox.Show($"You are about to overwrite {fivm.Document.FilePath} " +
                            $"with {newFilePath}, do you want to continue?", "Overrite file?",
                            MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            SaveFile(fivm.Document.FilePath, fivm.Document.Text);
                            File.Move(fivm.Document.FilePath, newFilePath);
                            fivm.Document.FileName = newFileName;
                            fivm.Document.FilePath = newFilePath;
                        }
                    }
                    else
                    {
                        SaveFile(fivm.Document.FilePath, fivm.Document.Text);
                        fivm.Document.FileName = newFileName;
                        fivm.Document.FilePath = newFilePath;
                    }
                }
                else
                {
                    SaveNotepadAs(fivm);
                }
            }
            catch (Exception e) { Information.Show(e.Message, "Error while saving a (manual) notepad item"); }
        }

        public void SaveNotepadAs(FileItemViewModel fivm)
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
                    SaveFile(newFilePath, fivm.Document.Text);
                    fivm.Document.FileName = Path.GetFileName(newFilePath);
                    fivm.Document.FilePath = newFilePath;
                }
            }
            catch (Exception e) { Information.Show(e.Message, "Error while saving (manual) notepaditem as..."); }
        }

        public void SaveCurrentNotepad()
        {
            Information.Show($"Attempted to save [{Notepad.Document.FileName}]", InfoTypes.FileIO);
            try
            {
                if (!CheckNotepadNull())
                {
                    if (File.Exists(Notepad.Document.FilePath))
                        SaveNotepad(new FileItemViewModel(Notepad));
                    else
                        SaveNotepadAs(new FileItemViewModel(Notepad));
                }
            }
            catch (Exception e) { Information.Show(e.Message, "Error while saving currently selected notepad"); }
            UpdateNotepad();
        }

        public void SaveCurrentNotepadAs()
        {
            try
            {
                if (!CheckNotepadNull())
                {
                    SaveNotepadAs(new FileItemViewModel(Notepad));
                }
            }
            catch (Exception e) { Information.Show(e.Message, "Error while saving currently selected notepad as..."); }
        }

        public void SaveAllNotepadItems()
        {
            foreach (NotepadListItem nli in NotepadItems)
            {
                if (nli.DataContext is FileItemViewModel fivm)
                    SaveNotepad(fivm);
            }
        }

        public void SaveFile(string path, string text)
        {
            try
            {
                NotepadActions.SaveFile(path, text);
                Information.Show($"Successfully saved [{Notepad.Document.FileName}]", InfoTypes.FileIO);
                SelectedNotepadViewModel.HasMadeChanges = false;
            }
            catch (Exception e) { Information.Show(e.Message, "Error while saving text to file."); }
        }

        #endregion

        #endregion

        #region Finding Text

        public void OpenFindWindow()
        {

        }

        #endregion

        #endregion
    }
}
