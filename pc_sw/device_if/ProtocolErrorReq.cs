using System;
namespace pc_sw.device_if
{

    public class ProtocolErrorReq : Message
    {
        public override MessageId Id { get { return MessageId.PROTOCOL_ERROR_REQ; } }
        public override String Description {
            get { return ""; } }
        public override Byte Size { get { return 1; } }
    }
}
