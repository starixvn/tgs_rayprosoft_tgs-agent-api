using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.LogApi.Interfaces;
using TGS.Core.DataServices.Log;
using TGS.Core.DataServices.Log.Models;
using TGS.Core.Infrastructures.Log.Events;

namespace TGS.Apis.LogApi.Loggers
{
    public sealed class AdminLogWriter : ILoggerWriter
    {
        private readonly Func<ILogRepository> _repoFactory;

        public AdminLogWriter(Func<ILogRepository> repoFactory)
        {
            _repoFactory = repoFactory;
        }

        async Task ILoggerWriter.SaveLogsAsync(IList<LogBusEvent> events)
        {
            var logs = events
                .Select(Convert)
                .ToArray();

            await using var repo = _repoFactory();
            await repo.AddAdminLogsAsync(logs);
        }

        async Task ILoggerWriter.SaveLogAsync(LogBusEvent @event)
        {
            var log = Convert(@event);
            await using var repo = _repoFactory();
            await repo.AddAdminLogAsync(log);
        }

        private static AdminLog Convert(LogBusEvent ent) => (AdminLog)ent;
    }
}
