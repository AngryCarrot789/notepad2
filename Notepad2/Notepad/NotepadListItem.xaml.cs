using Notepad2.Utilities;
using Notepad2.ViewModels;
using Notepad2.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

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
        public ListBox ParentListbox { get; set; }

        public NotepadListItem()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            switch (int.Parse(((MenuItem)sender).Uid))
            {
                case 0: Open?.Invoke(this); break;
                case 1: Close?.Invoke(this); break;
                case 2: OpenInFileExplorer?.Invoke(this); break;
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

        private void ElePar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (DataContext is FileItemViewModel notepad)
                {
                    try
                    {
                        if (File.Exists(notepad.Document.FilePath))
                        {
                            string[] path1 = new string[1] { notepad.Document.FilePath };
                            DragDrop.DoDragDrop(this, new DataObject(DataFormats.FileDrop, path1), DragDropEffects.Copy);
                        }
                        else
                        {
                            string tempFilePath = Path.Combine(Path.GetTempPath(), notepad.Document.FileName);
                            notepad.Document.FilePath = tempFilePath;
                            File.WriteAllText(tempFilePath, notepad.Document.Text);
                            string[] path = new string[1] { tempFilePath };
                            DragDrop.DoDragDrop(this, new DataObject(DataFormats.FileDrop, path), DragDropEffects.Copy);
                            File.Delete(tempFilePath);
                        }
                    }
                    catch { }
                }
            }
        }

        private void ElePar_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
