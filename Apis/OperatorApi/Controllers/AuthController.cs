using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Services.Operations;
using TGS.Services.Operations.Validators;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class AuthController : ControllerBase
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IStatusValidator _statusValidator;
        private readonly IJwtTokenHandler _handler;

        public AuthController(
            IBrandOperations brandOperations,
            IStatusValidator statusValidator,
            IJwtTokenHandler handler)
        {
            _brandOperations = brandOperations;
            _statusValidator = statusValidator;
            _handler = handler;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("jwt")]
        public async Task<AuthResponse> GetJwtToken(AuthRequest request)
        {
            var brandId = await _brandOperations.GetBrandIdByApiKeyAsync(request.ApiKey);
            await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(brandId, ValidStatus.Default);

            var currency = request.Currency;
            
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, $"{brandId}"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim( Consts.ClaimName.Currency, currency)
            });

            var secretKey = await _brandOperations.GetBrandSecretKeyAsync(brandId);
            var signature = $"{brandId}:{currency}:{secretKey}";
            var expires = DateTime.Now.AddSeconds(Core.Models.Consts.TokenExpireSeconds.Jwt);

            var token = _handler.GenerateToken(identity, Encoding.UTF8.GetBytes(signature),
                Consts.Site, expires);

            return new()
            {
                Token = token,
                ExpireTime = new DateTimeOffset(expires).ToUnixTimeMilliseconds()
            };
        }
    }
}
