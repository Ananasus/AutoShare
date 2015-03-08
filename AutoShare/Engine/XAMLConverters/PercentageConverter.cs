using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Engine.XAMLConverters
{
    public class PercentageConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDouble(value) * Double.Parse(parameter.ToString(), CultureInfo.InvariantCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDouble(value) / Double.Parse(parameter.ToString(), CultureInfo.InvariantCulture);
        }

        
    }
}