namespace TGS.Core.Infrastructures.Hangfire
{
    public record HangfireSettings
    {
        public string ConnectionString { get; init; }
        public string Prefix { get; set; }
        public string DbName { get; set; }
        public Schedule Schedule { get; set; }
    }

    public class Schedule
    {
        public int SchedulePollingInterval { get; set; }
    }
}