using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace IziChat.Converters
{
    public class ConverterUserCheck : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length!=3) throw new Exception("2 Usernames Expected");
            return values[0].ToString()==values[1].ToString()+ values[2].ToString() ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
