using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace TGS.Core.Infrastructures.Security
{
    public interface IJwtTokenHandler
    {
        string GenerateToken(ClaimsIdentity identity, byte[] securityKey, string issuer, DateTime? expires);

        JwtSecurityToken ReadToken(string token);

        /// <summary>
        /// Validate Jwt Token
        /// </summary>
        /// <remarks>
        /// It will use onError implementation by default when onExpired is null, if onError is null, means all errors will be passed.
        /// </remarks>
        (bool IsValid, ClaimsPrincipal ClaimsPrincipal, SecurityToken ValidatedToken) ValidateToken(string token,
            TokenValidationParameters parameters,
            Action<Exception> onError,
            Action<SecurityTokenExpiredException> onExpired = null);
    }

    public sealed class JwtTokenHandler : IJwtTokenHandler
    {
        private readonly JwtSecurityTokenHandler _handler = new();

        string IJwtTokenHandler.GenerateToken(ClaimsIdentity identity, byte[] securityKey, string issuer,
            DateTime? expires)
        {
            var key = new SymmetricSecurityKey(securityKey);
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Issuer = issuer,
                Expires = expires,
                SigningCredentials = credential
            };

            var securityToken = _handler.CreateToken(descriptor);
            return _handler.WriteToken(securityToken);
        }

        JwtSecurityToken IJwtTokenHandler.ReadToken(string token) =>
            _handler.ReadJwtToken(token);

        (bool, ClaimsPrincipal, SecurityToken) IJwtTokenHandler.ValidateToken(string token,
            TokenValidationParameters parameters, Action<Exception> onError,
            Action<SecurityTokenExpiredException> onExpired)
        {
            try
            {
                var claimsPrincipal = _handler.ValidateToken(token, parameters, out var validatedToken);
                return (true, claimsPrincipal, validatedToken);
            }
            catch (SecurityTokenExpiredException e)
            {
                onExpired ??= onError;
                onExpired?.Invoke(e);
            }
            catch (Exception e)
            {
                onError?.Invoke(e);
            }

            return (false, null, null);
        }
    }
}