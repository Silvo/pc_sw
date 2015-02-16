using System;
using System.Collections.Generic;

namespace pc_sw.device_if
{

    public class GetWarningLevelsRespPayload : MessagePayload
    {

        /*  */
        public Int16 LowWarningLevel { get; set; }

        /*  */
        public Int16 HighWarningLevel { get; set; }

        public GetWarningLevelsRespPayload() { }

        public GetWarningLevelsRespPayload(Byte[] bytes)
        {
            this.LowWarningLevel = BitConverter.ToInt16(bytes, 0);
            this.HighWarningLevel = BitConverter.ToInt16(bytes, 2);
        }

        public override Byte[] ToBytes()
        {
            List<Byte> bytes = new List<Byte>(4);

            bytes.AddRange(BitConverter.GetBytes(this.LowWarningLevel));
            bytes.AddRange(BitConverter.GetBytes(this.HighWarningLevel));

            return bytes.ToArray();
        }

        public override String ToString()
        {
            return String.Format(
                "LowWarningLevel: {0} HighWarningLevel: {1}",
                this.LowWarningLevel, this.HighWarningLevel);
        }
    }
}
