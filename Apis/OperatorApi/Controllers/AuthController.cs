using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TGS.Apis.OperatorApi.Models.Messages.Authentications;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Services.Operations;
using TGS.Services.Operations.Validators;
using TokenExpireSeconds = TGS.Core.Models.Consts.TokenExpireSeconds;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class AuthController : ControllerBase
    {
        private readonly ILicenseeOperations _licenseeOperations;
        private readonly IStatusValidator _statusValidator;
        private readonly IJwtTokenHandler _handler;

        public AuthController(
            ILicenseeOperations licenseeOperations,
            IStatusValidator statusValidator,
            IJwtTokenHandler handler)
        {
            _licenseeOperations = licenseeOperations;
            _statusValidator = statusValidator;
            _handler = handler;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("jwt")]
        public async Task<AuthResponse> GetJwtToken(AuthRequest request)
        {
            var licenseeId = await _licenseeOperations.GetLicenseeIdByApiKeyAsync(request.ApiKey);
            await _statusValidator.ThrowIfLicenseeStatusIsNotOneOfAsync(licenseeId, ValidStatus.Default);

            var currency = request.Currency;
            
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, $"{licenseeId}"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim( Consts.ClaimName.Currency, currency)
            });

            var secretKey = await _licenseeOperations.GetLicenseeSecretKeyAsync(licenseeId);
            var signature = $"{licenseeId}:{currency}:{secretKey}";
            var expires = DateTime.Now.AddSeconds(TokenExpireSeconds.Jwt);

            var token = _handler.GenerateToken(identity, Encoding.UTF8.GetBytes(signature),
                Consts.Site, expires);

            return new AuthResponse
            {
                Token = token,
                ExpireTime = new DateTimeOffset(expires).ToUnixTimeMilliseconds()
            };
        }
    }
}
