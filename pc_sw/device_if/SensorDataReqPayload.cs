using pc_sw.Helpers;
using System;
using System.Collections.Generic;

namespace pc_sw.device_if
{

    public class SensorDataReqPayload : MessagePayload
    {

        /* Time */
        public Int32 Time { get; set; }

        public SensorDataReqPayload() { }

        public SensorDataReqPayload(Byte[] bytes)
        {
            this.Time = BitConverter.ToInt32(bytes, 0);
        }

        public override Byte[] ToBytes()
        {
            List<Byte> bytes = new List<Byte>(4);

            bytes.AddRange(BitConverter.GetBytes(this.Time));

            return bytes.ToArray();
        }

        public override String ToString()
        {
            return String.Format(
                "Time: {0}({1})",
                this.Time, UnixTime.ToString(this.Time));
        }
    }
}
