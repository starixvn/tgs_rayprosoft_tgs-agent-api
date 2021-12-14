namespace TGS.Workers.Schedulers.Hangfire
{
    public record HangfireSettings
    {
        public string ConnectionString { get; init; }
        public string Prefix { get; init; }
        public Schedule Schedule { get; init; }
    }

    public record Schedule
    {
        public int SchedulePollingInterval { get; init; }
    }
}
