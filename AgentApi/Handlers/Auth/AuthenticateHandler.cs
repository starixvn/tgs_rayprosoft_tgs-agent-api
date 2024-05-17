using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TGS.Apis.AgentApi.Models;
using Tgs.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Services.Operations;
using TGS.Services.Operations.Validators;

namespace TGS.Apis.AgentApi.Handlers.Auth;

public sealed class AuthenticateHandler(
    IBrandOperations brandOperations,
    IStatusValidator statusValidator,
    IJwtTokenHandler handler)
    : IRequestHandler<AuthRequest, AuthResponse>
{
    async Task<AuthResponse> IRequestHandler<AuthRequest, AuthResponse>.Handle(
        AuthRequest request,
        CancellationToken cancellationToken)
    {
        var brandId = await brandOperations.GetBrandIdByApiKeyAsync(request.ApiKey);
        await statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, ValidStatus.Default);

        var currency = request.Currency;

        var identity = new ClaimsIdentity(new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, $"{brandId}"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(Consts.ClaimName.Currency, currency)
        });

        var secretKey = await brandOperations.GetBrandSecretKeyAsync(brandId);
        var signature = $"{brandId}:{currency}:{secretKey}";
        var expires = DateTime.Now.AddSeconds(Core.Models.Consts.TokenExpireSeconds.Jwt);

        var token = handler.GenerateToken(identity,
            Encoding.UTF8.GetBytes(signature),
            Consts.Site,
            expires);

        return new AuthResponse { Token = token, ExpiredOn = new DateTimeOffset(expires).ToUnixTimeMilliseconds() };
    }
}