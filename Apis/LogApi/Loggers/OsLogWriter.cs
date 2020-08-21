using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.LogApi.DataService;
using TGS.Apis.LogApi.DataService.Models;
using TGS.Infrastructures.Log.Events;

namespace TGS.Apis.LogApi.Loggers
{
    public sealed class OsLogWriter : ILoggerEventWriter
    {
        private readonly ILogRepository _logRepository;

        public OsLogWriter(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        Task ILoggerEventWriter.SaveLogsAsync(IList<LogBusEvent> events)
        {
            var logs = events
                .Select(Convert)
                .ToList();

            return _logRepository.AddOsLogsAsync(logs);
        }

        Task ILoggerEventWriter.SaveLogAsync(LogBusEvent @event)
        {
            var log = Convert(@event);
            return _logRepository.AddOsLogAsync(log);
        }

        private static OsLog Convert(LogBusEvent ent) => (OsLog)ent;

        ValueTask IAsyncDisposable.DisposeAsync()
        {
            return _logRepository.DisposeAsync();
        }
    }
}
