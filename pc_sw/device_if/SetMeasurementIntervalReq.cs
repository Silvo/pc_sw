using System;
namespace pc_sw.device_if
{

    public class SetMeasurementIntervalReq : Message
    {
        public override MessageId Id { get { return MessageId.SET_MEASUREMENT_INTERVAL_REQ; } }
        public override String Description {
            get { return ""; } }
        public override Byte Size { get { return 3; } }

        private SetMeasurementIntervalReq() { }

        public SetMeasurementIntervalReq(Byte[] payload)
        {
            this.Payload = new SetMeasurementIntervalReqPayload(payload);
        }

        public SetMeasurementIntervalReq(MessagePayload payload)
        {
            this.Payload = (SetMeasurementIntervalReqPayload)payload;
        }
    }
}
