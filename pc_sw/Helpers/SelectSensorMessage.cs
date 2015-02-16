using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pc_sw.Helpers
{
    public class SelectSensorMessage
    {
        public byte SensorId { get; set; }
        public string SensorName { get; set; }

        public SelectSensorMessage(byte id, string sensorName)
        {
            this.SensorId = id;
            this.SensorName = sensorName;
        }
    }
}
