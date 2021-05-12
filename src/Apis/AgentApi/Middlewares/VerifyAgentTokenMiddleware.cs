using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Resources;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Tokens;
using TGS.Services.Operations;
using TGS.Services.Operations.Validators;

namespace TGS.Apis.AgentApi.Middlewares
{
    public sealed class VerifyAgentTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public VerifyAgentTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            ITokenProvider tokenProvider,
            IHashProvider hashProvider,
            IAgentOperations agentOperations,
            IBrandOperations brandOperations,
            IExpirationValidator expirationValidator,
            IJwtTokenHandler handler)
        {
            var attribute = context.GetEndpoint()?.Metadata?.GetMetadata<AuthorizeAgentTokenAttribute>();

            if (attribute == null)
            {
                await _next(context);
                return;
            }

            var token = context.Request.Headers.TryGetValue("Token", out var value) ? value.FirstOrDefault() : null;

            if (token.IsNullOrEmpty())
            {
                if (!attribute.AllowNullOrEmpty)
                {
                    throw new MissingTokenException(Agent.MissingAgentToken);
                }

                await _next(context);
                return;
            }

            var jwt = handler.ReadToken(token);

            var sub = jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (sub.IsNullOrEmpty())
            {
                throw new MissingTokenException(Agent.MissingAgentToken);
            }

            var data = await tokenProvider.DecryptAsync<AgentTokenData>(sub);
            context.Items.Add(Core.Models.Consts.ContextItems.AgentToken, data);

            var agentId = data.AgentId;
            await expirationValidator.ThrowIfAgentLoginExpiredAsync(data.LoginRecordId);

            if (!attribute.NeedAuthorize)
            {
                await _next(context);
                return;
            }

            var agent = await agentOperations.GetAgentAsync(agentId);
            var secretKey = await brandOperations.GetBrandSecretKeyAsync(agent.BrandId);

            var jti = jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var payload = hashProvider.Hash($"{sub}:{jti}");
            var signature = $"{payload}:{secretKey}";

            handler.ValidateToken(token,
                new()
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

            await _next(context);
        }
    }
}