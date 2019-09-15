using Quartz;

namespace AB.QuartzAdmin.WebApi.Models.Jobs
{
    /// <summary>
    /// Data model holding details for a <see cref="IJob"/>
    /// </summary>
    public sealed class JobDetails
    {
        /// <summary>
        /// The name of the job.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The job group.
        /// </summary>
        public string Group { get; set; }
        /// <summary>
        /// Fully qualified name of the job type.
        /// </summary>
        public string JobType { get; set; }
        /// <summary>
        /// The job description.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// If a job is non-durable, it is automatically deleted from the scheduler once there are no longer any active triggers associated with it.
        /// In other words, non-durable jobs have a life span bounded by the existence of its triggers.
        /// </summary>
        public bool Durable { get; set; }
        /// <summary>
        /// If a job “requests recovery”, and it is executing during the time of a ‘hard shutdown’ of the scheduler
        /// (i.e. the process it is running within crashes, or the machine is shut off), then it is re-executed when the scheduler is started again.
        /// In this case, the JobExecutionContext.Recovering property will return true.
        /// </summary>
        public bool RequestsRecovery { get; set; }
    }
}
