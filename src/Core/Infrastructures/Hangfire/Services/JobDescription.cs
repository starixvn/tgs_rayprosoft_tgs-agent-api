using System;

namespace TGS.Core.Infrastructures.Hangfire.Services
{
    public record JobDescription
    {
        public JobDescription(Guid id, string cronExpression, object context, Type worker)
            : this(id, cronExpression, context, worker, TimeZoneInfo.Utc)
        {
        }

        public JobDescription(Guid id, string cronExpression, object context, Type worker, TimeZoneInfo timeZone)
        {
            Id = id;
            CronExpression = cronExpression;
            Context = context;
            Worker = worker;
            TimeZone = timeZone;
        }

        public Guid Id { get; init; }
        public string CronExpression { get; init; }
        public object Context { get; init; }
        public Type Worker { get; init; }
        public TimeZoneInfo TimeZone { get; init; }
        public string Name => Worker.Name;
    }
}