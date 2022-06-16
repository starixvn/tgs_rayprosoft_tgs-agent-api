using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TGS.Apis.TgsApi.GameProviders.DreamGame.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;

namespace TGS.Apis.TgsApi.GameProviders.DreamGame.Middlewares;

public sealed class AuthorizeDreamGameMiddleware : IMiddleware
{
    private readonly ILogger<AuthorizeDreamGameMiddleware> _logger;
    private readonly IDreamGameService _service;

    public AuthorizeDreamGameMiddleware(
        IDreamGameService service,
        ILogger<AuthorizeDreamGameMiddleware> logger)
    {
        _service = service ?? throw new System.ArgumentNullException(nameof(service));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var path = context.Request.Path.ToString();

        var configuration = await _service.GetDreamGameConfigurationAsync().ConfigureAwait(false);
        var jsonDocument = JsonDocument.Parse(await context.Request.ReadAsStringAsync().ConfigureAwait(false));
        var token = jsonDocument.RootElement.GetProperty("token").GetString();
        var key = _service.GetHashMD5(configuration.AgentName, configuration.ApiKey);

        _logger.LogInformation(eventId: new EventId(10000), message: $"GP Token:{token} | TGS Token:{key}");

        if (key != token)
        {
            throw new InvalidCredentialsException($"{token }");
        }

        var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
        context.Items.Add(Consts.ContextItems.GameProviderId, gameProviderId);

        if (path.Contains("user/getBalance/", StringComparison.CurrentCultureIgnoreCase) ||
            path.Contains("account/inform/", StringComparison.CurrentCultureIgnoreCase) ||
            path.Contains("account/transfer/", StringComparison.CurrentCultureIgnoreCase))
        {
            var playerCode = jsonDocument.RootElement.GetProperty("member").GetProperty("username").GetString().ToLower();
            var playerLoginRecordModel = await _service.GetPlayerLoginLastAsync(playerCode).ConfigureAwait(false);
            if (playerLoginRecordModel == null)
            {
                throw new PlayerLoginExpiredException($"{playerCode}");
            }
            var gameProviderConfigurationId = await _service.GetGameProviderConfigurationIdAsync(playerLoginRecordModel.BrandId).ConfigureAwait(false);
            context.Items.Add(Consts.ContextItems.GameProviderConfigurationId, gameProviderConfigurationId);
            context.Items.Add(Consts.ContextItems.PlayerCode, playerLoginRecordModel.PlayerCode);
            context.Items.Add(Consts.ContextItems.PlatformType, playerLoginRecordModel.PlatformType);
        }
        await next(context);
    }
}
