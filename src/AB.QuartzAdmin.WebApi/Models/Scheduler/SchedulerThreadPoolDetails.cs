using Quartz;
using Quartz.Util;
using System;

namespace AB.QuartzAdmin.WebApi.Models.Scheduler
{
    /// <summary>
    /// Model for the <see cref="IScheduler"/> Thread pool settings.
    /// </summary>
    [Serializable]
    public sealed class SchedulerThreadPoolDetails
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <param name="metaData">The <see cref="IScheduler"/> meta data.</param>
        public SchedulerThreadPoolDetails(SchedulerMetaData metaData)
        {
            Type = metaData.ThreadPoolType.AssemblyQualifiedNameWithoutVersion();
            Size = metaData.ThreadPoolSize;
        }

        /// <summary>
        /// Type of thread pool.
        /// </summary>
        public string Type { get; private set; }
        /// <summary>
        /// Number of threads in the pool.
        /// </summary>
        public int Size { get; private set; }
    }
}
