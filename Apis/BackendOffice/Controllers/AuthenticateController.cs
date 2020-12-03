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
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.DataServices.Models;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models.Services;
using TGS.Services.BackOffice.Domain;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api")]
    public class AuthenticateController : BaseContextController
    {
        private readonly IAntiforgery _antiForgery;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IPasswordHandler _passwordHandler;
        private readonly ITgsCommands<User> _userCommands;
        private readonly ITgsQueries<User> _userQueries;

        public AuthenticateController(
            IPasswordHandler passwordHandler,
            IAntiforgery antiForgery,
            ITgsQueries<User> userQueries,
            ITgsCommands<User> userCommands,
            IJsonSerializationProvider jsonSerializationProvider,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _passwordHandler = passwordHandler;
            _antiForgery = antiForgery;
            _userQueries = userQueries;
            _userCommands = userCommands;
            _jsonSerializationProvider = jsonSerializationProvider;
        }

        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        [HttpPost("auth")]
        [Description("Authenticate user login")]
        public async Task<AuthenticateUserResponse> AuthenticateAsync([FromBody] AuthenticateUserRequest request)
        {
            var user = await _userQueries.GetSingleAsync(p => p.Username == request.Username,
                u => u.Include(p => p.Role));

            if (user == null)
            {
                return CreateInvalidResponse<AuthenticateUserResponse>(Resources.User.NotFound);
            }

            if (!_passwordHandler.Verify(user.Password, user.Salt, request.Password))
            {
                return CreateInvalidResponse<AuthenticateUserResponse>(Resources.User.InvalidPassword);
            }

            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                user.LastLoginIp = HttpContext.Connection.RemoteIpAddress.ToString();
            }

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

        [HttpPost]
        [Route("logout")]
        public Task LogoutAsync()
        {
            return HttpContext.SignOutAsync();
        }

        private void SetCsrfToken()
        {
            var tokens = _antiForgery.GetAndStoreTokens(HttpContext);
            HttpContext.Response.Cookies.Append(
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

        private Task SignInAsync(User user)
        {
            SetCsrfToken();

            var claims = new List<Claim>
            {
                new Claim(Consts.UserInfo.UsernameKey, user.Username),
                new Claim(Consts.UserInfo.RolenameKey, user.Role.Name),
                new Claim(Consts.UserInfo.RoleTypeKey, user.Role.RoleType.ToString()),
                new Claim(Consts.UserInfo.ResellerKey, user.Role.ResellerId.ToString()),
                new Claim(Consts.UserInfo.LicenseeKey, user.Role.LicenseeId.ToString()),
                new Claim(Consts.UserInfo.BrandKey, user.Role.BrandId.ToString()),
                new Claim(Consts.UserInfo.PortalKey, user.Role.RoleType.ToString())
            };

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20),
                IsPersistent = true
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity), authProperties);
        }
    }
}