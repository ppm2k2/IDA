using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConceptONE.Infrastructure.Extensions
{
    public static class DateTimeExtensions
    {

        public static string ToStringSafe(this string dateString, string format)
        {
            DateTime date = DateTime.MinValue;
            string result = dateString ?? string.Empty;

            if (DateTime.TryParse(dateString, out date))
                result = date.ToString("MM/dd/yy");

            return result;
        }

        public static DateTime AddBusinessDays(this DateTime date, int days)
        {
            if (days == 0) return date;

            if (date.DayOfWeek == DayOfWeek.Saturday)
            {
                date = date.AddDays(2);
                days -= 1;
            }
            else if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                date = date.AddDays(1);
                days -= 1;
            }

            date = date.AddDays(days / 5 * 7);
            int extraDays = days % 5;

            if ((int)date.DayOfWeek + extraDays > 5)
            {
                extraDays += 2;
            }

            return date.AddDays(extraDays);

        }

        public static DateTime SubtractBusinessDays(this DateTime date, int days)
        {
            DateTime result = date;
            TimeSpan daySpan;
            int daysToSubtract = 0;

            while (days > 0)
            {
                if (result.DayOfWeek == DayOfWeek.Monday)
                    daysToSubtract = 3;
                else if (result.DayOfWeek == DayOfWeek.Sunday)
                    daysToSubtract = 2;
                else
                    daysToSubtract = 1;

                daySpan = new TimeSpan(daysToSubtract, 0, 0, 0);
                result = result.Subtract(daySpan);
                days--;
            }

            return result;
        }
        
        public static int GetBusinessDays(DateTime start, DateTime end)
        {
            if (start.DayOfWeek == DayOfWeek.Saturday)
            {
                start = start.AddDays(2);
            }
            else if (start.DayOfWeek == DayOfWeek.Sunday)
            {
                start = start.AddDays(1);
            }

            if (end.DayOfWeek == DayOfWeek.Saturday)
            {
                end = end.AddDays(-1);
            }
            else if (end.DayOfWeek == DayOfWeek.Sunday)
            {
                end = end.AddDays(-2);
            }

            int diff = (int)end.Subtract(start).TotalDays;

            int result = diff / 7 * 5 + diff % 7;

            if (end.DayOfWeek < start.DayOfWeek)
            {
                return result - 2;
            }
            else
            {
                return result;
            }
        }

        public static string DayOfWeekShort(this DateTime date)
        {
            string result = date.DayOfWeek.ToString().Substring(0, 3);
            return result;
        }

        public static bool Between(this DateTime input, DateTime fromDate, DateTime toDate)
        {
            return (input > fromDate && input < toDate);
        }

        public static bool BetweenInclusive(this DateTime input, DateTime fromDate, DateTime toDate)
        {
            return (input >= fromDate && input <= toDate);
        }

        public static bool IsWeekend(this DateTime date)
        {
            bool result = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
            return result;
        }


        /// <summary>
        /// Calculates number of business days, taking into account:
        ///  - weekends (Saturdays and Sundays)
        ///  - bank holidays in the middle of the week
        /// </summary>
        public static int BusinessDaysUntil(this DateTime firstDay, DateTime lastDay, params DateTime[] bankHolidays)
        {
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;

            if (firstDay > lastDay)
                return -1;

            TimeSpan span = lastDay - firstDay;
            int result = span.Days + 1;
            int fullWeekCount = result / 7;

            // find out if there are weekends during the time exceeding the full weeks
            if (result > fullWeekCount * 7)
            {
                // we are here to find out if there is a 1-day or 2-days weekend
                // in the time interval remaining after subtracting the complete weeks
                int firstDayOfWeek = (int)firstDay.DayOfWeek;
                int lastDayOfWeek = (int)lastDay.DayOfWeek;
                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;
                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 7)         // Both Saturday and Sunday are in the remaining time interval
                        result -= 2;
                    else if (lastDayOfWeek >= 6)    // Only Saturday is in the remaining time interval
                        result -= 1;
                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7) // Only Sunday is in the remaining time interval
                    result -= 1;
            }

            // subtract the weekends during the full weeks in the interval
            result -= fullWeekCount + fullWeekCount;

            // subtract the number of bank holidays during the time interval
            foreach (DateTime bankHoliday in bankHolidays)
            {
                DateTime bh = bankHoliday.Date;
                if (firstDay <= bh && bh <= lastDay)
                    --result;
            }

            return result;
        }

    }
}
