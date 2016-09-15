﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BahamutCommon
{
    public class DateTimeUtil
    {
        public static TimeSpan UnixTimeSpan
        {
            get
            {
                return DateTime.UtcNow - DateTime.Parse("01/01/1970");
            }
        }

        public static string ToAccurateDateTimeString(DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        public static string ToString(DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static DateTime ToDate(string dateString)
        {
            try
            {
                var time = DateTime.Parse(dateString);
                time = new DateTime(time.Ticks, DateTimeKind.Utc);
                return time;
            }
            catch (Exception)
            {
                return new DateTime(0);
            }
            
        }
    }
}
