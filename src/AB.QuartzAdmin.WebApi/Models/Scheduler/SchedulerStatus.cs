namespace AB.QuartzAdmin.WebApi.Models.Scheduler
{
    /// <summary>
    /// Possible statuses a Quartz Net Scheduler can have.
    /// </summary>
    public enum SchedulerStatus
    {
        /// <summary>
        /// Unknown state.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Scheduler is running.
        /// </summary>
        Running = 1,
        /// <summary>
        /// Scheduler is paused / in standby.
        /// </summary>
        Standby = 2,
        /// <summary>
        /// Scheduler is shut down.
        /// </summary>
        Shutdown = 3
    }
}
