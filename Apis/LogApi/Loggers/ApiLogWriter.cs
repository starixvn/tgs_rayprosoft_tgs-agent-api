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
        Task SaveLogsAsync(IList<LogBusEvent> events);
        Task SaveLogAsync(LogBusEvent @event);
    }

    public sealed class ApiLogWriter : ILoggerEventWriter
    {
        private readonly ILogRepository _logRepository;

        public ApiLogWriter(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        Task ILoggerEventWriter.SaveLogsAsync(IList<LogBusEvent> events)
        {
            var logs = events
                .Select(Convert)
                .ToList();

            return _logRepository.AddApiLogsAsync(logs);
        }

        Task ILoggerEventWriter.SaveLogAsync(LogBusEvent @event)
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
