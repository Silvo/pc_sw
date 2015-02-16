using System;
namespace pc_sw.device_if
{

    public class GetMeasurementIntervalResp : Message
    {
        public override MessageId Id { get { return MessageId.GET_MEASUREMENT_INTERVAL_RESP; } }
        public override String Description {
            get { return ""; } }
        public override Byte Size { get { return 2; } }

        private GetMeasurementIntervalResp() { }

        public GetMeasurementIntervalResp(Byte[] payload)
        {
            this.Payload = new GetMeasurementIntervalRespPayload(payload);
        }

        public GetMeasurementIntervalResp(MessagePayload payload)
        {
            this.Payload = (GetMeasurementIntervalRespPayload)payload;
        }
    }
}
