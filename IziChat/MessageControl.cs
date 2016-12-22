using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace IziChat
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:IziChat"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:IziChat;assembly=IziChat"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    public class MessageControl : Control
    {
        static MessageControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MessageControl), new FrameworkPropertyMetadata(typeof(MessageControl)));
        }

        public static readonly DependencyProperty LetterProperty = DependencyProperty.Register(
            "Letter", typeof(string), typeof(MessageControl), new PropertyMetadata(default(string)));

        public string Letter
        {
            get { return (string)GetValue(LetterProperty); }
            set { SetValue(LetterProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message", typeof(string), typeof(MessageControl), new PropertyMetadata(default(string)));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty DateTimeProperty = DependencyProperty.Register(
            "DateTime", typeof(string), typeof(MessageControl), new PropertyMetadata(default(string)));

        public string DateTime
        {
            get { return (string) GetValue(DateTimeProperty); }
            set { SetValue(DateTimeProperty, value); }
        }

        public static readonly DependencyProperty UserNameProperty = DependencyProperty.Register(
            "UserName", typeof(string), typeof(MessageControl), new PropertyMetadata(default(string)));

        public string UserName
        {
            get { return (string)GetValue(UserNameProperty); }
            set { SetValue(UserNameProperty, value); }
        }

        public static readonly DependencyProperty IconLocationProperty = DependencyProperty.Register(
            "IconLocation", typeof(int), typeof(MessageControl), new PropertyMetadata(0));

        public int IconLocation
        {
            get { return (int)GetValue(IconLocationProperty); }
            set { SetValue(IconLocationProperty, value); }
        }

        public static readonly DependencyProperty IconBackgroundProperty = DependencyProperty.Register(
            "IconBackground", typeof(Brush), typeof(MessageControl), new PropertyMetadata(default(Brush)));

        public Brush IconBackground
        {
            get { return (Brush)GetValue(IconBackgroundProperty); }
            set { SetValue(IconBackgroundProperty, value); }
        }

        public static readonly DependencyProperty IconForegroundProperty = DependencyProperty.Register(
            "IconForeground", typeof(Brush), typeof(MessageControl), new PropertyMetadata(default(Brush)));

        public Brush IconForeground
        {
            get { return (Brush)GetValue(IconForegroundProperty); }
            set { SetValue(IconForegroundProperty, value); }
        }


    }
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
