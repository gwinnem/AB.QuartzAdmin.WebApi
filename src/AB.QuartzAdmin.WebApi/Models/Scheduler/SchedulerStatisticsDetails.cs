using Quartz;

namespace AB.QuartzAdmin.WebApi.Models.Scheduler
{
    /// <summary>
    /// Statistic details for a <see cref="IScheduler"/>
    /// </summary>
    public sealed class SchedulerStatisticsDetails
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <param name="metaData">Metadata from a scheduler.</param>
        public SchedulerStatisticsDetails(SchedulerMetaData metaData)
        {
            NumberOfJobsExecuted = metaData.NumberOfJobsExecuted;
        }

        /// <summary>
        /// The number of jobs a scheduler has executed.
        /// </summary>
        public int NumberOfJobsExecuted { get; }
    }
}
