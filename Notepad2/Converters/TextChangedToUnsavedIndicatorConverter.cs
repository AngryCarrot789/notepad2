using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Notepad2.Converters
{
    /// <summary>
    /// I know the name of this converter is really long... i couldnt think of anything
    /// else to name it
    /// </summary>
    public class TextChangedToUnsavedIndicatorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool hasChanged = (bool)value;
            if (hasChanged)
            {
                return "Unsaved";
            }
            else
            {
                return "Saved";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
