using System;
using System.Collections.Generic;

namespace pc_sw.device_if
{

    public class IdentityCheckRespPayload : MessagePayload
    {

        /* Product ID */
        public UInt16 ProductId { get; set; }

        /*  */
        public UInt16 DeviceId { get; set; }

        /* Protocol version */
        public Byte ProtocolVersion { get; set; }

        public IdentityCheckRespPayload() { }

        public IdentityCheckRespPayload(Byte[] bytes)
        {
            this.ProductId = BitConverter.ToUInt16(bytes, 0);
            this.DeviceId = BitConverter.ToUInt16(bytes, 2);
            this.ProtocolVersion = (Byte)bytes[4];
        }

        public override Byte[] ToBytes()
        {
            List<Byte> bytes = new List<Byte>(5);

            bytes.AddRange(BitConverter.GetBytes(this.ProductId));
            bytes.AddRange(BitConverter.GetBytes(this.DeviceId));
            bytes.Add((Byte)this.ProtocolVersion);

            return bytes.ToArray();
        }

        public override String ToString()
        {
            return String.Format(
                "ProductId: {0} DeviceId: {1} ProtocolVersion: {2}",
                this.ProductId, this.DeviceId, this.ProtocolVersion);
        }
    }
}
