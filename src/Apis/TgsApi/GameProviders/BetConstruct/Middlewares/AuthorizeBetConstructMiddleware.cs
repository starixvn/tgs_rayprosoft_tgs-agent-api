using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Tgs.Core.Infrastructures.Security;
using TGS.Apis.TgsApi.GameProviders.BetConstruct.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Shared.Game.ApiClients.BetConstruct;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.BetConstruct.Middlewares
{
    public sealed class AuthorizeBetConstructMiddleware : IMiddleware
    {
        private static readonly string _Routing = "/bc/api";
        private readonly IPlayerOperations _playerOperations;
        private readonly IBetConstructService _service;
        private readonly ITokenProvider _tokenProvider;
        private readonly ISettingOperations _settingOperations;
        private readonly IBetConstructUtility _utility;

        public AuthorizeBetConstructMiddleware(
            IPlayerOperations playerOperations,
            IBetConstructService service,
            ITokenProvider tokenProvider,
            ISettingOperations settingOperations,
            IBetConstructUtility utility)
        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var value = await context.Request.ReadAsStringAsync();

            var elements = JsonSerializer.Deserialize<IReadOnlyDictionary<string, object>>(value);

            var configuration = await _settingOperations.GetValueFromJsonByKeyAsync<BetConstructConfiguration>(
                SettingConsts.GameProviderConfigurations.BetConstruct);            

            context.Items.Add("sharedKey", configuration.SharedKey);

            var gameProviderId = await _service.GetGameProviderIdByCodeAsync().ConfigureAwait(false);

            context.Items.Add(ContextItems.GameProviderId, gameProviderId);            
            
            //var signature = GetElementValue(elements, "Hash");

            //TODO:將Hash的檢查移到controller裡面檢查
            //var path = GetEndPath(context.Request.Path);
            //var hashKey = Array.Empty<string>();
            //if (path.Contains("GetClientDetails") || path.Contains("GetClientBalance"))
            //{
            //    hashKey = new string[] { "AuthToken","TS" };
            //}
            //else if(path.Contains("BetPlaced"))
            //{
            //    hashKey = new string[] { "AuthToken", "TS", "TransactionId", "BetId", "Amount", "Created", "BetType", "SystemMinCount", "TotalPrice" };
            //}
            //else if (path.Contains("BetResulted"))
            //{
            //    hashKey = new string[] { "AuthToken", "TS", "TransactionId", "BetId", "BetState", "Amount", "BonusAmount", "BonusId" };
            //}
            //else if (path.Contains("Rollback"))
            //{
            //    hashKey = new string[] { "AuthToken", "TS", "TransactionId" };
            //}

            //var hashDict = elements.Where(x => hashKey.Contains(x.Key)).OrderBy(x=> new {x. }).ToDictionary(x => x.Key, x => x.Value);

            //var computedSignature = ComputeSignature(hashDict, configuration.SharedKey);


            //if (signature.NotEquals(computedSignature))
            //{
            //    throw new InvalidCredentialsException(
            //        $"Signature is mismatched. The signature in the request is {signature}, which is different from {computedSignature} as computed.");
            //}

            var authToken = GetElementValue(elements, "AuthToken");
            var playerCode = await _tokenProvider.DecryptAsync<string>(authToken); 

            //var platformType = loginRecord.PlatformType;
            var brandId = await _service.GetBrandIdByPlayerCodeAsync(playerCode);
            var bgpc = await _service.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);

            context.Items.Add(ContextItems.GameProviderConfigurationId, bgpc.GameProviderConfigurationId);
            context.Items.Add(ContextItems.PlayerCode, playerCode);
            //context.Items.Add(ContextItems.PlatformType, platformType);
            await next(context);
        }



       

        private static string GetEndPath(PathString pathString)
        {
            var path = pathString.ToUriComponent();
            var idx = path.IndexOf(_Routing, StringComparison.OrdinalIgnoreCase);
            return path[idx..].Replace(_Routing, string.Empty);
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

    public class HashKeyComparer : IComparer<string>
    {
        public int Compare(string x, string y) => throw new NotImplementedException();
    }
}
