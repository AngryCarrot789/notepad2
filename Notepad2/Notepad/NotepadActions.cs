using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Notepad2.Notepad
{
    public static class NotepadActions
    {
        public static RichTextBox richTB;
        // Save XAML in RichTextBox to a file specified by _fileName
        public static void SaveXamlPackage(string _fileName)
        {
            if (richTB != null)
            {
                TextRange range;
                FileStream fStream;
                range = new TextRange(richTB.Document.ContentStart, richTB.Document.ContentEnd);
                fStream = new FileStream(_fileName, FileMode.Create);
                range.Save(fStream, DataFormats.XamlPackage);
                fStream.Close();
            }
        }

        // Load XAML into RichTextBox from a file specified by _fileName
        public static void LoadXamlPackage(string _fileName)
        {
            if (richTB != null)
            {
                TextRange range;
                FileStream fStream;
                if (File.Exists(_fileName))
                {
                    range = new TextRange(richTB.Document.ContentStart, richTB.Document.ContentEnd);
                    fStream = new FileStream(_fileName, FileMode.OpenOrCreate);
                    range.Load(fStream, DataFormats.XamlPackage);
                    fStream.Close();
                }
            }
        }

        public static void SaveFile(string fileName, string fileContent)
        {
            File.WriteAllText(fileName, fileContent);
        }

        public static string ReadFile(string fileName)
        {
            return File.ReadAllText(fileName);
        }
    }
}
