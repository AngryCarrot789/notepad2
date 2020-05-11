﻿using Microsoft.Win32;
using NamespaceHere;
using Notepad2.Notepad;
using Notepad2.Utilities;
using Notepad2.Views;
using System;
using System.Collections.Generic;
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

        public ICommand ClearListAndNotepadCommand { get; set; }

        // The ViewModel for the Notepad. This contains a DocumentModel and FormatModel, for holding
        // Styles, Text, FilePath, etc.
        public NotepadViewModel Notepad
        {
            get => _notepad;
            set => RaisePropertyChanged(ref _notepad, value);
        }

        // ViewModel for the help window (idk why it needs a view model...)
        public HelpViewModel Help { get; set; }
        public FindTextWindow FindWindow { get; set; }
        public MainWindow MainWind { get; set; }

        public Action<NotepadListItem, AnimationFlag> AnimateAddCallback { get; set; }
        public Action<string, string, bool, bool> FindTextCallback { get; set; }

        public MainViewModel()
        {
            Notepad = new NotepadViewModel();
            Help = new HelpViewModel();
            NotepadItems = new ObservableCollection<NotepadListItem>();
            TextEditorsSelectedIndex = 0;
            SetupCommands();

            FindWindow = new FindTextWindow();
            FindWindow.FindNext = FindAndSelect;
            KeydownManager.KeyDown += KeydownManager_KeyDown;
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

            ClearListAndNotepadCommand = new Command(ClearTextAndList);
        }

        private void OpenInNewWindow()
        {
            MainWindow mWnd = new MainWindow(SelectedNotepadItem, false);
            mWnd.Show();
            mWnd.LoadSettings();
            CloseSelectedNotepad();
        }

        #region Helpers

        /// <summary>
        /// Returns true if null
        /// </summary>
        /// <returns></returns>
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
            {
                // if (nli's datacontext is a fileitemviewmode, it's called fivm.
                if (nli != null && nli.DataContext is FileItemViewModel fivm)
                {
                    if (fivm != null && fivm.HasMadeChanges)
                    {
                        return true;
                    }
                }
            }

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
            AddNotepadItem(CreateDefaultStyleNotepadItem("", $"new{NotepadItems.Count()}.txt", null, 0));
        }

        public void AddNotepadItem(NotepadListItem nli)
        {
            NotepadItems.Add(nli);
            AnimateAddCallback?.Invoke(nli, AnimationFlag.NotepadItemOPEN);
            UpdateNotepad();
        }

        public void CloseNotepadItem(NotepadListItem nli)
        {
            //AnimateAddCallback?.Invoke(nli, AnimationFlag.NotepadItemCLOSE);
            NotepadItems.Remove(nli);
            UpdateNotepad();
        }

        private void CloseSelectedNotepad()
        {
            CloseNotepadItem(SelectedNotepadItem);
        }

        private void CloseAllNotepads()
        {
            NotepadItems.Clear();
        }

        #endregion

        #region Other functions

        private void ClearTextAndList()
        {
            NotepadItems.Clear();
            Notepad = new NotepadViewModel();
        }

        /// <summary>
        /// Sets the main notepad view to the selected notepad item. sometimes even if one is selected, one isnt 'set'. this sets it.
        /// </summary>
        private void UpdateNotepad()
        {
            OpenNotepadItem(SelectedNotepadItem);
        }

        #endregion

        #region Create

        public NotepadListItem CreateDefaultStyleNotepadItem(string text, string itemName, string itemPath, double itemSize)
        {
            FontFamily font;
            if (string.IsNullOrEmpty(Properties.Settings.Default.DefaultFont))
                font = new FontFamily("Consolas"); //default font
            else
                font = new FontFamily(Properties.Settings.Default.DefaultFont);
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
                EnableSpellCheck = true
            };
            return CreateNotepadItem(text, itemName, itemPath, itemSize, fm);
        }

        public NotepadListItem CreateNotepadItem(string text, string itemName, string itemPath, double itemSize, FormatModel fm)
        {
            NotepadListItem nli = new NotepadListItem();
            nli.ParentListbox = MainWind.notepadLstBox;
            FileItemViewModel fivm = new FileItemViewModel();
            fivm.Document.Text = text;
            fivm.Document.FileName = itemName;
            fivm.Document.FilePath = itemPath;
            fivm.Document.FileSize = itemSize;

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

                        Process.Start(info);
                    }

                    else
                    {
                        Error.Show("FilePath Doesn't Exist", "FilePath null");
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
                    foreach (string paths in ofd.FileNames)
                    {
                        OpenNotepadFileFromPath(paths);
                        try { SelectedNotepadViewModel.HasMadeChanges = false; } catch { }
                    }
                }
            }
            catch (Exception e) { Error.Show(e.Message, "Error while opening file from f.explorer"); }
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
                           path,
                           text.Length / 1000.0));
                }
            }
            catch (Exception e) { Error.Show(e.Message, "Error while opening file from path"); }
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
                            fivm.Document.FileSize = fivm.Document.Text.Length / 1000.0;
                        }
                    }
                    else
                    {
                        SaveFile(fivm.Document.FilePath, fivm.Document.Text);
                        fivm.Document.FileName = newFileName;
                        fivm.Document.FilePath = newFilePath;
                        fivm.Document.FileSize = fivm.Document.Text.Length / 1000.0;
                    }
                }
                else
                {
                    SaveNotepadAs(fivm);
                }
            }
            catch (Exception e) { Error.Show(e.Message, "Error while saving a (manual) notepad item"); }
        }

        public void SaveNotepadAs(FileItemViewModel fivm)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter =
                        "Plain Text (.txt)|*.txt|" +
                        "HTM File (.htm)|*.htm|" +
                        "HTML File (.html)|*.html|" +
                        "CSS File (.css)|*.css|" +
                        "JS File (.js)|*.js|" +
                        "CS File (.cs)|*.cs|" +
                        "C++ File (.cpp)|*.cpp|" +
                        "C/C++ Header File (.h)|*.h|" +
                        "All files|*.*";
                sfd.Title = "Select Files to save";
                sfd.FileName = fivm.Document.FileName;
                sfd.FilterIndex = 1;
                sfd.DefaultExt = "txt";
                sfd.RestoreDirectory = true;

                if (sfd.ShowDialog() == true)
                {
                    string newFilePath = sfd.FileName;
                    SaveFile(newFilePath, fivm.Document.Text);
                    fivm.Document.FileName = Path.GetFileName(newFilePath);
                    fivm.Document.FilePath = newFilePath;
                    fivm.Document.FileSize = fivm.Document.Text.Length / 1000.0;
                }
            }
            catch (Exception e) { Error.Show(e.Message, "Error while saving (manual) notepaditem as..."); }
        }

        public void SaveCurrentNotepad()
        {
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
            catch (Exception e) { Error.Show(e.Message, "Error while saving currently selected notepad"); }
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
            catch (Exception e) { Error.Show(e.Message, "Error while saving currently selected notepad as..."); }
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
                SelectedNotepadViewModel.HasMadeChanges = false;
            }
            catch (Exception e) { Error.Show(e.Message, "Error while saving text to file."); }
        }

        #endregion

        #region Find text

        public void OpenFindWindow()
        {
            FindWindow.Show();
        }

        public void FindAndSelect(string TextToFind, bool MatchCase, bool SearchDownOrUp)
        {
            FindTextCallback?.Invoke(TextToFind, Notepad.Document.Text, MatchCase, SearchDownOrUp);
        }

        #endregion

        #endregion
    }
}
