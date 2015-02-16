using System;
using System.Collections.Generic;
namespace pc_sw.device_if
{

    public abstract class Message
    {
        public abstract MessageId Id { get; }
        public abstract String Description { get; }
        public abstract Byte Size { get; }
        public Boolean Resend { get; set; }
        public DateTime TimeStamp { get; set; }
        public MessageDirection Direction { get; set; }
        public MessagePayload Payload { get; protected set; }

        public virtual Byte[] ToBytes()
        {
            List<byte> bytes = new List<Byte>(this.Size);
            Int32 resendBit = this.Resend ? 1 : 0;
            Byte header = (Byte)(((Byte)this.Id << 1) + resendBit);

            bytes.Add(header);
            if (this.Payload != null)
            {
                bytes.AddRange(this.Payload.ToBytes());
            }

            return bytes.ToArray();
        }

        public override String ToString()
        {
            String directionStr;

            if (this.Direction == MessageDirection.Outgoing)
            {
                directionStr = "Out";
            }
            else
            {
                directionStr = "In ";
            }

            if (this.Payload != null)
            {
                return String.Format(
                    "{0} {1} | {2:000}: {3}{5}    Payload: {4}",
                    directionStr,
                    this.TimeStamp.ToString("o"),
                    (Byte)this.Id,
                    this.Id,
                    this.Payload.ToString(),
                    Environment.NewLine);
            }
            else
            {
                return String.Format(
                    "{0} {1} | {2:000}: {3}",
                    directionStr,
                    this.TimeStamp.ToString("o"),
                    (Byte)this.Id,
                    this.Id);
            }
        }
    }
}
