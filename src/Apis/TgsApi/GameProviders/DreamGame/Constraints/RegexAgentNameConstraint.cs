using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TGS.Apis.TgsApi.GameProviders.DreamGame.Services;

namespace TGS.Apis.TgsApi.GameProviders.DreamGame.Constraints
{
    public class RegexAgentNameConstraint : IRouteConstraint
    {
        private readonly IDreamGameService _service;

        public RegexAgentNameConstraint(IDreamGameService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var agentName = values[routeKey].ToString();
            var configuration = _service.GetDreamGameConfigurationAsync().GetAwaiter().GetResult();
            return configuration.AgentName == agentName;
        }
    }
}
