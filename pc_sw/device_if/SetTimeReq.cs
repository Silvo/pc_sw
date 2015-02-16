using System;
namespace pc_sw.device_if
{

    public class SetTimeReq : Message
    {
        public override MessageId Id { get { return MessageId.SET_TIME_REQ; } }
        public override String Description {
            get { return ""; } }
        public override Byte Size { get { return 6; } }

        private SetTimeReq() { }

        public SetTimeReq(Byte[] payload)
        {
            this.Payload = new SetTimeReqPayload(payload);
        }

        public SetTimeReq(MessagePayload payload)
        {
            this.Payload = (SetTimeReqPayload)payload;
        }
    }
}
