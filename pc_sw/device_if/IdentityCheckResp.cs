using System;
namespace pc_sw.device_if
{

    public class IdentityCheckResp : Message
    {
        public override MessageId Id { get { return MessageId.IDENTITY_CHECK_RESP; } }
        public override String Description {
            get { return ""; } }
        public override Byte Size { get { return 6; } }

        private IdentityCheckResp() { }

        public IdentityCheckResp(Byte[] payload)
        {
            this.Payload = new IdentityCheckRespPayload(payload);
        }

        public IdentityCheckResp(MessagePayload payload)
        {
            this.Payload = (IdentityCheckRespPayload)payload;
        }
    }
}
