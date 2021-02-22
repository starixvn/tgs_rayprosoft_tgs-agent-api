using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TGS.Apis.LogApi.Interfaces;
using TGS.Apis.LogApi.Services;
using TGS.Core.DataServices.Log;
using TGS.Core.DataServices.Log.Models;
using TGS.Core.Infrastructures.Log.Events;

namespace TGS.Apis.LogApi.Loggers
{
    public sealed class LogRecordWriter : ILoggerWriter
    {
        private readonly Func<ILogRepository> _repoFactory;
        private readonly IOptions<LoggerServiceConfig> _loggerServiceConfig;
        public LogRecordWriter(
            Func<ILogRepository> repoFactory,
            IOptions<LoggerServiceConfig> loggerServiceConfig)
        {
            _repoFactory = repoFactory;
            _loggerServiceConfig = loggerServiceConfig;
        }

        async Task ILoggerWriter.SaveLogsAsync(IList<LogBusEvent> events)
        {
            var config = _loggerServiceConfig.Value;

            var logs = events
                .Select(Convert)
                .Where(r => r.Level >= config.LogLevel ||
                            config.UnrestrictedLoggers.Contains(r.Logger))
                .ToArray();

            await using var repo = _repoFactory();
            await repo.AddLogRecordsAsync(logs);
        }

        async Task ILoggerWriter.SaveLogAsync(LogBusEvent @event)
        {
            var log = Convert(@event);
            await using var repo = _repoFactory();
            await repo.AddLogRecordAsync(log);
        }

        private static LogRecord Convert(LogBusEvent ent) => (LogRecord)ent;
    }
}
