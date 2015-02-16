using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using pc_sw.device_if;
using pc_sw.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pc_sw.Model
{
    public class SerialPortDevice : IMessageInterface
    {
        public event Action<Message> MessageReceived;
        public event Action<Message> MessageSent;

        private SerialPort _serialPort;
        public bool IsConnected { get; protected set; }

        public SerialPortDevice()
        {
            this.IsConnected = false;
            
            _serialPort = new SerialPort("COM3", 115200, Parity.None, 8, StopBits.One);
            _serialPort.Handshake = Handshake.None;
            _serialPort.ReadTimeout = 1000;
            _serialPort.WriteTimeout = 1000;
        }

        public void Disconnect()
        {
            this.IsConnected = false;
        }

        public void Connect()
        {
            string[] ports = SerialPort.GetPortNames();
            
            if (ports.Length == 0)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(
                    () =>
                    {
                        Messenger.Default.Send<StatusChangedMessage>(
                            new StatusChangedMessage("No serial ports"));
                    });
                
                Thread.Sleep(2000);
            }
            else
            {
                foreach (String portName in ports)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(
                        () =>
                        {
                            Messenger.Default.Send<StatusChangedMessage>(
                                new StatusChangedMessage("Searching " + portName));
                        });
                    try
                    {
                        _serialPort.PortName = portName;
                    }
                    catch (Exception)
                    {
                        try
                        {
                            _serialPort.Close();
                            _serialPort.PortName = portName;
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                    
                    if (ConnectedToCorrectDevice(_serialPort))
                    {
                        this.IsConnected = true;
                        DispatcherHelper.CheckBeginInvokeOnUI(
                            () =>
                            {
                                Messenger.Default.Send<StatusChangedMessage>(
                                    new StatusChangedMessage("Connected"));
                            });
                        break;
                    }
                }
            }
        }

        private bool ConnectedToCorrectDevice(SerialPort serialPort)
        {
            try
            {
                serialPort.Open();
            }
            catch (Exception)
            {
                return false;
            }

            Message sentMessage = new IdentityCheckReq();
            Message receivedMessage;

            try
            {
                receivedMessage = ExchangeMessages(sentMessage);
            }
            catch (Exception)
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }
                return false;
            }

            if (receivedMessage.Id == MessageId.IDENTITY_CHECK_RESP)
            {
                IdentityCheckRespPayload deviceId = (IdentityCheckRespPayload)receivedMessage.Payload;

                if (deviceId.ProductId == 0x01 && deviceId.ProtocolVersion == 0x01)
                {
                    return true;
                }
            }

            serialPort.Close();
            return false;
        }

        public Message ExchangeMessages(Message messageToSend)
        {
            bool msgIdReceived = false;
            Message receivedMessage;
            MessageId msgId = MessageId.PROTOCOL_ERROR_RESP;
            byte[] msgPayload;
            int payloadSize = 0;

            messageToSend.Resend = false;

            for (int i = 0; i < 3; i++)
            {
                SendMessage(messageToSend);

                for (int j = 0; j < 10; j++)
                {
                    Thread.Sleep(1 << j);

                    if (msgIdReceived == false &&
                        _serialPort.BytesToRead >= 1)
                    {
                        Byte header = (Byte)_serialPort.ReadByte();
                        msgId = (MessageId)(header >> 1);
                        payloadSize = MessageFactory.GetSize(msgId) - 1;
                        msgIdReceived = true;
                    }
                    
                    if (msgIdReceived == true)
                    {
                        if (_serialPort.BytesToRead == payloadSize)
                        {
                            msgPayload = new byte[payloadSize];
                            _serialPort.Read(msgPayload, 0, payloadSize);
                            receivedMessage = MessageFactory.GetMessage(msgId, msgPayload);

                            receivedMessage.TimeStamp = DateTime.Now;
                            receivedMessage.Direction = MessageDirection.Incoming;

                            DispatcherHelper.CheckBeginInvokeOnUI(
                                () =>
                                {
                                    if (MessageReceived != null)
                                    {
                                        MessageReceived(receivedMessage);
                                    }
                                });

                            return receivedMessage;
                        }
                        else if (_serialPort.BytesToRead >= payloadSize)
                        {
                            byte[] buffer = new byte[_serialPort.BytesToRead];
                            _serialPort.Read(buffer, 0, _serialPort.BytesToRead);
                        }
                    }
                }
                messageToSend.Resend = true;
            }
            Disconnect();
            throw new ApplicationException("No response from the device");
        }
        
        private void SendMessage(Message msg)
        {
            msg.TimeStamp = DateTime.Now;
            msg.Direction = MessageDirection.Outgoing;

            byte[] msgBytes = msg.ToBytes();

            _serialPort.Write(msgBytes, 0, msgBytes.Length);

            DispatcherHelper.CheckBeginInvokeOnUI(
                () =>
                {
                    if (MessageSent != null)
                    {
                        MessageSent(msg);
                    }
                });
        }
    }
}
