using System;

namespace LumiGame.Script.Utility
{
    public static class DateTimeUtil
    {
        public static long GetCurrentMs(this DateTime dateTime)
        {
            return dateTime.Ticks / 10000;
        }

        public static string ToHourTimeString(this double timeInMs)
        {
            var timeSpan = TimeSpan.FromMilliseconds(timeInMs);
            
            return string.Format ("{0:00}:{1:00}:{2:00}", 
                (int)timeSpan.TotalHours, 
                timeSpan.Minutes, 
                timeSpan.Seconds);
        }
    }
}