using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.LogApi.Interfaces;
using TGS.Apis.LogApi.Services;
using TGS.Core.DataServices.Log;
using TGS.Core.DataServices.Log.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log.Events;

namespace TGS.Apis.LogApi.Loggers
{
    public sealed class ApiLogWriter : ILoggerWriter
    {
        private readonly Func<ILogRepository> _repoFactory;
        private readonly IGetNotifyTaskService _service;
        public ApiLogWriter(Func<ILogRepository> repoFactory, IGetNotifyTaskService service)
        {
            _repoFactory = repoFactory;
            _service = service;
        }

        async Task ILoggerWriter.SaveLogsAsync(IList<LogBusEvent> events)
        {
            var logs = events
                .Select(Convert)
                .ToArray();

            await using var repo = _repoFactory();
            await repo.AddApiLogsAsync(logs);

            await logs.Where(p => p.ResponseCode != 200)
                .Select(log => _service.GetNotifyTaskAsync(log))
                .WhenAll();
        }

        async Task ILoggerWriter.SaveLogAsync(LogBusEvent @event)
        {
            var log = Convert(@event);

            await using var repo = _repoFactory();
            await repo.AddApiLogAsync(log);

            if (log.ResponseCode != 200)
                await _service.GetNotifyTaskAsync(log);
        }

        private static ApiLog Convert(LogBusEvent ent) => (ApiLog)ent;
    }
}
