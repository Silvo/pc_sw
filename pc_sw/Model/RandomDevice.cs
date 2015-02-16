using GalaSoft.MvvmLight.Threading;
using pc_sw.Helpers;
using pc_sw.device_if;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pc_sw.Model
{
    public class RandomDevice : IMessageInterface
    {
        private readonly Random _random;

        public event Action<Message> MessageReceived;
        public event Action<Message> MessageSent;
        public bool IsConnected { get{ return true; } protected set { } }

        public RandomDevice()
        {
            _random = new Random();
        }

        public void Connect() { }
        public void Disconnect() { }

        public Message ExchangeMessages(Message sentMessage)
        {
            sentMessage.TimeStamp = DateTime.Now;
            sentMessage.Direction = MessageDirection.Outgoing;
            DispatcherHelper.CheckBeginInvokeOnUI(
                () =>
                {
                    if (MessageSent != null)
                        MessageSent(sentMessage);
                });
            
            Thread.Sleep(_random.Next(100, 200));

            Message receivedMessage;

            switch (sentMessage.Id)
            {
                case MessageId.PROTOCOL_ERROR_REQ:
                    receivedMessage = new ProtocolErrorResp();
                    break;
                case MessageId.IDENTITY_CHECK_REQ:
                    IdentityCheckRespPayload payload0 = new IdentityCheckRespPayload()
                    {
                        ProductId = 0x0001,
                        DeviceId = 0x0001,
                        ProtocolVersion = 0x01
                    };

                    receivedMessage = new IdentityCheckResp(payload0);
                    break;
                case MessageId.SENSOR_DATA_REQ:
                    int rnd = _random.Next(0,3);
                    
                    if (rnd == 0)
                    {
                        receivedMessage = new SensorDataNoneResp();
                    }
                    else
                    {
                        int timeStamp = UnixTime.FromDateTime(DateTime.Now);
                        short data = (short)_random.Next(-255, 255);
                        byte sensorId = (byte)_random.Next(0, 7);
                        byte sensorType = (byte)_random.Next(1, 4);

                        List<byte> payload1 = new List<byte>(8);
                        payload1.AddRange(BitConverter.GetBytes(timeStamp));
                        payload1.AddRange(BitConverter.GetBytes(data));
                        
                        payload1.Add(sensorId);
                        payload1.Add(sensorType);

                        receivedMessage = new SensorDataResp(payload1.ToArray());
                    }
                    
                    break;
                case MessageId.SET_TIME_REQ:
                    receivedMessage = new SetTimeResp();
                    break;
                default:
                    receivedMessage = new ProtocolErrorResp();
                    break;
            }
            receivedMessage.TimeStamp = DateTime.Now;
            receivedMessage.Direction = MessageDirection.Incoming;

            DispatcherHelper.CheckBeginInvokeOnUI(
                () =>
                {
                    if (MessageReceived != null)
                        MessageReceived(receivedMessage);
                });
            
            return receivedMessage;
        }

    }
}
