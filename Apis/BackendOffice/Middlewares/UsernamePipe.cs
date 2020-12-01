using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using TGS.Services.BackOfficeApplication.Models;

namespace TGS.Apis.BackendOffice.Middlewares
{
    public class UsernamePipe<TIn, TOut> : IPipelineBehavior<TIn, TOut>
    {
        private readonly HttpContext _httpContext;

        public UsernamePipe(IHttpContextAccessor accessor)
        {
            _httpContext = accessor.HttpContext;
        }

        public async Task<TOut> Handle(
            TIn request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TOut> next)
        {
            if (request is BaseRequest br)
            {
                var userInfoMap = _httpContext.User.Claims.ToDictionary(p => p.Type, p => p.Value);
                br.Username = userInfoMap[Consts.UserInfo.UsernameKey];
            }

            var result = await next();

            return result;
        }
    }
}