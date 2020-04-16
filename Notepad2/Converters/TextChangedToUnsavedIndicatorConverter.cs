using System;
using System.Globalization;
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
            return (bool)value ? "Unsaved" : "Saved";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
