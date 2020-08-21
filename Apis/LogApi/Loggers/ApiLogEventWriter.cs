using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.LogApi.DataService;
using TGS.Apis.LogApi.DataService.Models;
using TGS.Infrastructures.Log.Events;

namespace TGS.Apis.LogApi.Loggers
{
    public interface ILoggerEventWriter : IAsyncDisposable
    {
        Task SaveLogEventsAsync(IList<LogBusEvent> events);
        Task SaveLogEventAsync(LogBusEvent @event);
    }

    public sealed class ApiLogEventWriter : ILoggerEventWriter
    {
        private readonly ILogRepository _logRepository;

        public ApiLogEventWriter(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        Task ILoggerEventWriter.SaveLogEventsAsync(IList<LogBusEvent> events)
        {
            var logs = events
                .Select(Convert)
                .ToList();

            return _logRepository.AddApiLogsAsync(logs);
        }

        Task ILoggerEventWriter.SaveLogEventAsync(LogBusEvent @event)
        {
            var log = Convert(@event);
            return _logRepository.AddApiLogAsync(log);
        }

        private static ApiLog Convert(LogBusEvent ent) => (ApiLog)ent;

        ValueTask IAsyncDisposable.DisposeAsync()
        {
            return _logRepository.DisposeAsync();
        }
    }
}
