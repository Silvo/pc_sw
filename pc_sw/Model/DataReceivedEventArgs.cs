using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pc_sw.Model
{
    public class DataReceivedEventArgs : EventArgs
    {
        public SensorDataSample DataSample { get; private set; }

        public DataReceivedEventArgs(SensorDataSample dataSample)
        {
            this.DataSample = dataSample;
        }
    }
}
