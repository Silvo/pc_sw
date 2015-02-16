using System;
namespace pc_sw.device_if
{

    public class SensorDataResp : Message
    {
        public override MessageId Id { get { return MessageId.SENSOR_DATA_RESP; } }
        public override String Description {
            get { return ""; } }
        public override Byte Size { get { return 9; } }

        private SensorDataResp() { }

        public SensorDataResp(Byte[] payload)
        {
            this.Payload = new SensorDataRespPayload(payload);
        }

        public SensorDataResp(MessagePayload payload)
        {
            this.Payload = (SensorDataRespPayload)payload;
        }
    }
}
