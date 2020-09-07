using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.LogApi.DataService;
using TGS.Apis.LogApi.DataService.Models;
using TGS.Apis.LogApi.Interfaces;
using TGS.Infrastructures.Log.Events;

namespace TGS.Apis.LogApi.Loggers
{
    public sealed class AdminLogWriter : ILoggerWriter
    {
        private readonly ILogRepository _logRepository;

        public AdminLogWriter(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        Task ILoggerWriter.SaveLogsAsync(IList<LogBusEvent> events)
        {
            var logs = events
                .Select(Convert)
                .ToList();

            return _logRepository.AddAdminLogsAsync(logs);
        }

        Task ILoggerWriter.SaveLogAsync(LogBusEvent @event)
        {
            var log = Convert(@event);
            return _logRepository.AddAdminLogAsync(log);
        }

        private static AdminLog Convert(LogBusEvent ent) => (AdminLog)ent;
    }
}
