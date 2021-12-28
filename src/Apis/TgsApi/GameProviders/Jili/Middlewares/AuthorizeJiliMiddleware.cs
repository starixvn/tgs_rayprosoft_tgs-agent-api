using System;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Tgs.Core.Infrastructures.Security;
using TGS.Apis.TgsApi.GameProviders.Jili.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.Jili.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Jili.Middlewares
{
    public class AuthorizeJiliMiddleware : IMiddleware
    {
        private static readonly string _Routing = "/api/jili";

        private readonly IBrandOperations _brandOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IJiliPlayerService _playerService;
        private readonly IJiliService _service;
        private readonly ITokenProvider _tokenProvider;

        public AuthorizeJiliMiddleware(
            IJiliService service,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            ITokenProvider tokenProvider,
            IJiliPlayerService playerService)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var authorization = GetAuthorizationValue(context.Request.Headers);

            var configuration = await _service.GetConfigurationAsync();
            var signature =
                Convert.ToBase64String(Encoding.UTF8.GetBytes($"{configuration.ClientId}:{configuration.SecretKey}"));

            if (signature != authorization)
            {
                throw new InvalidCredentialsException("Invalid authorization credential");
            }

            var path = GetEndPath(context.Request.Path);

            if (path.Contains("auth"))
            {
                var request = await context.Request.ReadAsObjectFromJsonAsync<JiliAuthorizePlayerRequest>();

                var (playerCode, _, platformType) = await ParseTokenAsync(request.Token);

                context.Items.Add(Consts.ContextItems.PlayerCode, playerCode);
                context.Items.Add(Consts.ContextItems.PlatformType, platformType);
            }
            else if (path.Contains("bet"))
            {
                var gameProviderId = await _service.GetGameProviderIdAsync();
                context.Items.Add(Consts.ContextItems.GameProviderId, gameProviderId);

                var request = await context.Request.ReadAsObjectFromJsonAsync<JiliPlaceBetAndSettleRequest>();

                var (playerCode, _, platformType) = await ParseTokenAsync(request.Token);

                context.Items.Add(Consts.ContextItems.PlayerCode, playerCode);
                context.Items.Add(Consts.ContextItems.PlatformType, platformType);

                var cachedPlayer = await _playerOperations.GetCachedPlayerAsync(playerCode);
                var brandId = cachedPlayer.BrandId;
                var gameProviderConfigurationId = await _brandOperations
                    .GetBrandGameProviderConfigurationIdAsync(brandId, gameProviderId);

                var jiliPlayerId = string.IsNullOrEmpty(request.JiliPlayerId)
                    ? _playerService.ToJiliPlayerId(cachedPlayer.BrandCode, playerCode)
                    : request.JiliPlayerId;

                context.Items.Add(Consts.ContextItems.BrandId, brandId);
                context.Items.Add(Consts.ContextItems.GameProviderConfigurationId, gameProviderConfigurationId);
                context.Items.Add(JiliConsts.ContextItems.JiliPlayerId, jiliPlayerId);
            }
            else if (path.Contains("cancelBet"))
            {
                var gameProviderId = await _service.GetGameProviderIdAsync();
                context.Items.Add(Consts.ContextItems.GameProviderId, gameProviderId);

                var request = await context.Request.ReadAsObjectFromJsonAsync<JiliCancelTransactionRequest>();

                var (playerCode, _, platformType) = await ParseTokenAsync(request.Token);

                context.Items.Add(Consts.ContextItems.PlayerCode, playerCode);
                context.Items.Add(Consts.ContextItems.PlatformType, platformType);

                var cachedPlayer = await _playerOperations.GetCachedPlayerAsync(playerCode);
                var brandId = cachedPlayer.BrandId;
                var gameProviderConfigurationId = await _brandOperations
                    .GetBrandGameProviderConfigurationIdAsync(brandId, gameProviderId);

                var jiliPlayerId = string.IsNullOrEmpty(request.JiliPlayerId)
                    ? _playerService.ToJiliPlayerId(cachedPlayer.BrandCode, playerCode)
                    : request.JiliPlayerId;

                context.Items.Add(Consts.ContextItems.BrandId, brandId);
                context.Items.Add(Consts.ContextItems.GameProviderConfigurationId, gameProviderConfigurationId);
                context.Items.Add(JiliConsts.ContextItems.JiliPlayerId, jiliPlayerId);
            }

            await next(context);
        }

        private string GetAuthorizationValue(IHeaderDictionary headers)
        {
            var authorization = GetHeaderValue(headers, "Authorization", string.Empty);

            if (AuthenticationHeaderValue.TryParse(authorization, out var parsedValue))
            {
                return parsedValue.Parameter;
            }

            throw new ArgumentException("Invalid Authorization header");
        }

        private string GetEndPath(PathString pathString)
        {
            var path = pathString.ToUriComponent();
            var idx = path.IndexOf(_Routing, StringComparison.OrdinalIgnoreCase);
            return path[idx..].Replace(_Routing, string.Empty);
        }

        private string GetHeaderValue(IHeaderDictionary headers, string key, string defaultValue)
            => headers.TryGetValue(key, out var value) ? value.ToString() : defaultValue;

        private async ValueTask<(string PlayerCode, Guid LoginRecordId, PlatformType PlatfromType)>
            ParseTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Missing Token");
            }

            var decrypted = await _tokenProvider.DecryptAsync<string>(token);

            var values = decrypted!.Split(',');

            if (values.Length != 3)
            {
                throw new ArgumentException("Invalid token structure");
            }

            return (values[0], Guid.Parse(values[1]), Enum.Parse<PlatformType>(values[2]));
        }
    }
}
