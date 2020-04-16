using NamespaceHere;
using System;

namespace Notepad2.Notepad
{
    public class DocumentModel : BaseViewModel
    {
        public Action TextChanged { get; set; }
        private string _text;
        public string Text
        {
            get { return _text; }
            set { RaisePropertyChanged(ref _text, value); TextChanged?.Invoke(); }
        }

        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set { RaisePropertyChanged(ref _filePath, value); }
        }

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { RaisePropertyChanged(ref _fileName, value); }
        }

        private double _fileSize;
        public double FileSize
        {
            get { return _fileSize; }
            set { RaisePropertyChanged(ref _fileSize, value); }
        }

        public bool IsEmpty
        {
            get
            {
                if (string.IsNullOrEmpty(FileName) ||
                    string.IsNullOrEmpty(FilePath))
                    return true;

                return false;
            }
        }

        public static DocumentModel Duplicate(DocumentModel doc)
        {
            return new DocumentModel()
            {
                Text = doc.Text,
                FilePath = doc.FilePath,
                FileName = doc.FileName,
                FileSize = doc.FileSize
            };
        }
    }
}
