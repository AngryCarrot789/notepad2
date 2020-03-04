using Microsoft.Win32;
using NamespaceHere;
using Notepad2.Notepad;
using Notepad2.Utilities;
using Notepad2.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private ObservableCollection<NotepadListItem> _notepadItems = new ObservableCollection<NotepadListItem>();
        private int _selectedIndex;
        private bool _normalTextBoxSelected;
        private int _textEditorsSelectedIndex;
        private bool _wrapping;

        #endregion

        #region Public Fields

        public ObservableCollection<NotepadListItem> NotepadItems
        {
            get => _notepadItems;
            set => RaisePropertyChanged(ref _notepadItems, value);
        }
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

        public bool Wrapping
        {
            get => _wrapping;
            set
            {
                RaisePropertyChanged(ref _wrapping, value);
                SetWrapping(value);
            }
        }

        #endregion

        public ICommand NewCommand { get; set; }
        public ICommand OpenCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand SaveAsCommand { get; set; }
        public ICommand CloseSelectedNotepadCommand { get; set; }
        public ICommand CloseAllNotepadsCommand { get; set; }
        public ICommand OpenInNewWindowCommand { get; set; }
        public ICommand PrintFileCommand { get; set; }

        public ICommand ClearListAndNotepadCommand { get; set; }

        public ICommand CloseWindowCommand { get; set; }
        public ICommand MaximizeRestoreCommand { get; set; }
        public ICommand MinimizeWindowCommand { get; set; }

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
            TextEditorsSelectedIndex = 0;
            SetupCommands();
        }

        public void SetupCommands()
        {
            NewCommand = new Command(NewNotepad);
            OpenCommand = new Command(OpenNotepadFileFromFileExplorer);
            SaveCommand = new Command(SaveCurrentNotepad);
            SaveAsCommand = new Command(SaveCurrentNotepadAs);
            CloseSelectedNotepadCommand = new Command(CloseSelectedNotepad);
            CloseAllNotepadsCommand = new Command(CloseAllNotepads);
            OpenInNewWindowCommand = new Command(OpenInNewWindow);
            PrintFileCommand = new Command(PrintFile);

            ClearListAndNotepadCommand = new Command(ClearTextAndList);

            CloseWindowCommand = new Command(CloseWindow);
            MaximizeRestoreCommand = new Command(MaximRestre);
            MinimizeWindowCommand = new Command(MinimWindow);
        }

        #region MainWindow TitleTheme

        public MainWindow MainWind { get; set; }
        private void CloseWindow() { if (MainWind != null) MainWind.CloseWindow(); else Application.Current.Shutdown(); }
        private void MaximRestre() { if (MainWind != null) MainWind.MaximizeRestore(); }
        private void MinimWindow() { if (MainWind != null) MainWind.Minimize(); }

        private void OpenInNewWindow()
        {
            MainWindow mWnd = new MainWindow(SelectedNotepadItem, false);
            mWnd.Show();
            mWnd.LoadSettings();
            CloseSelectedNotepad();
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Returns true if null
        /// </summary>
        /// <returns></returns>
        public bool CheckNotepadNull()
        {
            return Notepad == null || Notepad.Document == null || Notepad.DocumentFormat == null;
        }
        StreamReader streamToPrint;
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

        public void Shutdown()
        {
            Help.Shutdown();
            bool changesMade = false;
            foreach (NotepadListItem nli in NotepadItems)
            {
                // if (nli's datacontext is a fileitemviewmode, it's called fivm.
                if (nli != null && nli.DataContext is FileItemViewModel fivm)
                {
                    if (fivm != null && fivm.HasMadeChanges)
                    {
                        changesMade = true;
                    }
                }
            }

            if (changesMade)
            {
                MessageBoxResult mbr = MessageBox.Show(
                    "You have unsaved work. Do you want to save it/them?",
                    "Unsaved Work",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information);

                if (mbr == MessageBoxResult.Yes)
                    SaveAllNotepadItems();
                else if (mbr == MessageBoxResult.No)
                {
                    //nothin ;)
                }

            }
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
            AddNotepadItem(CreateDefaultStyleNotepadItem("", "newNotepad.txt", null, 0));
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

        public void SetWrapping(bool setWrap)
        {
            if (setWrap)
                Notepad.DocumentFormat.Wrap = TextWrapping.Wrap;
            else
                Notepad.DocumentFormat.Wrap = TextWrapping.NoWrap;
        }

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
                Wrap = TextWrapping.Wrap
            };
            return CreateNotepadItem(text, itemName, itemPath, itemSize, fm);
        }

        public NotepadListItem CreateNotepadItem(string text, string itemName, string itemPath, double itemSize, FormatModel fm)
        {
            NotepadListItem nli = new NotepadListItem();
            FileItemViewModel fivm = new FileItemViewModel();
            fivm.Document.Text = text;
            fivm.Document.FileName = itemName;
            fivm.Document.FilePath = itemPath;
            fivm.Document.FileSize = itemSize;

            fivm.DocumentFormat = fm;

            fivm.HasMadeChanges = false;

            nli.DataContext = fivm;

            nli.Open = this.OpenNotepadItem;
            nli.Close = this.CloseNotepadItem;
            //fivm.DocumentFormat.Size = fm.Size;

            return nli;
        }

        #endregion

        #region Opening

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
                           (double)text.Length / 1000.0));
                }
            }
            catch (Exception e) { Error.Show(e.Message, "Error while opening file from path"); }
        }

        #endregion

        #region Saving

        public void SaveNotepad(NotepadListItem nli)
        {
            try
            {
                FileItemViewModel fivm = nli.DataContext as FileItemViewModel;
                if (File.Exists(fivm.Document.FilePath))
                {
                    SaveFile(fivm.Document.FilePath, fivm.Document.Text);
                    fivm.Document.FileName = Path.GetFileName(fivm.Document.FilePath);
                    fivm.Document.FilePath = fivm.Document.FilePath;
                    fivm.Document.FileSize = (double)fivm.Document.Text.Length / 1000.0;
                }
                else
                {
                    SaveNotepadAs(nli);
                }
            }
            catch (Exception e) { Error.Show(e.Message, "Error while saving a (manual) notepad item"); }
        }

        public void SaveNotepadAs(NotepadListItem nli)
        {
            try
            {
                FileItemViewModel fivm = nli.DataContext as FileItemViewModel;

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                sfd.Title = "Select Files to save";
                sfd.FileName = fivm.Document.FileName;
                sfd.FilterIndex = 1;
                sfd.DefaultExt = "txt";
                sfd.RestoreDirectory = true;

                if (sfd.ShowDialog() == true)
                {
                    SaveFile(sfd.FileName, fivm.Document.Text);
                    fivm.Document.FileName = Path.GetFileName(sfd.FileName);
                    fivm.Document.FilePath = sfd.FileName;
                    fivm.Document.FileSize = (double)fivm.Document.Text.Length / 1000.0;
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
                    {
                        SaveFile(Notepad.Document.FilePath, Notepad.Document.Text);
                        Notepad.Document.FileName = Path.GetFileName(Notepad.Document.FilePath);
                        Notepad.Document.FilePath = Notepad.Document.FilePath;
                        Notepad.Document.FileSize = (double)Notepad.Document.Text.Length / 1000.0;
                    }
                    else
                        SaveCurrentNotepadAs();
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
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                    sfd.Title = "Select Files to save";
                    sfd.FileName = Notepad.Document.FileName;
                    sfd.FilterIndex = 1;
                    sfd.DefaultExt = "txt";
                    sfd.RestoreDirectory = true;

                    if (sfd.ShowDialog() == true)
                    {
                        SaveFile(sfd.FileName, Notepad.Document.Text);
                        Notepad.Document.FileName = Path.GetFileName(sfd.FileName);
                        Notepad.Document.FilePath = sfd.FileName;
                        Notepad.Document.FileSize = (double)Notepad.Document.Text.Length / 1000.0;
                    }
                }
            }
            catch (Exception e) { Error.Show(e.Message, "Error while saving currently selected notepad as..."); }
        }

        public void SaveAllNotepadItems()
        {
            foreach (NotepadListItem nli in NotepadItems)
            {
                SaveNotepad(nli);
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

        #endregion
    }
}
