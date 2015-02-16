using System;
namespace pc_sw.device_if
{

    public enum MessageId : byte
    {
        INTERNAL                      = 0x00,
        PROTOCOL_ERROR_REQ            = 0x01,
        PROTOCOL_ERROR_RESP           = 0x02,
        IDENTITY_CHECK_REQ            = 0x1C,
        IDENTITY_CHECK_RESP           = 0x1D,
        SENSOR_DATA_REQ               = 0x21,
        SENSOR_DATA_NONE_RESP         = 0x22,
        SENSOR_DATA_RESP              = 0x23,
        SET_TIME_REQ                  = 0x5C,
        SET_TIME_RESP                 = 0x5D,
        GET_WARNING_LEVELS_REQ        = 0x61,
        GET_WARNING_LEVELS_RESP       = 0x62,
        SET_WARNING_LEVELS_REQ        = 0x63,
        SET_WARNING_LEVELS_RESP       = 0x64,
        GET_MEASUREMENT_INTERVAL_REQ  = 0x70,
        GET_MEASUREMENT_INTERVAL_RESP = 0x71,
        SET_MEASUREMENT_INTERVAL_REQ  = 0x72,
        SET_MEASUREMENT_INTERVAL_RESP = 0x73,
    }

    public enum MessageDirection
    {
        Incoming,
        Outgoing,
        Internal
    }
}
