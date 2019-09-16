namespace AB.QuartzAdmin.WebApi.Models.Triggers.SchedulerDescriptors
{
    /// <summary>
    /// Options for parsing and describing a Cron Expression
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Options()
        {
            ThrowExceptionOnParseError = true;
            Verbose = false;
            DayOfWeekStartIndexZero = true;
            Use24HourTimeFormat = true;
        }
        /// <summary>
        /// 
        /// </summary>
        public bool ThrowExceptionOnParseError { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Verbose { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool DayOfWeekStartIndexZero { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? Use24HourTimeFormat { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Locale { get; set; }
    }
}
