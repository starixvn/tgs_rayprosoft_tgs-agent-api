using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.GameProviders.AllBet.Exceptions;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;

namespace TGS.Apis.TgsApi.GameProviders.AllBet.Middlewares
{
    public sealed class AuthorizeAllBetMiddleware : IMiddleware
    {
        private static readonly string _Routing = "/api/ab";
        private readonly ISettingOperations _settingOperations;

        public AuthorizeAllBetMiddleware(
            ISettingOperations settingOperations)
        {
            _settingOperations = settingOperations;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var headers = context.Request.Headers;
            var (propertyId, signature) = GetPropertyIdAndSignature(headers);

            var configuration = await _settingOperations.GetValueFromJsonByKeyAsync<AllBetConfiguration>(
                SettingConsts.GameProviderConfigurations.AllBet);

            var agent = configuration.BrandMerchants.SelectMany(m => m.Agents)
                .FirstOrDefault(a => a.PropertyId.Equals(propertyId));

            if (agent == null)
            {
                throw new InvalidOperatorIdException();
            }

            var path = GetEndPath(context.Request.Path);
            var bypassSignatureVerify = bool.Parse(GetHeaderValue(headers, "BypassSignatureVerify", bool.FalseString));

            if (!bypassSignatureVerify)
            {
                var computedSignature = ComputeSignature(headers, context.Request.Method, path, agent.PartnerApiKey);

                if (signature.NotEquals(computedSignature))
                {
                    throw new InvalidCredentialsException(
                        $"Signature is mismatched. The signature in the request is {signature}, which is different from {computedSignature} as computed.");
                }
            }

            context.Items.Add(AllBetConsts.ContextItems.Agent, agent);

            if (path.Contains("Transfer"))
            {
                //var json = await context.Request.ReadAsStringAsync();
                //var doc = JsonDocument.Parse(json);
            }

            await next(context);
        }

        private static (string PropertyId, string Signature) GetPropertyIdAndSignature(IHeaderDictionary headers)
        {
            var authorization = GetHeaderValue(headers, "Authorization", string.Empty);

            if (authorization.IsNullOrEmpty())
            {
                throw new InvalidCredentialsException("Authorization header is empty");
            }

            var items = authorization.Replace("AB ", string.Empty).Split(":");
            if (items.Length != 2)
            {
                throw new InvalidCredentialsException("Invalid signature format");
            }

            return (items[0], items[1]);
        }

        private static string ComputeSignature(IHeaderDictionary headers, string method, string path,
            string partnerApiKey)
        {
            var md5 = GetHeaderValue(headers, "Content-MD5", string.Empty);
            var contentType = GetHeaderValue(headers, "Content-Type", string.Empty);
            var date = GetHeaderValue(headers, "Date", string.Empty);

            var stringToSign = string.Join("\n", method, md5, contentType, date, path);
            using var hasher = new HMACSHA1(Convert.FromBase64String(partnerApiKey));
            var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
            return hash.ToBase64String();
        }

        private static string GetEndPath(PathString pathString)
        {
            var path = pathString.ToUriComponent();
            var idx = path.IndexOf(_Routing, StringComparison.OrdinalIgnoreCase);
            return path[idx..].Replace(_Routing, string.Empty);
        }

        private static string GetHeaderValue(IHeaderDictionary headers, string key, string defaultValue)
            => headers.TryGetValue(key, out var value) ? value.ToString() : defaultValue;
    }
}