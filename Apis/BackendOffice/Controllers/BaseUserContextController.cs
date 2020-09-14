using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using TGS.Infrastructures.Extensions;

namespace TGS.Apis.BackendOffice.Controllers
{
    public class BaseUserContextController : BaseMessageController
    {
        private readonly Dictionary<string, string> _userInfoMap;
        public BaseUserContextController(IHttpContextAccessor httpContextAccessor)
        {
            _userInfoMap = httpContextAccessor.HttpContext.User.Claims.ToDictionary(p => p.Type, p => p.Value);
        }

        protected string Username => _userInfoMap[Consts.UserInfo.UsernameKey];

        protected string RoleName => _userInfoMap[Consts.UserInfo.RolenameKey];

        protected Guid? ResellerId => _userInfoMap[Consts.UserInfo.ResellerKey].IsNullOrEmpty() ? Guid.Empty : new Guid(_userInfoMap[Consts.UserInfo.ResellerKey]);

        protected Guid? BrandId => _userInfoMap[Consts.UserInfo.ResellerKey].IsNullOrEmpty() ? Guid.Empty : new Guid(_userInfoMap[Consts.UserInfo.LicenseeKey]);
    }
}
