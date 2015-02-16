using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pc_sw.Helpers
{
    public class SensorInfoMessage
    {
        public Byte Id { get; set; }
        public String Name { get; set; }
        public Nullable<Double> LowWarningLevel { get; set; }
        public Nullable<Double> HighWarningLevel { get; set; }

        public SensorInfoMessage(Byte id, String name,
            Nullable<Double> lowWarningLevel, Nullable<Double> highWarningLevel)
        {
            this.Id = id;
            this.Name = name;
            this.LowWarningLevel = lowWarningLevel;
            this.HighWarningLevel = highWarningLevel;
        }
    }
}
