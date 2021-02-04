using System;

namespace TGS.Core.Infrastructures.Hangfire.Services
{
    public record JobDescription(Guid Id, string CronExpression, object Context, Type Worker)
    {
        public string Name => Worker.Name;
    }
}
