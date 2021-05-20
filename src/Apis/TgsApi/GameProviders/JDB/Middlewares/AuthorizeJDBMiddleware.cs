using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.JDB.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.JDB.Middlewares
{
    public sealed class AuthorizeJDBMiddleware : IMiddleware
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IJsonSerializationProvider _json;

        public AuthorizeJDBMiddleware(
            IGameProviderOperations gameProviderOperations,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IJsonSerializationProvider json)
        {
            _brandOperations = brandOperations;
            _gameProviderOperations = gameProviderOperations;
            _playerOperations = playerOperations;
            _json = json;
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!context.Request.Form.TryGetValue("x", out var jsonValue))
            {
                throw new ArgumentException();
            }

            var baseRequest = _json.DeserializeFromString<BaseActionRequest>(jsonValue);
            var gameProviderId =
                await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.JDB);
            var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(baseRequest.UserId);
            var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);
            
            context.Items.Add(Consts.ContextItems.GameProviderId, gameProviderId);
            context.Items.Add(Consts.ContextItems.GameProviderConfigurationId, bgpc.GameProviderConfigurationId);
            context.Items.Add(Consts.ContextItems.PlayerCode, baseRequest.UserId);
            
            await next(context);
        }
    }
}