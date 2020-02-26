using NamespaceHere;
using Notepad2.Notepad;
using Notepad2.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad2.ViewModels
{
    public class FileItemViewModel : BaseViewModel, INotepad
    {
        private bool _hasMadeChanges;
        public bool HasMadeChanges
        {
            get => _hasMadeChanges; set => RaisePropertyChanged(ref _hasMadeChanges, value);
        }

        public FormatModel   DocumentFormat { get; set; }
        public DocumentModel Document       { get; set; }

        public FileItemViewModel()
        {
            DocumentFormat = new FormatModel();
            Document = new DocumentModel();

            Document.TextChanged = TextChanged;
        }

        private void TextChanged() { HasMadeChanges = true; }
    }
}
