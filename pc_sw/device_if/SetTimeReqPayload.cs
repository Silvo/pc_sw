using pc_sw.Helpers;
using System;
using System.Collections.Generic;

namespace pc_sw.device_if
{

    public class SetTimeReqPayload : MessagePayload
    {

        /* Time */
        public Int32 Time { get; set; }

        /* Offset from UTC */
        public SByte Offset { get; set; }

        public SetTimeReqPayload() { }

        public SetTimeReqPayload(Byte[] bytes)
        {
            this.Time = BitConverter.ToInt32(bytes, 0);
            this.Offset = (SByte)bytes[4];
        }

        public override Byte[] ToBytes()
        {
            List<Byte> bytes = new List<Byte>(5);

            bytes.AddRange(BitConverter.GetBytes(this.Time));
            bytes.Add((Byte)this.Offset);

            return bytes.ToArray();
        }

        public override String ToString()
        {
            return String.Format(
                "Time: {0}({1}) Offset: {2}",
                this.Time, UnixTime.ToString(this.Time), this.Offset);
        }
    }
}
