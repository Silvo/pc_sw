using System;
using System.Collections.Generic;

namespace pc_sw.device_if
{

    public class SetWarningLevelsReqPayload : MessagePayload
    {

        /* Sensor ID */
        public Byte SensorId { get; set; }

        /*  */
        public Int16 LowWarningLevel { get; set; }

        /*  */
        public Int16 HighWarningLevel { get; set; }

        public SetWarningLevelsReqPayload() { }

        public SetWarningLevelsReqPayload(Byte[] bytes)
        {
            this.SensorId = (Byte)bytes[0];
            this.LowWarningLevel = BitConverter.ToInt16(bytes, 1);
            this.HighWarningLevel = BitConverter.ToInt16(bytes, 3);
        }

        public override Byte[] ToBytes()
        {
            List<Byte> bytes = new List<Byte>(5);

            bytes.Add((Byte)this.SensorId);
            bytes.AddRange(BitConverter.GetBytes(this.LowWarningLevel));
            bytes.AddRange(BitConverter.GetBytes(this.HighWarningLevel));

            return bytes.ToArray();
        }

        public override String ToString()
        {
            return String.Format(
                "SensorId: {0} LowWarningLevel: {1} HighWarningLevel: {2}",
                this.SensorId, this.LowWarningLevel, this.HighWarningLevel);
        }
    }
}
