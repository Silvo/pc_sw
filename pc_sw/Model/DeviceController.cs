using pc_sw.device_if;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Collections.Concurrent;

namespace pc_sw.Model
{
    
    using Timer = System.Timers.Timer;
    using pc_sw.Helpers;
    using GalaSoft.MvvmLight.Threading;
    using GalaSoft.MvvmLight.Messaging;
    
    public class DeviceController : IDeviceControl, ISensorDataProvider
    {
        private ConcurrentQueue<Func<bool>> _procedures;
        private IMessageInterface _device;
        private Thread _controller;
        private bool _sensorDataAvailable = true;
        private Int32 _mostRecentTimestamp = MagicNumbers.FIRST_LOG_DATE;

        public event EventHandler DataReceived;

        public DeviceController(IMessageInterface device)
        {
            _procedures = new ConcurrentQueue<Func<bool>>();

            _device = device;

            _controller = new Thread(new ThreadStart(ControllerTask))
            {
                IsBackground = true,
                Priority = ThreadPriority.Normal,
            };

            Messenger.Default.Register<InitialTimestampMessage>(
                this, message =>
                {
                    this._mostRecentTimestamp = message.Timestamp;
                });

            _controller.Start();
        }

        private void GetSensorDataProcedure()
        {
            SensorDataReqPayload reqPayload = new SensorDataReqPayload()
            {
                Time = _mostRecentTimestamp
            };

            Message sentMessage = new SensorDataReq(reqPayload);
            Message receivedMessage;

            try
            {
                receivedMessage = _device.ExchangeMessages(sentMessage);
            }
            catch (Exception e)
            {
                if (_device.IsConnected)
                {
                    _device.Disconnect();
                }

                return;
            }

            if (receivedMessage.Id == MessageId.SENSOR_DATA_RESP)
            {
                _sensorDataAvailable = true;

                DispatcherHelper.CheckBeginInvokeOnUI(
                    () =>
                    {
                        SensorDataRespPayload respPayload = (SensorDataRespPayload)receivedMessage.Payload;
                        
                        if (respPayload.Timestamp > _mostRecentTimestamp)
                        {
                            _mostRecentTimestamp = respPayload.Timestamp;
                        }

                        if (DataReceived != null)
                        {
                            SensorDataSample sample = new SensorDataSample
                            {
                                Timestamp = respPayload.Timestamp,
                                Data = respPayload.Data,
                                SensorId = respPayload.SensorId,
                                SensorType = (SensorType)respPayload.SensorType
                            };

                            DataReceived(this, new DataReceivedEventArgs(sample));
                        }
                    });
                
            }
            else if (receivedMessage.Id == MessageId.SENSOR_DATA_NONE_RESP)
            {
                DateTime mostRecentSampleDate = UnixTime.ToDateTime(_mostRecentTimestamp).Date;

                if (mostRecentSampleDate == DateTime.UtcNow.Date)
                {
                    _sensorDataAvailable = false;
                }
                else
                {
                    _sensorDataAvailable = true;
                    _mostRecentTimestamp = UnixTime.FromDateTime(mostRecentSampleDate.AddDays(1));
                }
            }
            else
            {
                sentMessage = new ProtocolErrorReq();
                try
                {
                    receivedMessage = _device.ExchangeMessages(sentMessage);
                }
                catch (Exception e) { };

                if (receivedMessage.Id != MessageId.PROTOCOL_ERROR_RESP)
                {
                    _device.Disconnect();
                }
            }
        }

        private void ControllerTask()
        {
            while (true)
            {
                SetDeviceTime(null); // Set device time after the connection
                _device.Connect();
                while (_device.IsConnected)
                {
                    if (_procedures.Count > 0)
                    {
                        Func<bool> procedure;

                        if (_procedures.TryPeek(out procedure))
                        {
                            bool success = procedure();
                            
                            // If the procedure was completed succesfully remove the
                            // procedure from the queue
                            if (success)
                            {
                                while(_procedures.TryDequeue(out procedure) == false)
                                    ;
                            }
                        }
                    }
                    else if (_sensorDataAvailable == true)
                    {
                        GetSensorDataProcedure();
                    }
                    else
                    {
                        Thread.Sleep(200);
                        _sensorDataAvailable = true;
                    }
                }
                Thread.Sleep(250);
            }
        }

