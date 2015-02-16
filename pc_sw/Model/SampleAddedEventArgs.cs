using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pc_sw.Model
{
    public class SampleAddedEventArgs : EventArgs
    {
        public Tuple<DateTime, float> Sample { get; private set; }

        public SampleAddedEventArgs(Tuple<DateTime, float> sample)
        {
            this.Sample = sample;
        }
    }
}
