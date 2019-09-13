using Quartz;
using System;

namespace AB.QuartzAdmin.WebApi.Models.Scheduler
{
    /// <summary>
    /// Data model holding details for the <see cref="IJobExecutionContext"/> a <see cref="IJob"/> which is currently running in the Scheduler.
    /// </summary>
    public sealed class ExecutingJobContext
    {
        /// <summary>
        /// Id of the IJoContext the job is running under.
        /// </summary>
        public string FireInstanceId { get; set; }

        /// <summary>
        /// UTC Timestamp when the <see cref="IJob"/> was triggered.
        /// </summary>
        public DateTimeOffset FireTimeUtc { get; set; }

        /// <summary>
        /// UTC Timestamp when the <see cref="IJob"/> was scheduled for triggering.
        /// </summary>
        public DateTimeOffset? ScheduledFireTimeUtc { get; set; }

        /// <summary>
        /// UTC Timestamp when the <see cref="IJob"/> should be triggered next time.
        /// </summary>
        public DateTimeOffset? NextFireTimeUtc { get; set; }

        /// <summary>
        /// UTC Timestamp when the <see cref="IJob"/> was triggered last time.
        /// </summary>
        public DateTimeOffset? PreviousFireTimeUtc { get; set; }

        /// <summary>
        /// If a job “requests recovery”, and it is executing during the time of a ‘hard shutdown’ of the scheduler
        /// (i.e. the process it is running within crashes, or the machine is shut off),
        /// then it is re-executed when the scheduler is started again.
        /// In this case, the Recovering property will return true.
        /// </summary>
        public bool Recovering { get; set; }

        /// <summary>
        /// Defines how long the job has been running.
        /// </summary>
        public TimeSpan? JobRunTime { get; set; }

        /// <summary>
        /// The Job details.
        /// </summary>
        public ExecutingJobDetails JobDetails { get; set; }

        /// <summary>
        /// The Trigger details.
        /// </summary>
        public ExecutingJobTriggerDetails TriggerDetails { get; set; }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <param name="context">The <see cref="IJobExecutionContext"/> a job is running within.</param>
        public ExecutingJobContext(IJobExecutionContext context)
        {
            FireInstanceId = context.FireInstanceId;
            FireTimeUtc = context.FireTimeUtc;
            ScheduledFireTimeUtc = context.ScheduledFireTimeUtc;
            NextFireTimeUtc = context.NextFireTimeUtc;
            PreviousFireTimeUtc = context.PreviousFireTimeUtc;
            Recovering = context.Recovering;
            JobRunTime = context.JobRunTime;

            JobDetails = new ExecutingJobDetails(context);
            TriggerDetails = new ExecutingJobTriggerDetails(context);
        }
    }
}
