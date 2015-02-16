using System;
namespace pc_sw.device_if
{

    public class GetWarningLevelsResp : Message
    {
        public override MessageId Id { get { return MessageId.GET_WARNING_LEVELS_RESP; } }
        public override String Description {
            get { return ""; } }
        public override Byte Size { get { return 5; } }

        private GetWarningLevelsResp() { }

        public GetWarningLevelsResp(Byte[] payload)
        {
            this.Payload = new GetWarningLevelsRespPayload(payload);
        }

        public GetWarningLevelsResp(MessagePayload payload)
        {
            this.Payload = (GetWarningLevelsRespPayload)payload;
        }
    }
}
