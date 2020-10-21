using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.LogApi.Interfaces;
using TGS.Core.DataServices.Log;
using TGS.Core.DataServices.Log.Models;
using TGS.Core.Infrastructures.Log.Events;

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
    }
}
