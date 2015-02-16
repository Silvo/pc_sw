using System;
namespace pc_sw.device_if
{

    public class GetWarningLevelsReq : Message
    {
        public override MessageId Id { get { return MessageId.GET_WARNING_LEVELS_REQ; } }
        public override String Description {
            get { return ""; } }
        public override Byte Size { get { return 2; } }

        private GetWarningLevelsReq() { }

        public GetWarningLevelsReq(Byte[] payload)
        {
            this.Payload = new GetWarningLevelsReqPayload(payload);
        }

        public GetWarningLevelsReq(MessagePayload payload)
        {
            this.Payload = (GetWarningLevelsReqPayload)payload;
        }
    }
}
