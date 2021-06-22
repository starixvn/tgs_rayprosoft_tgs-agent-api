using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Exceptions;

namespace TGS.Apis.TgsApi.GameProviders.AllBet.Middlewares
{
    public sealed class AuthorizeAllBetMiddleware : IMiddleware
    {
        private static readonly string _Routing = "/api/ab";

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var headers = context.Request.Headers;
            var authorization = GetHeaderValue(headers, "Authorization", string.Empty);

            if (authorization.IsNullOrEmpty())
            {
                throw new InvalidCredentialsException("authorization header is empty");
            }

            var items = authorization.Replace("AB ", string.Empty).Split(":");
            if (items.Length != 2)
            {
                throw new InvalidCredentialsException("invalid signature format");
            }

            var operatorId = items[0];
            var signature = items[1];

            var path = context.Request.Path.ToUriComponent();
            var idx = path.IndexOf(_Routing, StringComparison.OrdinalIgnoreCase);
            var apiPath = path[idx..].Replace(_Routing, string.Empty);

            var method = context.Request.Method;
            var md5 = GetHeaderValue(headers, "Content-MD5", string.Empty);
            var contentType = GetHeaderValue(headers, "Content-Type", string.Empty);
            var date = GetHeaderValue(headers, "Date", string.Empty);


            //var json = await context.Request.ReadAsStringAsync();
            //var doc = JsonDocument.Parse(json);
            await next(context);
        }

        private static string GetHeaderValue(IHeaderDictionary headers, string key, string defaultValue)
            => headers.TryGetValue(key, out var value) ? value.ToString() : defaultValue;
    }
}