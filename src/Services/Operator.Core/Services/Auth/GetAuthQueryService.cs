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
    internal class GetAuthQueryService : IGetService<GetAuthQuery, ClaimsIdentity>
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IStatusValidator _statusValidator;

        public GetAuthQueryService(
            IBrandOperations brandOperations,
            IStatusValidator statusValidator)
        {
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _statusValidator = statusValidator ?? throw new ArgumentNullException(nameof(statusValidator));
        }

        public async ValueTask<ClaimsIdentity> GetAsync(GetAuthQuery query, CancellationToken cancellationToken = default)
        {
            var brandId = await _brandOperations.GetBrandIdByApiKeyAsync(query.ApiKey);
            await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, ValidStatus.Default);

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, $"{brandId}"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("curr", query.Currency)
            });

            return identity;
        }
    }
}