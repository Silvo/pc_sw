using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pc_sw.Model
{
    public delegate void DeviceTimeSet();
    public delegate void MeasurementIntervalReceived(byte sensorId, byte measurementInterval);
    public delegate void MeasurementIntervalSet(byte sensorId, byte measurementInterval);
    public delegate void WarningLevelsReceived(byte sensorId, Int16 lowWarningLevel, Int16 highWarningLevel);
    public delegate void WarningLevelsSet(byte sensorId, Int16 lowWarningLevel, Int16 highWarningLevel);

    public interface IDeviceControl
    {
        void SetDeviceTime(DeviceTimeSet callback);
        void GetMeasurementInterval(MeasurementIntervalReceived callback, byte sensorId);
        void SetMeasurementInterval(MeasurementIntervalSet callback, byte sensorId, byte interval);
        void GetWarningLevels(WarningLevelsReceived callback, byte sensorId);
        void SetWarningLevels(WarningLevelsSet callback, byte sensorId, Int16 lowLevel, Int16 highLevel);
    }
}
