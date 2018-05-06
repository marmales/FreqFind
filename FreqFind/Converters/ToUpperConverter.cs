using System;
using System.Windows.Data;

namespace FreqFind.Converters
{
    /// <summary>
    /// Converts string values to upper case.
    /// </summary>
    public class ToUpperConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                var strValue = value.ToString();

                return strValue.ToUpperInvariant();
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
