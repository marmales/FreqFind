using System;
using System.Globalization;
using System.Windows.Data;

namespace FreqFind.Converters
{
    public class ToFloatSliderConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var modelValue = (float)value;
            return modelValue * 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sliderValue = (double)value;
            return (float)(sliderValue / 100);
        }
    }
}
