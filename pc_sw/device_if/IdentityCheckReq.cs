using System;
namespace pc_sw.device_if
{

    public class IdentityCheckReq : Message
    {
        public override MessageId Id { get { return MessageId.IDENTITY_CHECK_REQ; } }
        public override String Description {
            get { return ""; } }
        public override Byte Size { get { return 1; } }
    }
}