        public void SetDeviceTime(DeviceTimeSet callback)
        {
            _procedures.Enqueue(
                () =>
                {
                    DateTimeOffset time = DateTimeOffset.Now;
                    SetTimeReqPayload payload = new SetTimeReqPayload()
                    {
                        Time = UnixTime.FromDateTime(time.UtcDateTime),
                        Offset = (SByte)time.Offset.TotalHours
                    };

                    Message sentMessage = new SetTimeReq(payload);

                    Message receivedMessage;

                    try
                    {
                        receivedMessage = _device.ExchangeMessages(sentMessage);
                    }
                    catch (Exception e)
                    {
                        if (_device.IsConnected)
                        {
                            _device.Disconnect();
                        }

                        return false;
                    }

                    if (receivedMessage.Id == MessageId.SET_TIME_RESP)
                    {
                        if (callback != null)
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(
                                () => {
                                    Messenger.Default.Send<StatusChangedMessage>(
                                        new StatusChangedMessage("Device time set"));
                                    callback();
                                });
                        }
                        return true;
                    }
                    else
                    {
                        sentMessage = new ProtocolErrorReq();

                        try
                        {
                            receivedMessage = _device.ExchangeMessages(sentMessage);
                        }
                        catch (Exception e) { };

                        if (receivedMessage.Id != MessageId.PROTOCOL_ERROR_RESP)
                        {
                            _device.Disconnect();
                        }
                        return false;
                    }

                });
        }

        public void GetWarningLevels(WarningLevelsReceived callback, byte sensorId)
        {
            _procedures.Enqueue(
                () =>
                {
                    DateTime time = DateTime.Now;
                    TimeSpan offset = time - time.ToUniversalTime();

                    GetWarningLevelsReqPayload reqPayload = new GetWarningLevelsReqPayload
                    {
                        SensorId = sensorId
                    };

                    Message sentMessage = new GetWarningLevelsReq(reqPayload);
                    Message receivedMessage;

                    try
                    {
                        receivedMessage = _device.ExchangeMessages(sentMessage);
                    }
                    catch (Exception e)
                    {
                        if (_device.IsConnected)
                        {
                            _device.Disconnect();
                        }

                        return false;
                    }

                    if (receivedMessage.Id == MessageId.GET_WARNING_LEVELS_RESP)
                    {
                        if (callback != null)
                        {
                            GetWarningLevelsRespPayload respPayload =
                                (GetWarningLevelsRespPayload)receivedMessage.Payload;

                            DispatcherHelper.CheckBeginInvokeOnUI(
                                () =>
                                {
                                    callback(sensorId, respPayload.LowWarningLevel,
                                        respPayload.HighWarningLevel);
                                });
                        }
                        return true;
                    }
                    else
                    {
                        sentMessage = new ProtocolErrorReq();
                        try
                        {
                            receivedMessage = _device.ExchangeMessages(sentMessage);
                        }
                        catch (Exception e) { };

                        if (receivedMessage.Id != MessageId.PROTOCOL_ERROR_RESP)
                        {
                            _device.Disconnect();
                        }
                        return false;
                    }
                });
        }

