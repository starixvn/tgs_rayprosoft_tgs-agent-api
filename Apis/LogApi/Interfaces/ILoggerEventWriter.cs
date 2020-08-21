using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.Infrastructures.Log.Events;

namespace TGS.Apis.LogApi.Interfaces
{
    public interface ILoggerEventWriter : IDisposable
    {
        Task SaveLogEventsAsync(IList<LogBusEvent> events);
        Task SaveLogEventAsync(LogBusEvent @event);
    }
}
