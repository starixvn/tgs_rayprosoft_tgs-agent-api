using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Tgs.Core.Infrastructures.Security;
using Tgs.ServiceUtility;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Resources;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Tokens;
using TGS.Services.Operations;
using TGS.Services.Operator.ApplicationServices.Player;

namespace TGS.Apis.OperatorApi.Middlewares
{
    public sealed class VerifyPlayerTokenMiddleware
    {
        private static readonly ActivitySource _VerifyPlayerTokenMiddlewareSource = new($"TGS.Apis.OperatorApi.Middlewares.{nameof(VerifyPlayerTokenMiddleware)}");

        private readonly RequestDelegate _next;

        public VerifyPlayerTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            ITokenProvider tokenProvider,
            IHashProvider hashProvider,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IJwtTokenHandler handler,
            ICommandService<ValidatePlayerLoginRecordCommand> loginRecordValidateService)
        {
            using var activity = _VerifyPlayerTokenMiddlewareSource.StartActivity(nameof(VerifyPlayerTokenMiddleware));

            var attribute = context.GetEndpoint()?.Metadata?.GetMetadata<AuthorizePlayerTokenAttribute>();

            if (attribute == null)
            {
                await _next(context);
                return;
            }

            var token = context.Request.Headers.TryGetValue("Token", out var value) ? value.FirstOrDefault() : null;

            if (string.IsNullOrEmpty(token))
            {
                if (!attribute.AllowNullOrEmpty)
                {
                    throw new MissingTokenException(Player.MissingPlayerToken);
                }

                await _next(context);
                return;
            }

            var jwt = handler.ReadToken(token);

            var sub = jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(sub))
            {
                throw new MissingTokenException(Player.MissingPlayerToken);
            }

            var data = await tokenProvider.DecryptAsync<PlayerTokenData>(sub);
            if (data == null)
            {
                throw new MissingTokenException(Player.MissingPlayerToken);
            }

            context.Items.Add(Core.Models.Consts.ContextItems.PlayerToken, data);

            var playerCode = data.PlayerCode;

            await loginRecordValidateService.ExecuteAsync(new ValidatePlayerLoginRecordCommand(data.LoginRecordId, playerCode));

            if (!attribute.NeedAuthorize)
            {
                await _next(context);

                return;
            }

            var licenseeId = await playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);
            var secretKey = await brandOperations.GetBrandSecretKeyAsync(licenseeId);

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
                 _ => throw new PlayerLoginExpiredException(playerCode));

            await _next(context);
        }
    }
}
