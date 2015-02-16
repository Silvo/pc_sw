using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pc_sw.Helpers
{
    public class SensorVisibilityChangedMessage
    {
        public byte SensorId { get; private set; }
        public bool IsVisible { get; private set; }

        public SensorVisibilityChangedMessage(byte sensorId, bool isVisible)
        {
            this.SensorId = sensorId;
            this.IsVisible = isVisible;
        }
    }
}
