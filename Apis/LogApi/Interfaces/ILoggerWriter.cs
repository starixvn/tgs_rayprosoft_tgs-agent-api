using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.Infrastructures.Log.Events;

namespace TGS.Apis.LogApi.Interfaces
{
    public interface ILoggerWriter : IAsyncDisposable
    {
        Task SaveLogsAsync(IList<LogBusEvent> events);
        Task SaveLogAsync(LogBusEvent @event);
    }
}
