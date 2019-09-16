// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
namespace AB.QuartzAdmin.WebApi.Models.Triggers.SchedulerDescriptors
{
    /// <summary>
    /// Enum to define the description "parts" of a Cron Expression  
    /// </summary>
    public enum DescriptionTypeEnum
    {
        /// <summary>
        /// 
        /// </summary>
        FULL,
        /// <summary>
        /// 
        /// </summary>
        TIMEOFDAY,
        /// <summary>
        /// 
        /// </summary>
        SECONDS,
        /// <summary>
        /// 
        /// </summary>
        MINUTES,
        /// <summary>
        /// 
        /// </summary>
        HOURS,
        /// <summary>
        /// 
        /// </summary>
        DAYOFWEEK,
        /// <summary>
        /// 
        /// </summary>
        MONTH,
        /// <summary>
        /// 
        /// </summary>
        DAYOFMONTH,
        /// <summary>
        /// 
        /// </summary>
        YEAR
    }
}
