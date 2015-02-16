using System;
namespace pc_sw.device_if
{

    public class ProtocolErrorResp : Message
    {
        public override MessageId Id { get { return MessageId.PROTOCOL_ERROR_RESP; } }
        public override String Description {
            get { return ""; } }
        public override Byte Size { get { return 1; } }
    }
}
