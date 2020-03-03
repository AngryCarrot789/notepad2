using NamespaceHere;
using Notepad2.Notepad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad2.ViewModels
{
    public class RichNotepadViewModel : BaseViewModel
    {
        private FormatModel _documentFormat;
        private DocumentModel _document;
        public FormatModel DocumentFormat
        {
            get => _documentFormat;
            set => RaisePropertyChanged(ref _documentFormat, value);
        }
        public DocumentModel Document
        {
            get => _document;
            set => RaisePropertyChanged(ref _document, value);
        }

        public RichNotepadViewModel()
        {
            DocumentFormat = new FormatModel();
            Document = new DocumentModel();
        }

        public void SetNotepad(FileItemViewModel fivm)
        {
            this.DocumentFormat = fivm.DocumentFormat;
            this.Document = fivm.Document;
        }
    }
}
