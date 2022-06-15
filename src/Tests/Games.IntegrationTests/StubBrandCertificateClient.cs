using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Tgs.BrandCertificate.Clients;

namespace TGS.Tests.Games.IntegrationTests
{
    internal class StubBrandCertificateClient : IBrandCertificateClient
    {
        private static SecurityKey _SecurityKey = new RsaSecurityKey(RSA.Create(512));
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        private readonly SigningCredentials _signingCredentials;

        public StubBrandCertificateClient()
        {
            _signingCredentials = new SigningCredentials(_SecurityKey, SecurityAlgorithms.RsaSha256);
        }

        public Task<SecurityKey> GetSystemValidateSecurityKeyAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(_SecurityKey);

        public Task<string> MakeJsonWebTokenAsync(Guid brandId, CancellationToken cancellationToken = default)
        {
            var id = new ClaimsIdentity(new[]{
                new Claim(JwtRegisteredClaimNames.Name, "GPI"),
                new Claim(JwtRegisteredClaimNames.Sub, "4e86719f-243f-46f6-b010-5c758b7285af")
            });

            var token = _jwtSecurityTokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = "tgs",
                Subject = id,
                SigningCredentials = _signingCredentials
            });

            return Task.FromResult(_jwtSecurityTokenHandler.WriteToken(token));
        }
    }
}
