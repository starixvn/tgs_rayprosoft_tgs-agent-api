using Microsoft.Extensions.Logging;

namespace TGS.Infrastructures.Log
{
    public interface IServiceLog<T>
    {
        void LogError(string message);
        void LogInformation(string message);
        void LogWarning(string message);
    }

    public sealed class ServiceLog<T> : IServiceLog<T>
    {
        private readonly ILogger<T> _logger;

        public ServiceLog(ILogger<T> logger)
        {
            _logger = logger;
        }

        void IServiceLog<T>.LogError(string message)
        {
            _logger.LogError(message);
        }

        void IServiceLog<T>.LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        void IServiceLog<T>.LogWarning(string message)
        {
            _logger.LogWarning(message);
        }
    }
}