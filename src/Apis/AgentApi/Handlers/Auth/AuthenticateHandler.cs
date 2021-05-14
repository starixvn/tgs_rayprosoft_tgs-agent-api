using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TGS.Apis.AgentApi.Models;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Services.Operations;
using TGS.Services.Operations.Validators;

namespace TGS.Apis.AgentApi.Handlers.Auth
{
    public sealed class AuthenticateHandler : IRequestHandler<AuthRequest, AuthResponse>
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IJwtTokenHandler _handler;
        private readonly IStatusValidator _statusValidator;

        public AuthenticateHandler(
            IBrandOperations brandOperations,
            IStatusValidator statusValidator,
            IJwtTokenHandler handler)
        {
            _brandOperations = brandOperations;
            _statusValidator = statusValidator;
            _handler = handler;
        }

        async Task<AuthResponse> IRequestHandler<AuthRequest, AuthResponse>.Handle(AuthRequest request,
            CancellationToken cancellationToken)
        {
            var brandId = await _brandOperations.GetBrandIdByApiKeyAsync(request.ApiKey);
            await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, ValidStatus.Default);

            var currency = request.Currency;

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, $"{brandId}"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(Consts.ClaimName.Currency, currency)
            });

            var secretKey = await _brandOperations.GetBrandSecretKeyAsync(brandId);
            var signature = $"{brandId}:{currency}:{secretKey}";
            var expires = DateTime.Now.AddSeconds(Core.Models.Consts.TokenExpireSeconds.Jwt);

            var token = _handler.GenerateToken(identity, Encoding.UTF8.GetBytes(signature),
                Consts.Site, expires);

            return new() {Token = token, ExpiredOn = new DateTimeOffset(expires).ToUnixTimeMilliseconds()};
        }
    }
}