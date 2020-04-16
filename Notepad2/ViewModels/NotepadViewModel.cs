using NamespaceHere;
using Notepad2.Notepad;

namespace Notepad2.ViewModels
{
    /// <summary>
    /// Contains everything a notepad would contain; text, wrapping, font size, font, ect
    /// </summary>
    public class NotepadViewModel : BaseViewModel
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

        public NotepadViewModel()
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
