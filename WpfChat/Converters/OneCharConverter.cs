using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfChat.Converters
{
    public class OneCharConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString().ToUpper()[0] ?? ' ';
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
