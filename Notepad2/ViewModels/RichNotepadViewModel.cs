﻿using Notepad2.Utilities;
using Notepad2.Notepad;

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

        public void SetNotepad(NotepadViewModel fivm)
        {
            this.DocumentFormat = fivm.DocumentFormat;
            this.Document = fivm.Document;
        }
    }
}
