using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Infrastructures.Extensions;

namespace TGS.Apis.BackendOffice.Controllers
{
    public class BackendOfficeControllerBase : ControllerBase
    {
        private readonly Dictionary<string, string> UserInfoMap;
        public BackendOfficeControllerBase(IHttpContextAccessor httpContextAccessor)
        {
            UserInfoMap = httpContextAccessor.HttpContext.User.Claims.ToDictionary(p => p.Type, p => p.Value);
        }

        protected string Username => UserInfoMap[Consts.UserInfo.UsernameKey];

        protected string RoleName => UserInfoMap[Consts.UserInfo.RolenameKey];

        protected Guid? ResellerId => UserInfoMap[Consts.UserInfo.ResellerKey].IsNullOrEmpty() ? Guid.Empty : new Guid(UserInfoMap[Consts.UserInfo.ResellerKey]);

        protected Guid? BrandId => UserInfoMap[Consts.UserInfo.ResellerKey].IsNullOrEmpty() ? Guid.Empty : new Guid(UserInfoMap[Consts.UserInfo.LicenseeKey]);

        protected static TResponse CreateValidResponse<TResponse>() where TResponse : MessageResponse, new()
        {
            return new TResponse
            {
                Valid = true
            };
        }

        protected static TResponse CreateInvalidResponse<TResponse>(string message) where TResponse : MessageResponse, new()
        {
            return new TResponse
            {
                Valid = false,
                Message = message
            };
        }
    }
}
