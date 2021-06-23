using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Services.Operations;
using TGS.Services.Operations.Validators;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Auth
{
    internal class GetAuthQueryService : IGetService<GetAuthQuery, AuthInfo>
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IStatusValidator _statusValidator;
        private readonly IJwtTokenHandler _handler;

        public GetAuthQueryService(
            IBrandOperations brandOperations,
            IStatusValidator statusValidator,
            IJwtTokenHandler handler)
        {
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _statusValidator = statusValidator ?? throw new ArgumentNullException(nameof(statusValidator));
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public async ValueTask<AuthInfo> GetAsync(GetAuthQuery query, CancellationToken cancellationToken = default)
        {
            var brandId = await _brandOperations.GetBrandIdByApiKeyAsync(query.ApiKey);
            await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, ValidStatus.Default);

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, $"{brandId}"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim( query.ClaimNameCurrency, query.Currency)
            });

            var secretKey = await _brandOperations.GetBrandSecretKeyAsync(brandId);
            var signature = $"{brandId}:{query.Currency}:{secretKey}";
            var expires = DateTime.Now.AddSeconds(Consts.TokenExpireSeconds.Jwt);

            var token = _handler.GenerateToken(identity, Encoding.UTF8.GetBytes(signature),
                query.Site, expires);

            return new AuthInfo(
                token,
                new DateTimeOffset(expires).ToUnixTimeMilliseconds());
        }
    }
}