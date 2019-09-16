using AB.QuartzAdmin.WebApi.Models.Triggers.SchedulerDescriptors;
using Quartz;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AB.QuartzAdmin.WebApi.Models.Triggers
{
    public static class TriggerExtensions
    {
        /// <summary>
        /// Extension for getting trigger type from a <see cref="ITrigger"/> instance. 
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        public static TriggerType GetTriggerType(this ITrigger trigger)
        {
            switch(trigger)
            {
                case ICronTrigger _:
                    return TriggerType.Cron;
                case IDailyTimeIntervalTrigger _:
                    return TriggerType.Daily;
                case ISimpleTrigger _:
                    return TriggerType.Simple;
                case ICalendarIntervalTrigger _:
                    return TriggerType.Calendar;
                default:
                    return TriggerType.Unknown;
            }
        }

        public static string GetScheduleDescription(this ITrigger trigger)
        {
            if(trigger is ICronTrigger cr)
                return ExpressionDescriptor.GetDescription(cr.CronExpressionString);
            if(trigger is IDailyTimeIntervalTrigger dt)
                return GetScheduleDescription(dt);
            if(trigger is ISimpleTrigger st)
                return GetScheduleDescription(st);
            if(trigger is ICalendarIntervalTrigger ct)
                return GetScheduleDescription(ct.RepeatInterval, ct.RepeatIntervalUnit);

            return null;
        }

        public static string GetScheduleDescription(this IDailyTimeIntervalTrigger trigger)
        {
            var result = GetScheduleDescription(trigger.RepeatInterval, trigger.RepeatIntervalUnit, trigger.RepeatCount);
            result += " from " + trigger.StartTimeOfDay.ToShortFormat() + " to " + trigger.EndTimeOfDay.ToShortFormat();

            if(trigger.DaysOfWeek.Count < 7)
            {
                var dow = DaysOfWeekViewModel.Create(trigger.DaysOfWeek);

                if(dow.AreOnlyWeekdaysEnabled)
                    result += " only on Weekdays";
                else if(dow.AreOnlyWeekendEnabled)
                    result += " only on Weekends";
                else
                    result += " on " + string.Join(", ", trigger.DaysOfWeek);
            }

            return result;
        }

        public static string GetScheduleDescription(this ISimpleTrigger trigger)
        {
            var result = "Repeat ";
            if(trigger.RepeatCount > 0)
                result += trigger.RepeatCount + " times ";
            result += "every ";

            var diff = trigger.RepeatInterval.TotalMilliseconds;

            var messagesParts = new List<string>();
            foreach(var part in TimespanPart.Items)
            {
                var currentPartValue = Math.Floor(diff / part.Multiplier);
                diff -= currentPartValue * part.Multiplier;

                if(currentPartValue == 1)
                    messagesParts.Add(part.Singular);
                else if(currentPartValue > 1)
                    messagesParts.Add(currentPartValue + " " + part.Plural);
            }

            result += string.Join(", ", messagesParts);

            return result;
        }

        public static string GetScheduleDescription(int repeatInterval, IntervalUnit repeatIntervalUnit, int repeatCount = 0)
        {
            var result = "Repeat ";
            if(repeatCount > 0)
                result += repeatCount + " times ";
            result += "every ";

            var unitStr = repeatIntervalUnit.ToString().ToLower();

            if(repeatInterval == 1)
                result += unitStr;
            else
                result += repeatInterval + " " + unitStr + "s";

            return result;
        }




        private class TimespanPart
        {
            public static readonly TimespanPart[] Items = new[]
            {
            new TimespanPart("day", 1000 * 60 * 60 * 24),
            new TimespanPart("hour", 1000 * 60 * 60),
            new TimespanPart("minute", 1000 * 60),
            new TimespanPart("second", 1000),
            new TimespanPart("millisecond", 1),
        };

            public string Singular { get; set; }
            public string Plural { get; set; }
            public long Multiplier { get; set; }

            public TimespanPart(string singular, long multiplier) : this(singular)
            {
                Multiplier = multiplier;
            }
            public TimespanPart(string singular)
            {
                Singular = singular;
                Plural = singular + "s";
            }
        }

        public static string ToShortFormat(this TimeOfDay timeOfDay)
        {
            return timeOfDay.ToTimeSpan().ToString("g", CultureInfo.InvariantCulture);
        }

        public static TimeSpan ToTimeSpan(this TimeOfDay timeOfDay)
        {
            return TimeSpan.FromSeconds(timeOfDay.Second + timeOfDay.Minute * 60 + timeOfDay.Hour * 3600);
        }

        public class DaysOfWeekViewModel
        {
            public bool Monday { get; set; }
            public bool Tuesday { get; set; }
            public bool Wednesday { get; set; }
            public bool Thursday { get; set; }
            public bool Friday { get; set; }
            public bool Saturday { get; set; }
            public bool Sunday { get; set; }

            public void AllOn()
            {
                Monday = true;
                Tuesday = true;
                Wednesday = true;
                Thursday = true;
                Friday = true;
                Saturday = true;
                Sunday = true;
            }

            public static DaysOfWeekViewModel Create(IEnumerable<DayOfWeek> list)
            {
                var model = new DaysOfWeekViewModel();
                foreach(var item in list)
                {
                    if(item == DayOfWeek.Sunday)
                        model.Sunday = true;
                    if(item == DayOfWeek.Monday)
                        model.Monday = true;
                    if(item == DayOfWeek.Tuesday)
                        model.Tuesday = true;
                    if(item == DayOfWeek.Wednesday)
                        model.Wednesday = true;
                    if(item == DayOfWeek.Thursday)
                        model.Thursday = true;
                    if(item == DayOfWeek.Friday)
                        model.Friday = true;
                    if(item == DayOfWeek.Saturday)
                        model.Saturday = true;
                }
                return model;
            }

            public IEnumerable<DayOfWeek> GetSelected()
            {
                if(Monday) yield return DayOfWeek.Monday;
                if(Tuesday) yield return DayOfWeek.Tuesday;
                if(Wednesday) yield return DayOfWeek.Wednesday;
                if(Thursday) yield return DayOfWeek.Thursday;
                if(Friday) yield return DayOfWeek.Friday;
                if(Saturday) yield return DayOfWeek.Saturday;
                if(Sunday) yield return DayOfWeek.Sunday;
            }

            public bool AreOnlyWeekendEnabled => !Monday && !Tuesday && !Wednesday && !Thursday && !Friday && Saturday && Sunday;
            public bool AreOnlyWeekdaysEnabled => Monday && Tuesday && Wednesday && Thursday && Friday && !Saturday && !Sunday;
        }

    }
}
