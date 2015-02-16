using pc_sw.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pc_sw.Design
{
    public class DummyDeviceController : IDeviceControl, ISensorDataProvider
    {
        public DummyDeviceController(IMessageInterface device)
        {

        }

        public event EventHandler DataReceived
        {
            add { }
            remove { }
        }

        public void SetDeviceTime(DeviceTimeSet callback)
        {
            throw new NotImplementedException();
        }

        public void GetMeasurementInterval(MeasurementIntervalReceived callback, byte sensorId)
        {
            throw new NotImplementedException();
        }

        public void SetMeasurementInterval(MeasurementIntervalSet callback, byte sensorId, byte interval)
        {
            throw new NotImplementedException();
        }

        public void GetWarningLevels(WarningLevelsReceived callback, byte sensorId)
        {
            throw new NotImplementedException();
        }

        public void SetWarningLevels(WarningLevelsSet callback, byte sensorId, short lowLevel, short highLevel)
        {
            throw new NotImplementedException();
        }
    }
}
