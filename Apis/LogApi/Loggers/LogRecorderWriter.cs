using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TGS.Apis.LogApi.DataService;
using TGS.Apis.LogApi.DataService.Models;
using TGS.Apis.LogApi.Interfaces;
using TGS.Apis.LogApi.Services;
using TGS.Core.Infrastructures.Log.Events;

namespace TGS.Apis.LogApi.Loggers
{
    public sealed class LogRecordWriter : ILoggerWriter
    {
        private readonly ILogRepository _logRepository;
        private readonly IOptions<LoggerServiceConfig> _loggerServiceConfig;
        public LogRecordWriter(
            Func<ILogRepository> repoFactory,
            IOptions<LoggerServiceConfig> loggerServiceConfig)
        {
            _logRepository = repoFactory();
            _loggerServiceConfig = loggerServiceConfig;
        }

        Task ILoggerWriter.SaveLogsAsync(IList<LogBusEvent> events)
        {
            var config = _loggerServiceConfig.Value;

            var logs = events
                .Select(Convert)
                .Where(r => r.Level >= config.LogLevel ||
                            config.UnrestrictedLoggers.Contains(r.Logger))
                .ToArray();

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
