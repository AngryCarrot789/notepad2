using Notepad2.InformationStuff;
using Notepad2.RecyclingBin;
using Notepad2.Utilities;
using Notepad2.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace Notepad2.Notepad
{
    /// <summary>
    /// Interaction logic for NotepadListItem.xaml
    /// </summary>
    public partial class NotepadListItem : UserControl
    {
        public Action<NotepadListItem> Close { get; set; }
        public Action<NotepadListItem> Open { get; set; }
        public Action<NotepadListItem> OpenInFileExplorer { get; set; }
        public FileItemViewModel Notepad { get => DataContext as FileItemViewModel; }

        public NotepadListItem()
        {
            InitializeComponent();
        }

        private Point MouseDownPoint;

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            switch (int.Parse(((MenuItem)sender).Uid))
            {
                case 0: Open?.Invoke(this); break;
                case 1: Close?.Invoke(this); break;
                case 2: OpenInFileExplorer?.Invoke(this); break;
                case 3: DeleteFile(); break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close?.Invoke(this);
        }

        private void ElePar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Pressed)
                Close?.Invoke(this);
        }

        private void gripLeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                MouseDownPoint = e.GetPosition(null);
        }

        private void gripMouseMove(object sender, MouseEventArgs e)
        {
            if (MouseDownPoint != e.GetPosition(null) && e.LeftButton == MouseButtonState.Pressed)
            {
                if (DataContext is FileItemViewModel notepad)
                {
                    try
                    {
                        if (File.Exists(notepad.Document.FilePath))
                        {
                            string[] path1 = new string[1] { notepad.Document.FilePath };
                            SetDraggingStatus(true);
                            DragDrop.DoDragDrop(this, new DataObject(DataFormats.FileDrop, path1), DragDropEffects.Copy);
                            SetDraggingStatus(false);
                        }
                        else
                        {
                            string tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), notepad.Document.FileName);
                            File.WriteAllText(tempFilePath, notepad.Document.Text);
                            string[] path = new string[1] { tempFilePath };
                            SetDraggingStatus(true);
                            DragDrop.DoDragDrop(this, new DataObject(DataFormats.FileDrop, path), DragDropEffects.Copy);
                            File.Delete(tempFilePath);
                            SetDraggingStatus(false);
                        }
                    }
                    catch { }
                }
            }
        }

        public void SetDraggingStatus(bool isDragging)
        {
            if (isDragging)
            {
                BorderThickness = new Thickness(2);
                Information.Show($"Started dragging", "DragDrop");
            }
            else
            {
                BorderThickness = new Thickness(0);
            }
        }

        public void DeleteFile()
        {
            string fileName = Notepad.Document.FilePath;
            if (File.Exists(Notepad.Document.FilePath))
                Task.Run(() => RecycleBin.SilentSend(fileName));
            Close?.Invoke(this);
        }

        private void SetFileExtensionsClicks(object sender, RoutedEventArgs e)
        {
            string notepadName = Notepad.Document.FileName;
            string extension = ((MenuItem)sender).Uid;
            bool hasExtension = false;
            foreach (string extensionThing in GlobalPreferences.PRESET_EXTENSIONS)
            {
                if (notepadName.Contains(extensionThing))
                {
                    hasExtension = true;
                    break;
                }
            }
            if (hasExtension)
            {
                string[] dotSplits = notepadName.Split('.');
                string curExtension = dotSplits[dotSplits.Count() - 1];
                int noCharToRemove = curExtension.Length + 1;
                string fileNameNoExtension = notepadName.Substring(0, notepadName.Length - noCharToRemove);
                Notepad.Document.FileName = fileNameNoExtension + extension;
            }
            else
            {
                Notepad.Document.FileName += extension;
            }
        }
    }
}