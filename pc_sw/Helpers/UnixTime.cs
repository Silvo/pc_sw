using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace pc_sw.Helpers
{
    public static class UnixTime
    {
        public static DateTime ToDateTime(Int32 unixTime)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTime);
        }

        public static int FromDateTime(DateTime dateTime)
        {
            return (Int32)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        public static String ToString(Int32 unixTime)
        {
            Int32 timeOffset = (Int32)TimeZoneInfo.Local.GetUtcOffset(
                UnixTime.ToDateTime(unixTime)).TotalSeconds;

            return ToDateTime(unixTime + timeOffset).ToString("d MMM yyyy HH:mm:ss");
        }
    }
}
