using System;
using System.Collections.Generic;

namespace pc_sw.device_if
{

    public class GetWarningLevelsReqPayload : MessagePayload
    {

        /* Sensor ID */
        public Byte SensorId { get; set; }

        public GetWarningLevelsReqPayload() { }

        public GetWarningLevelsReqPayload(Byte[] bytes)
        {
            this.SensorId = (Byte)bytes[0];
        }

        public override Byte[] ToBytes()
        {
            List<Byte> bytes = new List<Byte>(1);

            bytes.Add((Byte)this.SensorId);

            return bytes.ToArray();
        }

        public override String ToString()
        {
            return String.Format(
                "SensorId: {0}",
                this.SensorId);
        }
    }
}
