using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TGS.Apis.TgsApi.GameProviders.Habanero.Middlewares
{
    public sealed class AuthorizeHabaneroMiddleware : IMiddleware
    {
        Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            throw new NotImplementedException();
        }
    }
}
