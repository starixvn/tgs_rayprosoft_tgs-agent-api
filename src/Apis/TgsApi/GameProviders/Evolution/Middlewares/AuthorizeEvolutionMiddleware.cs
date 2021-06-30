using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.Evolution.Commons;

namespace TGS.Apis.TgsApi.GameProviders.Evolution.Middlewares
{
    public sealed class AuthorizeEvolutionMiddleware : IMiddleware
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;
        private readonly ITokenProvider _tokenProvider;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;

        public AuthorizeEvolutionMiddleware(
            IGameProviderOperations gameProviderOperations,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            ISettingOperations settingOperations,
            ITokenProvider tokenProvider,
            IJsonSerializationProvider jsonSerializationProvider)
        {
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<SexyGamingConfiguration>(SettingConsts.GameProviderConfigurations.Evolution);

            var body = await context.Request.ReadAsStringAsync();
            var baseRequest = _jsonSerializationProvider.DeserializeFromString<EvolutionBaseRequest>(body);

            var gameProviderId =
                    await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.Evolution);
            context.Items.Add(Consts.ContextItems.GameProviderId, gameProviderId);

            await next(context);
        }
    }
}