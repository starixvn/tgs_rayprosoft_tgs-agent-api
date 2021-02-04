using System.Linq;
using Microsoft.AspNetCore.Http;

namespace TGS.Core.Infrastructures.WebApiServices
{
    public interface IRequestAnalyzer
    {
        string GetIp(HttpContext context);
    }

    public sealed class RequestAnalyzer : IRequestAnalyzer
    {
        private readonly ILocalIpFinder _localIpFinder;
        private const string HttpForwardFor = "X-FORWARDED-FOR";
        private const string UnknownIp = "0.0.0.0";

        public RequestAnalyzer(ILocalIpFinder localIpFinder)
        {
            _localIpFinder = localIpFinder;
        }

        string IRequestAnalyzer.GetIp(HttpContext context)
        {
            if (context?.Request == null)
            {
                return UnknownIp;
            }

            var remoteIp = context.Connection.RemoteIpAddress;
            var userHostAddress = remoteIp.IsIPv4MappedToIPv6 ? remoteIp.MapToIPv4().ToString() : remoteIp.ToString();

            var forwardIps = context.Request.Headers[HttpForwardFor];

            if (forwardIps.Count == 0)
            {
                return userHostAddress;
            }

            return forwardIps.Select(e => e.Trim())
                       .FirstOrDefault(e =>
                       {
                           var ipi = _localIpFinder.GetIpInfo(e);
                           return ipi.IsValid && !ipi.IsLocal; // valid IP but not local
                       })
                   ?? userHostAddress;
        }
    }
}
