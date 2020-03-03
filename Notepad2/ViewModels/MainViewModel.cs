using Microsoft.Win32;
using NamespaceHere;
using Notepad2.Notepad;
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
using System.Windows.Input;
using System.Windows.Media;

namespace Notepad2.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        //Notepad View (text, format)
        private NotepadViewModel _notepad;
        public NotepadViewModel Notepad
        {
            get => _notepad;
            set => RaisePropertyChanged(ref _notepad, value);
        }

        public HelpViewModel Help { get; set; }

        private ObservableCollection<NotepadListItem> _notepadItems = new ObservableCollection<NotepadListItem>();
        public ObservableCollection<NotepadListItem> NotepadItems
        {
            get => _notepadItems;
            set => RaisePropertyChanged(ref _notepadItems, value);
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                RaisePropertyChanged(ref _selectedIndex, value);
                UpdateNotepad();
            }
        }

        /// <summary>
        /// Sets the main notepad view to the selected notepad item. sometimes even if one is selected, one isnt 'set'. this sets it.
        /// </summary>
        private void UpdateNotepad()
        {
            OpenNotepadItem(SelectedNotepadItem);
        }

        public NotepadListItem SelectedNotepadItem
        {
            get
            {
                try { return NotepadItems[SelectedIndex]; }
                catch
                {
                    return null;
                }
            }
        }

        public FileItemViewModel SelectedNotepadViewModel
        {
            get
            {
                try { return NotepadItems[SelectedIndex].DataContext as FileItemViewModel; }
                catch
                {
                    return null;
                }
            }
        }

        public ICommand ClearListAndNotepadCommand { get; set; }
        private void ClearTextAndList()
        {
            NotepadItems.Clear();
            Notepad = new NotepadViewModel();
        }
        public ICommand ShowFormatCommand { get; set; }
        private bool _wrapping;
        public bool Wrapping
        {
            get => _wrapping; set
            {
                RaisePropertyChanged(ref _wrapping, value); SetWrapping(value);
            }
        }
        private void SetWrapping(bool setWrap)
        {
            if (setWrap)
                Notepad.DocumentFormat.Wrap = TextWrapping.Wrap;
            else
                Notepad.DocumentFormat.Wrap = TextWrapping.NoWrap;
        }

        public ICommand NewCommand    { get; set; }
        public ICommand OpenCommand   { get; set; }
        public ICommand SaveCommand   { get; set; }
        public ICommand SaveAsCommand { get; set; }

        public ICommand CloseWindowCommand     { get; set; }
        public ICommand MaximizeRestoreCommand { get; set; }
        public ICommand MinimizeWindowCommand  { get; set; }

        public ICommand OpenInNewWindowCommand { get; set; }
        private void OpenInNewWindow()
        {
            MainWindow mWnd = new MainWindow(SelectedNotepadItem, false);
            mWnd.Show();
            CloseSelectedNotepad();
        }

        public ICommand CloseSelectedNotepadCommand { get; set; }
        private void CloseSelectedNotepad()
        {
            CloseNotepadItem(SelectedNotepadItem);
        }
        public ICommand CloseAllNotepadsCommand { get; set; }
        private void CloseAllNotepads()
        {
            try
            {
                foreach (NotepadListItem nli in NotepadItems)
                {
                    CloseNotepadItem(nli);
                }
            }
            catch
            {

            }
        }

        public MainViewModel()
        {
            Notepad = new NotepadViewModel();
            Help = new HelpViewModel();

            OpenInNewWindowCommand = new Command(OpenInNewWindow);

            ClearListAndNotepadCommand = new Command(ClearTextAndList);
            NewCommand = new Command(NewNotepad);
            OpenCommand = new Command(OpenNotepadFileFromFileExplorer);
            SaveCommand = new Command(SaveCurrentNotepad);
            SaveAsCommand = new Command(SaveCurrentNotepadAs);

            CloseWindowCommand = new Command(CloseWindow);
            MaximizeRestoreCommand = new Command(MaximRestre);
            MinimizeWindowCommand = new Command(MinimWindow);

            CloseSelectedNotepadCommand = new Command(CloseSelectedNotepad);
            CloseAllNotepadsCommand = new Command(CloseAllNotepads);
        }
        public MainWindow MainWind { get; set; }
        private void CloseWindow() { if (MainWind != null) MainWind.CloseWindow(); else Application.Current.Shutdown(); }
        private void MaximRestre() { if (MainWind != null) MainWind.MaximizeRestore(); }
        private void MinimWindow() { if (MainWind != null) MainWind.Minimize(); }

        #region NotepadFunctions
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
            fivm.Document.Text     = text;
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
        #endregion

        public void NewNotepad()
        {
            AddNotepadItem(CreateDefaultStyleNotepadItem("", "newNotepad.txt", null, 0));
        }

        public void AddNotepadItem(NotepadListItem nli)
        {
            NotepadItems.Add(nli);
            UpdateNotepad();
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
            catch(Exception e) { MessageBox.Show(e.Message, "Error while opening file from f.explorer"); }
        }

        public void OpenNotepadFileFromPath(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    string text = File.ReadAllText(path);
                    AddNotepadItem(
                        CreateDefaultStyleNotepadItem(
                           text,
                           Path.GetFileName(path),
                           path,
                           (double)text.Length / 1000.0));
                }
            }
            catch (Exception e) { MessageBox.Show(e.Message, "Error while opening file from path"); }
        }

        public void SaveFile(string path, string text)
        {
            try
            {
                File.WriteAllText(path, text);
                SelectedNotepadViewModel.HasMadeChanges = false;
            }
            catch (Exception e) { MessageBox.Show(e.Message, "Error while saving text to file."); }
        }

        public void SaveAllNotepadItems()
        {
            foreach(NotepadListItem nli in NotepadItems)
            {
                SaveNotepad(nli);
            }
        }

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
            catch (Exception e) { MessageBox.Show(e.Message, "Error while saving a (manual) notepad item"); }
        }

        /// <summary>
        /// Returns true if null
        /// </summary>
        /// <returns></returns>
        public bool CheckNotepadNull()
        {
            return Notepad == null || Notepad.Document == null || Notepad.DocumentFormat == null;
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
            catch (Exception e) { MessageBox.Show(e.Message, "Error while saving currently selected notepad"); }
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
            catch (Exception e) { MessageBox.Show(e.Message, "Error while saving currently selected notepad as..."); }
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
            catch (Exception e) { MessageBox.Show(e.Message, "Error while saving (manual) notepaditem as..."); }
        }

        public void CloseNotepadItem(NotepadListItem nli)
        {
            NotepadItems.Remove(nli);
            UpdateNotepad();
        }

        public void Shutdown()
        {
            Help.Shutdown();
            bool changesMade = false;
            foreach(NotepadListItem nli in NotepadItems)
            {
                FileItemViewModel fivm = nli.DataContext as FileItemViewModel;

                if (fivm.HasMadeChanges)
                {
                    changesMade = true;
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
    }
}
