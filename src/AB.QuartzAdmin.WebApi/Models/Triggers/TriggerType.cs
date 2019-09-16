namespace AB.QuartzAdmin.WebApi.Models.Triggers
{
    /// <summary>
    /// Defines the different trigger types.
    /// </summary>
    public enum TriggerType
    {
        Unknown = 0,
        Simple = 1,
        Daily = 2,
        Cron = 3,
        Calendar = 4,
    }
}
