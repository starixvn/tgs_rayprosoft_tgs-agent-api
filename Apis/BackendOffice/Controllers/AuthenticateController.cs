using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages.Users.Authentication;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Apis.BackendOffice.DataService.Repositories;
using TGS.Apis.BackendOffice.Services;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Security;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api")]
    public class AuthenticateController : BaseContextController
    {
        private readonly IHashProvider _hashProvider;
        private readonly IAntiforgery _antiForgery;
        private readonly IBackendOfficeQueries<User> _userQueries;
        private readonly IBackendOfficeCommands<User> _userCommands;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticateController(
            IHashProvider hashProvider,
            IAntiforgery antiForgery,
            IBackendOfficeQueries<User> userQueries,
            IBackendOfficeCommands<User> userCommands,
            IJsonSerializationProvider jsonSerializationProvider,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _hashProvider = hashProvider;
            _antiForgery = antiForgery;
            _userQueries = userQueries;
            _userCommands = userCommands;
            _jsonSerializationProvider = jsonSerializationProvider;
            _httpContextAccessor = httpContextAccessor;
        }


        [AllowAnonymous, IgnoreAntiforgeryToken]
        [HttpPost("auth"), Description("Authenticate user login")]
        public async Task<AuthenticateUserResponse> AuthenticateAsync([FromBody] AuthenticateUserRequest request)
        {
            var user = await _userQueries.GetSingleAsync(p => p.Username == request.Username, u => u.Include(p => p.Role));

            if (user == null)
            {
                return CreateInvalidResponse<AuthenticateUserResponse>(Resources.User.NotFound);
            }

            if (!_hashProvider.PasswordMatched(user.Password, user.Salt, request.Password))
            {
                return CreateInvalidResponse<AuthenticateUserResponse>(Resources.User.InvalidPassword);
            }

            if (_httpContextAccessor.HttpContext?.Connection.RemoteIpAddress != null)
                user.LastLoginIp = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress.ToString();

            user.LastLoginTime = DateTimeOffset.Now;

            await _userCommands.UpdateAsync(user);
            await SignInAsync(user);

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

        [HttpPost, Route("logout")]
        public Task LogoutAsync()
        {
            return _httpContextAccessor.HttpContext?.SignOutAsync();
        }

        private void SetCsrfToken()
        {
            var tokens = _antiForgery.GetAndStoreTokens(HttpContext);
            _httpContextAccessor.HttpContext.Response.Cookies.Append(
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

        private async Task SignInAsync(User user)
        {
            SetCsrfToken();

            var claims = new List<Claim>
            {
                new Claim(Consts.UserInfo.UsernameKey, user.Username),
                new Claim(Consts.UserInfo.RolenameKey, user.Role.Name),
                new Claim(Consts.UserInfo.ResellerKey, user.ResellerId.ToString()),
                new Claim(Consts.UserInfo.LicenseeKey, user.LicenseeId.ToString()),
                new Claim(Consts.UserInfo.PortalKey, user.Role.RoleType.ToString()),
            };

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20),
                IsPersistent = true,
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await _httpContextAccessor.HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity), authProperties);
        }
    }
}
