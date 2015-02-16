using System;
namespace pc_sw.device_if
{

    public class GetMeasurementIntervalReq : Message
    {
        public override MessageId Id { get { return MessageId.GET_MEASUREMENT_INTERVAL_REQ; } }
        public override String Description {
            get { return ""; } }
        public override Byte Size { get { return 2; } }

        private GetMeasurementIntervalReq() { }

        public GetMeasurementIntervalReq(Byte[] payload)
        {
            this.Payload = new GetMeasurementIntervalReqPayload(payload);
        }

        public GetMeasurementIntervalReq(MessagePayload payload)
        {
            this.Payload = (GetMeasurementIntervalReqPayload)payload;
        }
    }
}
