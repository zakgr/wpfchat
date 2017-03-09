using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfChat.Converters
{
    public class AligmentToNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (HorizontalAlignment)value == System.Windows.HorizontalAlignment.Left ? 0 : 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
