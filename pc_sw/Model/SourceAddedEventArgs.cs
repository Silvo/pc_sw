using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pc_sw.Model
{
    public class SourceAddedEventArgs : EventArgs
    {
        public DataSource Source { get; private set; }

        public SourceAddedEventArgs(DataSource source)
        {
            this.Source = source;
        }
    }
}
