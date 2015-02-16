using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pc_sw.device_if
{
    public class WrapperMessage : Message
    {
        public override MessageId Id{ get { return MessageId.INTERNAL; } }
        public override string Description { get { return "Internal message wrapper"; } }
        public override byte Size { get { return 1; } }

        private readonly String _messageText;

        public WrapperMessage(String messageText)
        {
            _messageText = messageText;
            this.Direction = MessageDirection.Internal;
            this.TimeStamp = DateTime.Now;
        }

        public override string ToString()
        {
            return String.Format(
                    "    {0} | {1}",
                    this.TimeStamp.ToString("o"),
                    _messageText);
        }
    }
}
