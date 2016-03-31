using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.REST.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Set the timezone without changing the time 
        /// For example: 2020-01-01 18:00:00 GMT+06:00 -> 2020-01-01 18:00:00 GMT+02:00)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime SetTimeZone(this DateTime date, string timezone)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            var trTime = TimeZoneInfo.ConvertTimeToUtc(date.Date, timeZoneInfo).Add(date.TimeOfDay);

            return trTime;
        }

        /// <summary>
        /// Change timezone of the datetime
        /// For example: 2020-01-01 17:00:00 GMT+01:00 -> 2020-01-01 18:00:00 GMT+02:00)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime ChangeTimeZone(this DateTime date, string timezone)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            var trTime = TimeZoneInfo.ConvertTimeFromUtc(date.ToUniversalTime(), timeZoneInfo);

            return trTime;
        }
    }
}
