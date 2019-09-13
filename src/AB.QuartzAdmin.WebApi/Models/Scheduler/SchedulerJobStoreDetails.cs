using Quartz;
using Quartz.Util;

namespace AB.QuartzAdmin.WebApi.Models.Scheduler
{
    /// <summary>
    /// Model for Scheduler Job store details.
    /// </summary>
    public sealed class SchedulerJobStoreDetails
    {
        /// <summary>
        /// Statistic details for a <see cref="IScheduler"/>
        /// </summary>
        public SchedulerJobStoreDetails(SchedulerMetaData metaData)
        {
            Type = metaData.JobStoreType.AssemblyQualifiedNameWithoutVersion();
            Clustered = metaData.JobStoreClustered;
            Persistent = metaData.JobStoreSupportsPersistence;
        }

        /// <summary>
        /// Job Store Type.
        /// </summary>
        public string Type { get; }
        /// <summary>
        /// Is Job Stored clustered.
        /// </summary>
        public bool Clustered { get; }
        /// <summary>
        /// Is Job Store Persistent
        /// </summary>
        public bool Persistent { get; }
    }
}
