using pc_sw.device_if;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pc_sw.Model
{
    public interface IMessageInterface
    {
        event Action<Message> MessageReceived;
        event Action<Message> MessageSent;

        bool IsConnected { get; }
        void Connect();
        void Disconnect();
        Message ExchangeMessages(Message msg);
    }
}
