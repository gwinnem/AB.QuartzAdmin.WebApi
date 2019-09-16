namespace AB.QuartzAdmin.WebApi.Models.Triggers
{
    /// <summary>
    /// Data model for a trigger list item.
    /// </summary>
    public sealed class TriggerListItem
    {
        public string TriggerName { get; set; }
        public string TriggerGroup { get; set; }
        public bool IsPaused { get; set; }
        public TriggerType Type { get; set; }
        public string Description { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string LastFireTime { get; set; }
        public string NextFireTime { get; set; }
        public string ScheduleDescription { get; set; }
    }
}
