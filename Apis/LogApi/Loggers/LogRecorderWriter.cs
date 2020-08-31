using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TGS.Apis.LogApi.DataService;
using TGS.Apis.LogApi.DataService.Models;
using TGS.Apis.LogApi.Interfaces;
using TGS.Apis.LogApi.Services;
using TGS.Infrastructures.Log.Events;

namespace TGS.Apis.LogApi.Loggers
{
    public sealed class LogRecordWriter : ILoggerWriter
    {
        private readonly ILogRepository _logRepository;
        private readonly IOptions<LoggerServiceConfig> _loggerServiceConfig;
        public LogRecordWriter(
            ILogRepository ugsRepository,
            IOptions<LoggerServiceConfig> loggerServiceConfig)
        {
            _logRepository = ugsRepository;
            _loggerServiceConfig = loggerServiceConfig;
        }

        Task ILoggerWriter.SaveLogsAsync(IList<LogBusEvent> events)
        {
            var logs = events
                .Select(Convert)
                .Where(r => r.Level >= _loggerServiceConfig.Value.LogLevel ||
                            _loggerServiceConfig.Value.UnrestrictedLoggers.Contains(r.Logger))
                .ToList();

            return _logRepository.AddLogRecordsAsync(logs);
        }

        Task ILoggerWriter.SaveLogAsync(LogBusEvent @event)
        {
            var log = Convert(@event);
            return _logRepository.AddLogRecordAsync(log);
        }

        private static LogRecord Convert(LogBusEvent ent) => (LogRecord)ent;
    }
}
