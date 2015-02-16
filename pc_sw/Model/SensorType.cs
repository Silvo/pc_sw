using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pc_sw.Model
{
    public enum SensorType : byte
    {
        TemperatureSensor = 0,
        AirHumiditySensor = 1,
        SoilMoistureSensor = 2
    }
}
