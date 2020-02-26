using NamespaceHere;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                isWrapped = value == TextWrapping.Wrap ? true : false;
            }
        }

        private bool _isWrapped;
        public bool isWrapped
        {
            get { return _isWrapped; }
            set { RaisePropertyChanged(ref _isWrapped, value); }
        }

        private double _size;
        public double Size
        {
            get { return _size; }
            set { RaisePropertyChanged(ref _size, value); }
        }
    }
}
