using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.GameProviders.Skywind.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;

namespace TGS.Apis.TgsApi.GameProviders.TCG.Middlewares
{
    public sealed class AuthorizeTCGMiddleware : IMiddleware
    {

        public AuthorizeTCGMiddleware(
            )
        {

        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {

            await next(context);
        }
    }
}
