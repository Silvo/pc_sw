using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace pc_sw.Helpers
{
    public class IdToColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            byte R, G, B;

            switch ((byte)value)
            {
                case 0:
                    R = 0xF9;
                    G = 0xB2;
                    B = 0x33;
                    break;

                case 1:
                    R = 0x00;
                    G = 0x9E;
                    B = 0xE3;
                    break;

                case 2:
                    R = 0xE5;
                    G = 0x00;
                    B = 0x7D;
                    break;

                case 3:
                    R = 0x00;
                    G = 0x96;
                    B = 0x40;
                    break;

                case 4:
                    R = 0x94;
                    G = 0x1D;
                    B = 0x80;
                    break;
                
                case 5:
                    R = 0xED;//ed4d2e
                    G = 0x4D;
                    B = 0x2E;
                    break;

                case 6:
                    R = 0x00;
                    G = 0xA0;
                    B = 0x9A;
                    break;

                default:
                    R = 0xFF;
                    G = 0x00;
                    B = 0x00;
                    break;
            }
            
            if (targetType == typeof(Brush))
            {
                return new SolidColorBrush(new Color()
                {
                    R = R,
                    G = G,
                    B = B,
                    A = 0xFF
                });
            }
            else
            {
                return new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(0xFF, R, G, B));
            }
        }

        public object ConvertBack(object value, Type Byte, object parameter, System.Globalization.CultureInfo culture)
        {
            return 0;
        }
    }
}
