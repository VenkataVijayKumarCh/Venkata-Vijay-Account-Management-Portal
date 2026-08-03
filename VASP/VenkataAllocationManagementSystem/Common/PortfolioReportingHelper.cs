using System.Collections.Generic;
using System.Linq;

namespace VenkataAllocationManagementSystem.Common
{
    public static class PortfolioReportingHelper
    {
        public static IEnumerable<DateOnly> GetDatesInRange(DateOnly startDate, DateOnly endDate)
        {
            for (var current = startDate; current <= endDate; current = current.AddDays(1))
            {
                yield return current;
            }
        }

        public static bool IsWeekend(DateOnly date) => date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

        public static int CountWorkingDays(DateOnly startDate, DateOnly endDate, IReadOnlyCollection<DateOnly> holidays, IReadOnlyCollection<DateOnly> leaveDates)
        {
            return GetDatesInRange(startDate, endDate)
                .Count(date => !IsWeekend(date) && !holidays.Contains(date) && !leaveDates.Contains(date));
        }
    }
}
