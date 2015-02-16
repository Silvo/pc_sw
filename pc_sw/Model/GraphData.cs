using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pc_sw.Helpers;

namespace pc_sw.Model
{
    public class GraphData
    {
        public Pen DrawingPen { get; set; }
        public ObservableCollection<Tuple<DateTime, Single>> Points { get; set; }
        public Single MaxAmplitude { get; set; }
        public Single HighWarningLevel { get; set; }
        public Single LowWarningLevel { get; set; }

        public GraphData(Pen pen, IEnumerable<Tuple<DateTime, Single>> points)
        {
            this.DrawingPen = pen;
            this.Points = new ObservableCollection<Tuple<DateTime,Single>>(points);
            this.HighWarningLevel = MagicNumbers.HIGH_WARNING_OFF_SINGLE;
            this.LowWarningLevel = MagicNumbers.LOW_WARNING_OFF_SINGLE;
        }
    }
}
