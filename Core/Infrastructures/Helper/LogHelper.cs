using Microsoft.Extensions.Logging;

namespace TGS.Core.Infrastructures.Helper
{
    public static class LogHelper
    {
        private static ILogger<T> CreateLogger<T>(ILoggerFactory factory)
        {
            return new Logger<T>(factory);
        }
    }
}
