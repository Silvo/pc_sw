using System;
using System.Collections.Generic;

namespace pc_sw.device_if
{

    public class SetMeasurementIntervalReqPayload : MessagePayload
    {

        /* Sensor ID */
        public Byte SensorId { get; set; }

        /* Measurement interval */
        public Byte MeasurementInterval { get; set; }

        public SetMeasurementIntervalReqPayload() { }

        public SetMeasurementIntervalReqPayload(Byte[] bytes)
        {
            this.SensorId = (Byte)bytes[0];
            this.MeasurementInterval = (Byte)bytes[1];
        }

        public override Byte[] ToBytes()
        {
            List<Byte> bytes = new List<Byte>(2);

            bytes.Add((Byte)this.SensorId);
            bytes.Add((Byte)this.MeasurementInterval);

            return bytes.ToArray();
        }

        public override String ToString()
        {
            return String.Format(
                "SensorId: {0} MeasurementInterval: {1}",
                this.SensorId, this.MeasurementInterval);
        }
    }
}
