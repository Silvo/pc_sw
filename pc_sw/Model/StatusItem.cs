using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pc_sw.Model
{
    public class StatusItem
    {
        public StatusVisualization Visual { get; set; }
        public Int32 TimeToRemoval { get; set; }

        public StatusItem(StatusVisualization visual, Int32 timeToRemoval)
        {
            this.Visual = visual;
            this.TimeToRemoval = timeToRemoval;
        }
    }
}
