using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.REST.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        /// <summary>
        /// Set the offset without changing the time 
        /// For example: 2020-01-01 18:00:00 GMT+06:00 -> 2020-01-01 18:00:00 GMT+02:00)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTimeOffset SetOffset(this DateTime date, string timezone)
        {
            var zone = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            var time = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone);

            var dpTime = new DateTimeOffset(DateTime.SpecifyKind(date, DateTimeKind.Unspecified), zone.GetUtcOffset(time));

            return dpTime;
        }
    }
}
