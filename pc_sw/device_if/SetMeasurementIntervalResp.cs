using System;
namespace pc_sw.device_if
{

    public class SetMeasurementIntervalResp : Message
    {
        public override MessageId Id { get { return MessageId.SET_MEASUREMENT_INTERVAL_RESP; } }
        public override String Description {
            get { return ""; } }
        public override Byte Size { get { return 1; } }
    }
}