        public void SetWarningLevels(WarningLevelsSet callback, byte sensorId, Int16 lowLevel, Int16 highLevel)
        {
            _procedures.Enqueue(
                () =>
                {
                    DateTime time = DateTime.Now;
                    TimeSpan offset = time - time.ToUniversalTime();

                    SetWarningLevelsReqPayload reqPayload = new SetWarningLevelsReqPayload
                    {
                        SensorId = sensorId,
                        HighWarningLevel = highLevel,
                        LowWarningLevel = lowLevel
                    };

                    Message sentMessage = new SetWarningLevelsReq(reqPayload);
                    Message receivedMessage;

                    try
                    {
                        receivedMessage = _device.ExchangeMessages(sentMessage);
                    }
                    catch (Exception e)
                    {
                        if (_device.IsConnected)
                        {
                            _device.Disconnect();
                        }

                        return false;
                    }

                    if (receivedMessage.Id == MessageId.SET_WARNING_LEVELS_RESP)
                    {
                        if (callback != null)
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(
                                () => { callback(sensorId, lowLevel, highLevel); });
                        }
                        return true;
                    }
                    else
                    {
                        sentMessage = new ProtocolErrorReq();
                        try
                        {
                            receivedMessage = _device.ExchangeMessages(sentMessage);
                        }
                        catch (Exception e) { };

                        if (receivedMessage.Id != MessageId.PROTOCOL_ERROR_RESP)
                        {
                            _device.Disconnect();
                        }
                        return false;
                    }
                });
        }

        public void GetMeasurementInterval(MeasurementIntervalReceived callback, byte sensorId)
        {
            _procedures.Enqueue(
                () =>
                {
                    DateTime time = DateTime.Now;
                    TimeSpan offset = time - time.ToUniversalTime();

                    GetMeasurementIntervalReqPayload reqPayload = new GetMeasurementIntervalReqPayload()
                    {
                        SensorId = sensorId
                    };

                    Message sentMessage = new GetMeasurementIntervalReq(reqPayload);
                    Message receivedMessage;

                    try
                    {
                        receivedMessage = _device.ExchangeMessages(sentMessage);
                    }
                    catch (Exception e)
                    {
                        if (_device.IsConnected)
                        {
                            _device.Disconnect();
                        }

                        return false;
                    }

                    if (receivedMessage.Id == MessageId.GET_MEASUREMENT_INTERVAL_RESP)
                    {
                        if (callback != null)
                        {
                            GetMeasurementIntervalRespPayload respPayload =
                                (GetMeasurementIntervalRespPayload)receivedMessage.Payload;

                            DispatcherHelper.CheckBeginInvokeOnUI(
                                () => { callback(sensorId, respPayload.MeasurementInterval); });
                        }
                        return true;
                    }
                    else
                    {
                        sentMessage = new ProtocolErrorReq();
                        try
                        {
                            receivedMessage = _device.ExchangeMessages(sentMessage);
                        }
                        catch (Exception e) { };

                        if (receivedMessage.Id != MessageId.PROTOCOL_ERROR_RESP)
                        {
                            _device.Disconnect();
                        }
                        return false;
                    }
                });
        }

        public void SetMeasurementInterval(MeasurementIntervalSet callback, byte sensorId, byte interval)
        {
            _procedures.Enqueue(
                () =>
                {
                    DateTime time = DateTime.Now;
                    TimeSpan offset = time - time.ToUniversalTime();

                    SetMeasurementIntervalReqPayload payload = new SetMeasurementIntervalReqPayload()
                    {
                        SensorId = sensorId,
                        MeasurementInterval = interval
                    };

                    Message sentMessage = new SetMeasurementIntervalReq(payload);
                    Message receivedMessage;

                    try
                    {
                        receivedMessage = _device.ExchangeMessages(sentMessage);
                    }
                    catch (Exception e)
                    {
                        if (_device.IsConnected)
                        {
                            _device.Disconnect();
                        }

                        return false;
                    }

                    if (receivedMessage.Id == MessageId.SET_MEASUREMENT_INTERVAL_RESP)
                    {
                        if (callback != null)
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(
                                () => { callback(sensorId, interval); });
                        }
                        return true;
                    }
                    else
                    {
                        sentMessage = new ProtocolErrorReq();
                        try
                        {
                            receivedMessage = _device.ExchangeMessages(sentMessage);
                        }
                        catch (Exception e) { };

                        if (receivedMessage.Id != MessageId.PROTOCOL_ERROR_RESP)
                        {
                            _device.Disconnect();
                        }
                        return false;
                    }
                });
        }
    }
}
