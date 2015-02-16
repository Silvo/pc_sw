using System;
using System.Collections.Generic;

namespace pc_sw.device_if
{

    public class GetMeasurementIntervalRespPayload : MessagePayload
    {

        /* Measurement interval */
        public Byte MeasurementInterval { get; set; }

        public GetMeasurementIntervalRespPayload() { }

        public GetMeasurementIntervalRespPayload(Byte[] bytes)
        {
            this.MeasurementInterval = (Byte)bytes[0];
        }

        public override Byte[] ToBytes()
        {
            List<Byte> bytes = new List<Byte>(1);

            bytes.Add((Byte)this.MeasurementInterval);

            return bytes.ToArray();
        }

        public override String ToString()
        {
            return String.Format(
                "MeasurementInterval: {0}",
                this.MeasurementInterval);
        }
    }
}
