using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Http;
using TGS.Apis.OperatorApi.Models.Messages.Players;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models.Tokens;
using ContextItems = TGS.Core.Models.Consts.ContextItems;

namespace TGS.Apis.OperatorApi.Middlewares
{
    public sealed class DecryptPlayerTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITokenProvider _tokenProvider;

        public DecryptPlayerTokenMiddleware(RequestDelegate next, ITokenProvider tokenProvider)
        {
            _next = next;
            _tokenProvider = tokenProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = await context.Request.ReadFromJsonAsync<BaseTokenRequest>();

            if (request == null || request.PlayerToken.IsNullOrEmpty())
            {
                await _next(context);
                return;
            }

            var data = _tokenProvider.Decrypt<PlayerTokenData>(request.PlayerToken);
            context.Items.Add(ContextItems.PlayerToken, data);

            await _next(context);
        }
    }
}
