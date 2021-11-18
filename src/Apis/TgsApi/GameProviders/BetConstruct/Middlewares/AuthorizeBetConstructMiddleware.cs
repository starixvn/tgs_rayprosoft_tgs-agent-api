using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.GameProviders.BetConstruct.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.BetConstruct.Middlewares
{
    public sealed class AuthorizeBetConstructMiddleware : IMiddleware
    {
        private readonly IPlayerOperations _playerOperations;
        private readonly IBetConstructService _service;

        public AuthorizeBetConstructMiddleware(
            IPlayerOperations playerOperations,
            IBetConstructService service)
        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var value = await context.Request.ReadAsStringAsync();
            //var elements = JsonSerializer.Deserialize<IReadOnlyDictionary<string, object>>(value);

            //var gameProviderId = await _service.GetGameProviderIdByCodeAsync().ConfigureAwait(false);

            //context.Items.Add(ContextItems.GameProviderId, gameProviderId);

            //var tokenId = GetElementValue(elements, "AuthToken");

            //var loginRecord = await DecryptTokenAsync(tokenId);

            //var platformType = loginRecord.PlatformType;
            //var brandId = await _service.GetBrandIdByPlayerCodeAsync(loginRecord.PlayerCode);
            //var bgpc = await _service.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);

            //context.Items.Add(ContextItems.GameProviderConfigurationId, bgpc.GameProviderConfigurationId);
            //context.Items.Add(ContextItems.PlayerCode, loginRecord.PlayerCode);
            //context.Items.Add(ContextItems.PlatformType, platformType);
            await next(context);
        }

        private static IReadOnlyDictionary<string, string> GetElements(string xml)
        {
            var xDoc = XDocument.Parse(xml);

            if (xDoc.Root != null)
            {
                return xDoc.Root.Elements().Where(e => !e.Name.LocalName.Contains("record"))
                    .ToDictionary(a => a.Name.LocalName, a => a.Value);
            }

            return xDoc.Root?.Elements().First(e => e.Name.LocalName.Contains("Record"))
                    .Elements().Where(e => !e.Name.LocalName.Contains("transactionID"))
                    .ToDictionary(a => a.Name.LocalName, a => a.Value) ??
                new Dictionary<string, string>();
        }

        private static string GetElementValue(IReadOnlyDictionary<string, object> elements, string name)
        {
            var isSuccess = elements.TryGetValue(name, out var value);

            return isSuccess ? value.ToString() : null;
        }

        private async Task<CachedPlayerLoginRecord> DecryptTokenAsync(string tokenId)
        {
            if (tokenId.IsNullOrEmpty())
            {
                throw new MissingTokenException("empty token");
            }

            var loginRecordId = Guid.Parse(tokenId);
            var loginRecord = await _playerOperations.GetPlayerLoginRecordAsync(loginRecordId);

            if (loginRecord == null)
            {
                throw new PlayerLoginExpiredException();
            }

            return loginRecord;
        }
    }
}
