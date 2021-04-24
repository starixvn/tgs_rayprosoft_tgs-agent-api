using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Shared.Game.Models.JDB.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.JDB.Middlewares
{
    public sealed class AuthorizeJDBMiddleware : IMiddleware
    {
        private readonly IJsonSerializationProvider _json;

        public AuthorizeJDBMiddleware(
            IJsonSerializationProvider json)
        {
            _json = json;
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var jsonValue = context.Request.Form;
            
            await next(context);
        }
    }
}