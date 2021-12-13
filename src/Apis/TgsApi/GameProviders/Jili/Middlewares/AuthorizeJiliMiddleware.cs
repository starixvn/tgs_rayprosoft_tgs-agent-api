using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.GameProviders.Jili.Services;
using Tgs.Core.Infrastructures.Security;
using TGS.Services.Operations;

namespace TGS.Apis.TgsApi.GameProviders.Jili.Middlewares
{
    public class AuthorizeJiliMiddleware : IMiddleware
    {
        public AuthorizeJiliMiddleware(
            IJiliService service,
            IPlayerOperations playerOperations,
            ITokenProvider tokenProvider)
        {
        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            return default;
        }
    }
}
