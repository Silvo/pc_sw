using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pc_sw.Helpers
{
    public class StatusChangedMessage
    {
        public String Text { get; set; }

        public StatusChangedMessage(String text)
        {
            this.Text = text;
        }
    }
}
