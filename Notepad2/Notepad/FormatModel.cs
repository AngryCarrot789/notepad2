using NamespaceHere;
using System.Windows;
using System.Windows.Media;

namespace Notepad2.Notepad
{
    public class FormatModel : BaseViewModel
    {
        private FontStyle _style;
        public FontStyle Style
        {
            get { return _style; }
            set { RaisePropertyChanged(ref _style, value); }
        }

        private FontWeight _weight;
        public FontWeight Weight
        {
            get { return _weight; }
            set { RaisePropertyChanged(ref _weight, value); }
        }

        private TextDecorationCollection _decoration;
        public TextDecorationCollection Decoration
        {
            get => _decoration;
            set => RaisePropertyChanged(ref _decoration, value);
        }

        private FontFamily _family;
        public FontFamily Family
        {
            get { return _family; }
            set { RaisePropertyChanged(ref _family, value); }
        }

        private TextWrapping _wrap;
        public TextWrapping Wrap
        {
            get { return _wrap; }
            set
            {
                RaisePropertyChanged(ref _wrap, value);
            }
        }

        private bool _isWrapped;
        public bool IsWrapped
        {
            get { return _isWrapped; }
            set
            {
                RaisePropertyChanged(ref _isWrapped, value);
                Wrap = value ? TextWrapping.Wrap : TextWrapping.NoWrap;
            }
        }

        private double _size;
        public double Size
        {
            get { return _size; }
            set { RaisePropertyChanged(ref _size, value); }
        }
    }
}
