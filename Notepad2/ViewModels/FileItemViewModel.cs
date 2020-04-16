using NamespaceHere;
using Notepad2.Notepad;

namespace Notepad2.ViewModels
{
    public class FileItemViewModel : BaseViewModel
    {
        private bool _hasMadeChanges;
        public bool HasMadeChanges
        {
            get => _hasMadeChanges; set => RaisePropertyChanged(ref _hasMadeChanges, value);
        }

        public FormatModel DocumentFormat { get; set; }
        public DocumentModel Document { get; set; }

        public FileItemViewModel()
        {
            DocumentFormat = new FormatModel();
            Document = new DocumentModel();

            Document.TextChanged = TextChanged;
        }

        public FileItemViewModel(NotepadViewModel nvm)
        {
            DocumentFormat = nvm.DocumentFormat;
            Document = nvm.Document;

            Document.TextChanged = TextChanged;
        }

        private void TextChanged() { HasMadeChanges = true; }
    }
}
