using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Engine.XAMLConverters
{
    class MultiIntToBoolConverter:IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType,
               object parameter, System.Globalization.CultureInfo culture)
        {
            for (int i = 1, l = values.Length; i < l; ++i )
                if((int)(values[i])!=(int)(values[i-1]))
                return false;
            return true;
        }
        public object[] ConvertBack(object value, Type[] targetTypes,
               object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }

    }
}
