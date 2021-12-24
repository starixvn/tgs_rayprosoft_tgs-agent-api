using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Tgs.Core.Infrastructures.Security;
using TGS.Apis.TgsApi.GameProviders.TF.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.TF.Middlewares
{
    public sealed class AuthorizeTFMiddleware : IMiddleware
    {
        private readonly ITFService _service;
        private readonly ISettingOperations _settingOperations;        
        private readonly ITokenProvider _tokenProvider;

        public AuthorizeTFMiddleware(
            ITFService service,
            ITokenProvider tokenProvider,
            ISettingOperations settingOperations)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));            
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var value = await context.Request.ReadAsStringAsync();

            
            var elements = JsonSerializer.Deserialize<IReadOnlyDictionary<string, object>>(value);


            //var configuration = await _settingOperations
            //    .GetValueFromJsonByKeyAsync<TFConfiguration>(SettingConsts.GameProviderConfigurations.TFGaming);

            //context.Items.Add("publicToken", configuration.PublicToken);

            //var gameProviderId = await _service.GetGameProviderIdByCodeAsync().ConfigureAwait(false);

            //context.Items.Add(ContextItems.GameProviderId, gameProviderId);

            //var authToken = GetElementValue(elements, "AuthToken");
            //var playerCode = await _tokenProvider.DecryptAsync<string>(authToken);

            //var brandId = await _service.GetBrandIdByPlayerCodeAsync(playerCode);
            //var bgpc = await _service.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);

            //context.Items.Add(ContextItems.GameProviderConfigurationId, bgpc.GameProviderConfigurationId);
            //context.Items.Add(ContextItems.PlayerCode, playerCode);

            await next(context);
        }

        private static string GetElementValue(IReadOnlyDictionary<string, object> elements, string name)
        {
            var isSuccess = elements.TryGetValue(name, out var value);

            return isSuccess ? value.ToString() : null;
        }
    }
}
