using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages.Users.Authentication;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Apis.BackendOffice.Services;

namespace TGS.Apis.BackendOffice.Controllers
{
    public partial class UserController
    {
        [AllowAnonymous, IgnoreAntiforgeryToken]
        [Route("auth")]
        [HttpPost, Description("Authenticate user login")]
        public async Task<AuthenticateUserResponse> AuthenticateAsync(AuthenticateUserRequest request)
        {
            var user = await _userQueries.GetUserByUsernameAsync(request.Username);

            if (user == null)
            {
                return CreateInvalidResponse<AuthenticateUserResponse>("User not found.");
            }

            if (PasswordMismatch(user, request.Password))
            {
                return CreateInvalidResponse<AuthenticateUserResponse>("Invalid password.");
            }

            if (_httpContextAccessor.HttpContext?.Connection.RemoteIpAddress != null)
                user.LastLoginIp = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress.ToString();

            user.LastLoginTime = DateTimeOffset.Now;

            await _userCommands.UpdateUserAsync(user);
            await SignInAsync(user.Username, user.Role.Name);

            return new AuthenticateUserResponse
            {
                Valid = true,
                UserInfo = new UserInfo
                {
                    Role = user.Role.Name,
                    Username = user.Username,
                    UserId = user.Id,
                    TimeZone = user.TimeZone,
                    Menus = _jsonSerializationProvider.DeserializeFromString<MenuSetting[]>(user.Role.MenuSetting)
                }
            };
        }

        [HttpPost("user")]
        public Dictionary<string, string> GetUser()
        {
            var user = HttpContext.User;
            return HttpContext.User.Claims.ToDictionary(p => p.Type, p => p.Value);
        }

        [HttpPost, Route("logout")]
        public Task LogoutAsync()
        {
            return _httpContextAccessor.HttpContext?.SignOutAsync();
        }

        private void SetCsrfToken()
        {
            var tokens = _antiForgery.GetAndStoreTokens(HttpContext);
            Response.Cookies.Append(
                Consts.CsrfTokenName,
                tokens.RequestToken ?? string.Empty,
                new CookieOptions
                {
                    HttpOnly = false,
                    Path = "/",
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax
                }
            );
        }

        private async Task SignInAsync(string username, string role)
        {
            SetCsrfToken();

            var claims = new List<Claim>
            {
                new Claim(Consts.UserInfo.UsernameKey, username),
                new Claim(Consts.UserInfo.RolenameKey, role),
                new Claim(Consts.UserInfo.ResellerKey, Guid.NewGuid().ToString()),
                new Claim(Consts.UserInfo.BrandKey, Guid.NewGuid().ToString()),
            };

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20),
                IsPersistent = true,
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await _httpContextAccessor.HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity), authProperties);
        }

        protected bool PasswordMismatch(User user, string password)
        {
            var salt = _hashProvider.Hash(user.Salt);

            var hashedPassword = _hashProvider.Hash(
                Encoding.UTF8.GetBytes(password),
                salt,
                _tgsHashConfig.HashAlgorithm);

            return !user.Password.SequenceEqual(hashedPassword);
        }
    }
}
