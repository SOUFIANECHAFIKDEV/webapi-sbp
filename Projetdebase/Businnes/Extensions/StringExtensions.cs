using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Extensions
{
    public static class StringExtensions
    {
            public static List<DateTime> GetWeekEnd(this DateTime dateStart, DateTime dateEnd)
            {
                var weekEnd = new List<DateTime>();
                var end = new DateTime(dateEnd.Year, dateEnd.Month, dateEnd.Day, 00, 00, 00);
                var start = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 00, 00, 00);
                while (DateTime.Compare(start, end) <= 0)
                {
                    if (start.DayOfWeek == DayOfWeek.Saturday || start.DayOfWeek == DayOfWeek.Sunday)
                    {
                        weekEnd.Add(start);
                    }
                    start = start.AddDays(1);
                }
                return weekEnd;
            }
    }
}
