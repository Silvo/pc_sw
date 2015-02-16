using pc_sw.Helpers;
using System;
using System.Collections.Generic;

namespace pc_sw.device_if
{

    public class SensorDataRespPayload : MessagePayload
    {

        /* Timestamp */
        public Int32 Timestamp { get; set; }

        /* Measurement data */
        public Int16 Data { get; set; }

        /* Sensor ID */
        public Byte SensorId { get; set; }

        /* Sensor type */
        public Byte SensorType { get; set; }

        public SensorDataRespPayload() { }

        public SensorDataRespPayload(Byte[] bytes)
        {
            this.Timestamp = BitConverter.ToInt32(bytes, 0);
            this.Data = BitConverter.ToInt16(bytes, 4);
            this.SensorId = (Byte)bytes[6];
            this.SensorType = (Byte)bytes[7];
        }

        public override Byte[] ToBytes()
        {
            List<Byte> bytes = new List<Byte>(8);

            bytes.AddRange(BitConverter.GetBytes(this.Timestamp));
            bytes.AddRange(BitConverter.GetBytes(this.Data));
            bytes.Add((Byte)this.SensorId);
            bytes.Add((Byte)this.SensorType);

            return bytes.ToArray();
        }

        public override String ToString()
        {
            return String.Format(
                "Timestamp: {0}({1}) Data: {2} SensorId: {3} SensorType: {4}",
                this.Timestamp, UnixTime.ToString(this.Timestamp), this.Data, this.SensorId, this.SensorType);
        }
    }
}
