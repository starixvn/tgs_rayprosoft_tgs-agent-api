using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.LogApi.DataService;
using TGS.Apis.LogApi.DataService.Models;
using TGS.Infrastructures.Log.Events;

namespace TGS.Apis.LogApi.Loggers
{
    public sealed class EventLogWriter : ILoggerEventWriter
    {
        private readonly ILogRepository _logRepository;

        public EventLogWriter(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        Task ILoggerEventWriter.SaveLogsAsync(IList<LogBusEvent> events)
        {
            var logs = events
                .Select(Convert)
                .ToList();

            return _logRepository.AddEventLogsAsync(logs);
        }

        Task ILoggerEventWriter.SaveLogAsync(LogBusEvent @event)
        {
            var log = Convert(@event);
            return _logRepository.AddEventLogAsync(log);
        }

        private static EventLog Convert(LogBusEvent ent) => (EventLog)ent;

        ValueTask IAsyncDisposable.DisposeAsync()
        {
            return _logRepository.DisposeAsync();
        }
    }
}
