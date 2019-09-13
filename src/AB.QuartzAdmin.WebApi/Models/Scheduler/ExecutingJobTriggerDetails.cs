using System;
using System.Security.Cryptography;
using Quartz;

namespace AB.QuartzAdmin.WebApi.Models.Scheduler
{
    /// <summary>
    /// Data model for <see cref="IJob"/> <see cref="ITrigger"/> details.
    /// </summary>
    public sealed class ExecutingJobTriggerDetails
    {
        /// <summary>
        /// UTC Timestamp defining when the trigger starts firing the first time.
        /// </summary>
        public DateTimeOffset StartTimeUtc { get; set; }

        /// <summary>
        /// UTC Timestamp defining when the trigger stops firing.
        /// </summary>
        public DateTimeOffset? EndTimeUtc { get; set; }

        /// <summary>
        /// UTC Timestamp defining when the trigger was fired the last time.
        /// </summary>
        public DateTimeOffset? FinalFireTimeUtc { get; set; }

        /// <summary>
        /// Name of the group the trigger is configured under.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// The name of the trigger.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The description of the trigger.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The definition on how a scheduler should handle a misfire situation.
        /// For more details, see: https://www.quartz-scheduler.net/documentation/quartz-2.x/tutorial/more-about-triggers.html
        /// </summary>
        public int MisfireInstruction { get; set; }
        /// <summary>
        /// The internal priority a trigger has.
        /// For more details, see: https://www.quartz-scheduler.net/documentation/quartz-2.x/tutorial/more-about-triggers.html
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="context">The <see cref="IJobExecutionContext"/> a job is running within.</param>
        public ExecutingJobTriggerDetails(IJobExecutionContext context)
        {
            StartTimeUtc = context.Trigger.StartTimeUtc;
            EndTimeUtc = context.Trigger.EndTimeUtc;
            FinalFireTimeUtc = context.Trigger.FinalFireTimeUtc;
            Group = context.Trigger.Key.Group;
            Name = context.Trigger.Key.Name;
            Description = context.Trigger.Description;
            MisfireInstruction = context.Trigger.MisfireInstruction;
            Priority = context.Trigger.Priority;
        }
    }
}
