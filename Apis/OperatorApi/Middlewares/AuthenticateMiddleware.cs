using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Models;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Security;

namespace TGS.Apis.OperatorApi.Middlewares
{
    public class AuthenticateMiddleware
    {
        private readonly IHashProvider _hashProvider;
        private readonly ITgsQueries<Licensee> _licenseeQueries;
        private readonly RequestDelegate _next;

        public AuthenticateMiddleware(IHashProvider hashProvider, ITgsQueries<Licensee> licenseeQueries, RequestDelegate next)
        {
            _hashProvider = hashProvider;
            _licenseeQueries = licenseeQueries;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!HttpMethods.IsPost(context.Request.Method))
            {
                await _next(context);
                return;
            }

            var clientId = context.Request.Headers["clientid"];
            var time = context.Request.Headers["time"];

            var licensee = await _licenseeQueries.GetSingleAsync(p => p.ClientId == clientId);
            var authCode = context.Request.Headers["authcode"];

           
            await _next(context);
        }
    }
}
