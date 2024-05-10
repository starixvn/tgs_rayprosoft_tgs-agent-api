using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using TGS.Apis.AgentApi.Attributes;
using Tgs.Core.Infrastructures.Security;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Tokens;
using TGS.Services.Agent.ApplicationServices.Agent;
using TGS.Services.Operations;
using Tgs.ServiceUtility;

namespace TGS.Apis.AgentApi.Middlewares;

public sealed class VerifyAgentTokenMiddleware(RequestDelegate next)
{
    private static readonly ActivitySource _VerifyAgentTokenMiddlewareSource =
        new($"TGS.Apis.AgentApi.Middlewares.{nameof(VerifyAgentTokenMiddleware)}");

    public async Task InvokeAsync(
        HttpContext context,
        ITokenProvider tokenProvider,
        IHashProvider hashProvider,
        IAgentOperations agentOperations,
        IBrandOperations brandOperations,
        IJwtTokenHandler handler,
        ICommandService<ValidateAgentLoginRecordCommand> loginRecordValidateService)
    {
        using var activity = _VerifyAgentTokenMiddlewareSource.StartActivity(nameof(VerifyAgentTokenMiddleware));

        var attribute = context.GetEndpoint()?.Metadata?.GetMetadata<AuthorizeAgentTokenAttribute>();

        if (attribute == null)
        {
            await next(context);
            return;
        }

        var token = context.Request.Headers.TryGetValue("Token", out var value) ? value.FirstOrDefault() : null;

        if (string.IsNullOrEmpty(token))
        {
            if (!attribute.AllowNullOrEmpty) throw new MissingTokenException(Resources.Agent.MissingAgentToken);

            await next(context);
            return;
        }

        var jwt = handler.ReadToken(token);

        var sub = jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(sub)) throw new MissingTokenException(Resources.Agent.MissingAgentToken);

        var tokenData = await tokenProvider.DecryptAsync<AgentTokenData>(sub);
        if (tokenData == null) throw new MissingTokenException(Resources.Agent.MissingAgentToken);

        context.Items.Add(Core.Models.Consts.ContextItems.AgentToken, tokenData);

        var agentId = tokenData.AgentId;
        await loginRecordValidateService.ExecuteAsync(new ValidateAgentLoginRecordCommand(tokenData.LoginRecordId));

        if (!attribute.NeedAuthorize)
        {
            await next(context);
            return;
        }

        var agent = await agentOperations.GetAgentAsync(agentId);
        var secretKey = await brandOperations.GetBrandSecretKeyAsync(agent.BrandId);

        var jti = jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
        var payload = hashProvider.Hash($"{sub}:{jti}");
        var signature = $"{payload}:{secretKey}";

        _ = handler.ValidateToken(token,
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Consts.Site,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signature)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            },
            exception => throw new InvalidCredentialsException(exception.Message),
            _ => throw new AgentLoginExpiredException(agentId));

        await next(context);
    }
}