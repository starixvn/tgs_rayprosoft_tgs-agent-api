using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Models;
using TGS.Services.Operations;
using TGS.Services.Operations.Validators;
using Tgs.ServiceUtility;
using TGS.Services.Operator.ApplicationServices.Auth;

namespace TGS.Services.Operator.ApplicationServices.Auth
{
    internal class GetAuthQueryService : IGetService<GetAuthQuery, ClaimsIdentity>
    {
        private readonly IBrandOperations _brandOperations;

        public GetAuthQueryService(
            IBrandOperations brandOperations)
        {
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
        }

        public async ValueTask<ClaimsIdentity> GetAsync(
            GetAuthQuery query,
            CancellationToken cancellationToken = default)
        {
            var brandId = await _brandOperations.GetBrandIdByApiKeyAsync(query.ApiKey);

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
