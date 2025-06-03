using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace STC.WPF.Services
{
    public class BoolToVisibilityConverterService : IValueConverter
    {
        public bool Invert { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool val = value is bool b && b;

            if (parameter?.ToString() == "invert") val = !val;
            if (Invert) val = !val;

            return val ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is Visibility vis && vis == Visibility.Visible);
        }
    }
}
