using Notepad2.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Notepad2.Converters
{
    public class FileSizeToColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.TryParse(value.ToString(), out double fileSizeKB))
            {
                if (fileSizeKB < GlobalPreferences.WARN_FILE_SIZE_KB)
                    return Colors.Transparent;
                else if (
                    fileSizeKB > GlobalPreferences.WARN_FILE_SIZE_KB && 
                    fileSizeKB < GlobalPreferences.ALERT_FILE_SIZE_KB)
                    return GlobalPreferences.WARN_FILE_TOO_BIG_COLOUR;
                else if (fileSizeKB > GlobalPreferences.ALERT_FILE_SIZE_KB)
                    return GlobalPreferences.ALERT_FILE_TOO_BIG_COLOUR;
            }
            return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Color)value == Colors.Red ? GlobalPreferences.WARN_FILE_SIZE_KB : 0;
        }
    }
}
