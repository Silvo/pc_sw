using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pc_sw.Helpers
{
    public static class MagicNumbers
    {
        public const Int16 LOW_WARNING_OFF_INT16 = Int16.MinValue;
        public const Double LOW_WARNING_OFF_DOUBLE = (Double)LOW_WARNING_OFF_INT16 / 10.0;
        public const Single LOW_WARNING_OFF_SINGLE = (Single)LOW_WARNING_OFF_DOUBLE;
        public const Int16 HIGH_WARNING_OFF_INT16 = Int16.MaxValue;
        public const Double HIGH_WARNING_OFF_DOUBLE = (Double)HIGH_WARNING_OFF_INT16 / 10.0;
        public const Single HIGH_WARNING_OFF_SINGLE = (Single)HIGH_WARNING_OFF_DOUBLE;
        public const Int32 FIRST_LOG_DATE = 1394841600; // 2014-03-15
    }
}
