using System;
namespace pc_sw.device_if
{

    public class SetWarningLevelsReq : Message
    {
        public override MessageId Id { get { return MessageId.SET_WARNING_LEVELS_REQ; } }
        public override String Description {
            get { return ""; } }
        public override Byte Size { get { return 6; } }

        private SetWarningLevelsReq() { }

        public SetWarningLevelsReq(Byte[] payload)
        {
            this.Payload = new SetWarningLevelsReqPayload(payload);
        }

        public SetWarningLevelsReq(MessagePayload payload)
        {
            this.Payload = (SetWarningLevelsReqPayload)payload;
        }
    }
}
