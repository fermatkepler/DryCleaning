using System.Runtime.Serialization;

namespace DryCleaning.Domain
{
    public class Scheduler
    {
        public OpenedInterval? NormalOpen { get; set; }
        public Dictionary<DayOfWeek, OpenedInterval>? WeekDayOpen { get; set; }
        public Dictionary<DateOnly, OpenedInterval>? YearDayOpen { get; set; }
        public List<DayOfWeek>? WeekDaysClose { get; set; }
        public List<DateOnly>? YearDaysClose { get; set; }

    }
}
