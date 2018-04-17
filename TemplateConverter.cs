using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EditModeSample
{
    public class TemplateConverter : IValueConverter
    {
        public DataTemplate Template { get; set; }
        public DataTemplate EditTemplate { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) ? EditTemplate : Template;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
