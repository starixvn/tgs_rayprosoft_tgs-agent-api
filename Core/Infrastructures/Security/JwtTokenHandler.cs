using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace TGS.Core.Infrastructures.Security
{
    public interface IJwtTokenHandler
    {
        string GenerateToken(ClaimsIdentity identity, byte[] securityKey, string issuer, DateTime? expires);
    }

    public sealed class JwtTokenHandler : IJwtTokenHandler
    {
        string IJwtTokenHandler.GenerateToken(ClaimsIdentity identity, byte[] securityKey, string issuer, DateTime? expires)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(securityKey);
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Issuer = issuer,
                Expires = expires,
                SigningCredentials = credential
            };

            var securityToken = handler.CreateToken(descriptor);
            return handler.WriteToken(securityToken);
        }
    }
}