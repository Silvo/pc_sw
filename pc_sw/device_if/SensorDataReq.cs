using System;
namespace pc_sw.device_if
{

    public class SensorDataReq : Message
    {
        public override MessageId Id { get { return MessageId.SENSOR_DATA_REQ; } }
        public override String Description {
            get { return ""; } }
        public override Byte Size { get { return 5; } }

        private SensorDataReq() { }

        public SensorDataReq(Byte[] payload)
        {
            this.Payload = new SensorDataReqPayload(payload);
        }

        public SensorDataReq(MessagePayload payload)
        {
            this.Payload = (SensorDataReqPayload)payload;
        }
    }
}
