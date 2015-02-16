using System;
namespace pc_sw.device_if
{

    public class SensorDataNoneResp : Message
    {
        public override MessageId Id { get { return MessageId.SENSOR_DATA_NONE_RESP; } }
        public override String Description {
            get { return ""; } }
        public override Byte Size { get { return 1; } }
    }
}
