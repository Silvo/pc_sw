using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using pc_sw.Model;
using System.Windows.Media;
using pc_sw.device_if;

namespace pc_sw.Helpers
{
    public class DirectionToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((MessageDirection)value == MessageDirection.Outgoing)
            {
                return Brushes.Aquamarine;
            }
            else if ((MessageDirection)value == MessageDirection.Incoming)
            {
                return Brushes.Bisque;
            }
            else
            {
                return Brushes.LightSeaGreen;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
