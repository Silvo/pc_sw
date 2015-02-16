using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pc_sw.Helpers
{
    public class InitialTimestampMessage
    {
        public Int32 Timestamp { get; set; }

        public InitialTimestampMessage(Int32 timeStamp)
        {
            this.Timestamp = timeStamp;
        }
    }
}
