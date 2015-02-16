using System;
namespace pc_sw.device_if
{

    public static class MessageFactory
    {

        public static Message GetMessage(MessageId id, Byte[] payload)
        {
            switch(id)
            {
                case MessageId.PROTOCOL_ERROR_REQ:
                    return new ProtocolErrorReq();
                case MessageId.PROTOCOL_ERROR_RESP:
                    return new ProtocolErrorResp();
                case MessageId.IDENTITY_CHECK_REQ:
                    return new IdentityCheckReq();
                case MessageId.IDENTITY_CHECK_RESP:
                    return new IdentityCheckResp(payload);
                case MessageId.SENSOR_DATA_REQ:
                    return new SensorDataReq(payload);
                case MessageId.SENSOR_DATA_NONE_RESP:
                    return new SensorDataNoneResp();
                case MessageId.SENSOR_DATA_RESP:
                    return new SensorDataResp(payload);
                case MessageId.SET_TIME_REQ:
                    return new SetTimeReq(payload);
                case MessageId.SET_TIME_RESP:
                    return new SetTimeResp();
                case MessageId.GET_WARNING_LEVELS_REQ:
                    return new GetWarningLevelsReq(payload);
                case MessageId.GET_WARNING_LEVELS_RESP:
                    return new GetWarningLevelsResp(payload);
                case MessageId.SET_WARNING_LEVELS_REQ:
                    return new SetWarningLevelsReq(payload);
                case MessageId.SET_WARNING_LEVELS_RESP:
                    return new SetWarningLevelsResp();
                case MessageId.GET_MEASUREMENT_INTERVAL_REQ:
                    return new GetMeasurementIntervalReq(payload);
                case MessageId.GET_MEASUREMENT_INTERVAL_RESP:
                    return new GetMeasurementIntervalResp(payload);
                case MessageId.SET_MEASUREMENT_INTERVAL_REQ:
                    return new SetMeasurementIntervalReq(payload);
                case MessageId.SET_MEASUREMENT_INTERVAL_RESP:
                    return new SetMeasurementIntervalResp();
                default:
                    throw new NotImplementedException();
            }
        }

        public static Byte GetSize(MessageId id)
        {
            switch(id)
            {
                case MessageId.PROTOCOL_ERROR_REQ:
                    return 1;
                case MessageId.PROTOCOL_ERROR_RESP:
                    return 1;
                case MessageId.IDENTITY_CHECK_REQ:
                    return 1;
                case MessageId.IDENTITY_CHECK_RESP:
                    return 6;
                case MessageId.SENSOR_DATA_REQ:
                    return 5;
                case MessageId.SENSOR_DATA_NONE_RESP:
                    return 1;
                case MessageId.SENSOR_DATA_RESP:
                    return 9;
                case MessageId.SET_TIME_REQ:
                    return 6;
                case MessageId.SET_TIME_RESP:
                    return 1;
                case MessageId.GET_WARNING_LEVELS_REQ:
                    return 2;
                case MessageId.GET_WARNING_LEVELS_RESP:
                    return 5;
                case MessageId.SET_WARNING_LEVELS_REQ:
                    return 6;
                case MessageId.SET_WARNING_LEVELS_RESP:
                    return 1;
                case MessageId.GET_MEASUREMENT_INTERVAL_REQ:
                    return 2;
                case MessageId.GET_MEASUREMENT_INTERVAL_RESP:
                    return 2;
                case MessageId.SET_MEASUREMENT_INTERVAL_REQ:
                    return 3;
                case MessageId.SET_MEASUREMENT_INTERVAL_RESP:
                    return 1;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
