using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.LogApi.DataService;
using TGS.Apis.LogApi.DataService.Models;
using TGS.Apis.LogApi.Interfaces;
using TGS.Infrastructures.Log.Events;

namespace TGS.Apis.LogApi.Loggers
{
    public sealed class ApiLogWriter : ILoggerWriter
    {
        private readonly ILogRepository _logRepository;

        public ApiLogWriter(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        Task ILoggerWriter.SaveLogsAsync(IList<LogBusEvent> events)
        {
            var logs = events
                .Select(Convert)
                .ToList();

            return _logRepository.AddApiLogsAsync(logs);
        }

        Task ILoggerWriter.SaveLogAsync(LogBusEvent @event)
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
