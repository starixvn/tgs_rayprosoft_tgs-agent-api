using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Services.Operations;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Auth
{
    internal class MakeBrandAuthTokenCommandService : IGetService<MakeBrandAuthTokenCommand, AuthInfo>
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IJwtTokenHandler _jwtTokenHandler;

        public MakeBrandAuthTokenCommandService(IJwtTokenHandler jwtTokenHandler, IBrandOperations brandOperations)
        {
            _jwtTokenHandler = jwtTokenHandler ?? throw new ArgumentNullException(nameof(jwtTokenHandler));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
        }

        public async ValueTask<AuthInfo> GetAsync(MakeBrandAuthTokenCommand query, CancellationToken cancellationToken = default)
        {
            var secretKey = await _brandOperations
                .GetBrandSecretKeyAsync(query.BrandId)
                .ConfigureAwait(false);

            var currency = query.Identity.FindFirst("curr")!.Value;
            var signature = $"{query.BrandId}:{currency}:{secretKey}";
            var expires = DateTime.Now.AddSeconds(Consts.TokenExpireSeconds.Jwt);

            var token = _jwtTokenHandler.GenerateToken(query.Identity, Encoding.UTF8.GetBytes(signature),
                query.Site, expires);

            return new AuthInfo(
                token,
                new DateTimeOffset(expires).ToUnixTimeMilliseconds());
        }
    }
}
