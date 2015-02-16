using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pc_sw.Model
{
    public class SensorDataSample
    {
        public Int32 Timestamp { get; set; }
        public Int16 Data { get; set; }
        public byte SensorId { get; set; }
        public SensorType SensorType { get; set; }

        public override string ToString()
        {
            return String.Format(
                "Time stamp: {0}, Data: {1}, Sensor ID: {2}, Sensor type: {3}",
                Timestamp,
                Data,
                SensorId,
                SensorType);
        }

        public float ToFloat()
        {
            float data;
            switch (SensorType)
            {
                case SensorType.TemperatureSensor:
                    /* Intentional fall through */
                case SensorType.AirHumiditySensor:
                    data = this.Data / 10.0f;
                    break;
                case SensorType.SoilMoistureSensor:
                    data = this.Data / 10.24f;
                    break;
                default:
                    data = (float)this.Data;
                    break;
            }
            
            return data;
        }
    }
}
