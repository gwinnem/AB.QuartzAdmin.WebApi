using Quartz;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AB.QuartzAdmin.WebApi.Models.Scheduler
{
    /// <summary>
    /// Data model for the scheduler instance details.
    /// </summary>
    [Serializable]
    public sealed class SchedulerDetails
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="scheduler">The <see cref="IScheduler"/>  instance.</param>
        /// <param name="metaData">The <see cref="SchedulerMetaData"/> meta data.</param>
        public SchedulerDetails(IScheduler scheduler, SchedulerMetaData metaData)
        {
            Name = scheduler.SchedulerName;
            SchedulerInstanceId = scheduler.SchedulerInstanceId;
            Status = TranslateStatus(scheduler);
            RunningSince = metaData.RunningSince?.LocalDateTime.ToString(CultureInfo.InvariantCulture) ?? "N / A";
            QuartzVersion = metaData.Version;
            ThreadPool = new SchedulerThreadPoolDetails(metaData);
            JobStore = new SchedulerJobStoreDetails(metaData);
            Statistics = new SchedulerStatisticsDetails(metaData);
            JobKeys = scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup()).GetAwaiter().GetResult();
            TriggerKeys = scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup()).GetAwaiter().GetResult();
            GetJobTriggerPausedGroups(scheduler).GetAwaiter().GetResult();
        }

        /// <summary>
        /// List of all triggers configured for the scheduler.
        /// </summary>
        public IReadOnlyCollection<TriggerKey> TriggerKeys { get; set; }
        /// <summary>
        /// List of all jobs configured for the scheduler.
        /// </summary>
        public IReadOnlyCollection<JobKey> JobKeys { get; set; }
        /// <summary>
        /// List of all paused job groups.
        /// </summary>
        public IEnumerable<object> PausedJobGroups { get; set; }
        /// <summary>
        /// List of all paused trigger groups.
        /// </summary>
        public IEnumerable<object> PausedTriggerGroups { get; set; }

        /// <summary>
        /// Instance id for the scheduler.
        /// </summary>
        public string SchedulerInstanceId { get; }
        /// <summary>
        /// Name of the scheduler.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Scheduler status.
        /// </summary>
        public SchedulerStatus Status { get; }
        /// <summary>
        /// Date stamp when scheduler started.
        /// </summary>
        public string RunningSince { get; }
        /// <summary>
        /// Scheduler version.
        /// </summary>
        public string QuartzVersion { get; }
        /// <summary>
        /// Number of jobs configured in the scheduler.
        /// </summary>
        public int NumberOfJobs => JobKeys.Count;
        /// <summary>
        /// Number of triggers configured in the scheduler.
        /// </summary>
        public int NumberOfTriggers => TriggerKeys.Count;

        /// <summary>
        /// Scheduler Thread Pool details.
        /// </summary>
        public SchedulerThreadPoolDetails ThreadPool { get; }
        /// <summary>
        /// Scheduler JobStore details.
        /// </summary>
        public SchedulerJobStoreDetails JobStore { get; }
        /// <summary>
        /// Scheduler Statistics.
        /// </summary>
        public SchedulerStatisticsDetails Statistics { get; }


        #region Private helpers
        public static SchedulerStatus TranslateStatus(IScheduler scheduler)
        {
            if (scheduler.IsShutdown)
            {
                return SchedulerStatus.Shutdown;
            }
            if (scheduler.InStandbyMode)
            {
                return SchedulerStatus.Standby;
            }

            return scheduler.IsStarted
                ? SchedulerStatus.Running
                : SchedulerStatus.Unknown;
        }

        private async Task GetJobTriggerPausedGroups(IScheduler scheduler)
        {
            try
            {
                PausedJobGroups = await GetGroupPauseState(
                    await scheduler.GetJobGroupNames(),
                    async x => await scheduler.IsJobGroupPaused(x));
            }
            catch (NotImplementedException) { }

            try
            {
                PausedTriggerGroups = await GetGroupPauseState(
                    await scheduler.GetTriggerGroupNames(),
                    async x => await scheduler.IsTriggerGroupPaused(x));
            }
            catch (NotImplementedException) { }
        }

        private static async Task<IEnumerable<object>> GetGroupPauseState(IEnumerable<string> groups, Func<string, Task<bool>> func)
        {
            var result = new List<object>();

            foreach (var name in groups.OrderBy(x => x, StringComparer.InvariantCultureIgnoreCase))
                result.Add(new { Name = name, IsPaused = await func(name) });

            return result;
        }
        #endregion
    }
}
