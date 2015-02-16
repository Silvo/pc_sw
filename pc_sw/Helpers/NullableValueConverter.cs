using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace pc_sw.Helpers
{
    public class NullableValueConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (String.IsNullOrWhiteSpace(value.ToString()))
            {
                return null;
            }
            else
            {
                return value;
            }
        }
    }
}
